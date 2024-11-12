using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator animator;
    [SerializeField] protected WaypointData[] waypoints;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float chaseSpeed = 4f; // Скорость преследования
    [SerializeField] protected ParticleSystem actionParticles;
    [SerializeField] protected LayerMask playerLayer; // Слой игрока
    [SerializeField] protected LayerMask hideLayer; // Слой, где игрок может прятаться

    // Параметры лучей
    [SerializeField] protected float backRayLength = 5f;
    [SerializeField] protected float frontRayLength = 8f;
    [SerializeField] protected float extendedFrontRayLength = 12f;
    protected int lastKnownWaypointIndex = 0;
    protected int currentWaypointIndex = 0;
    protected bool isMoving = true;
    protected bool isChasing = false;
    protected Vector3 lastWaypointPosition;
    protected Transform playerTransform;
    protected SpriteRenderer spriteRenderer;
    protected Vector2 facingDirection;

    [System.Serializable]
    public struct WaypointData
    {
        public Transform point;
        public bool enableParticles;
        public bool openDoor;
        public float waitDuration;
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (waypoints.Length > 0)
        {
            MoveToNextWaypoint();
        }
    }

    protected virtual void Update()
    {
        if (!isChasing && isMoving && currentWaypointIndex < waypoints.Length)
        {
            MoveTowardsWaypoint();
        }
        else if (isChasing && playerTransform != null)
        {
            ChasePlayer();
        }

        CheckPlayerDetection();
        UpdateFacingDirection();
    }

    protected void UpdateFacingDirection()
    {
        if (rb.velocity.x != 0)
        {
            spriteRenderer.flipX = rb.velocity.x < 0;
            facingDirection = new Vector2(Mathf.Sign(rb.velocity.x), 0);
        }
    }

    protected void CheckPlayerDetection()
    {
        // Направление взгляда врага
        float direction = spriteRenderer.flipX ? -1 : 1;
        Vector2 position = transform.position;

        // Отрисовка лучей для отладки
        Debug.DrawRay(position, Vector2.right * direction * (isChasing ? extendedFrontRayLength : frontRayLength), Color.red);
        Debug.DrawRay(position, Vector2.right * -direction * backRayLength, Color.blue);

        // Проверка переднего луча
        RaycastHit2D frontHit = Physics2D.Raycast(position, Vector2.right * direction,
            isChasing ? extendedFrontRayLength : frontRayLength, playerLayer | hideLayer);

        // Проверка заднего луча
        RaycastHit2D backHit = Physics2D.Raycast(position, Vector2.right * -direction,
            backRayLength, playerLayer | hideLayer);

        HandlePlayerDetection(frontHit, backHit);
    }
    protected void HandlePlayerDetection(RaycastHit2D frontHit, RaycastHit2D backHit)
    {
        if (!isChasing)
        {
            // Начинаем погоню только если обнаружили игрока на нужном слое
            if ((frontHit.collider != null || backHit.collider != null) &&
                ((frontHit.collider?.gameObject.layer == LayerMask.NameToLayer("Player")) ||
                 (backHit.collider?.gameObject.layer == LayerMask.NameToLayer("Player"))))
            {
                StartChasing(frontHit.collider != null ? frontHit.collider.transform : backHit.collider.transform);
            }
        }
        else
        {
            // Проверяем, виден ли все еще игрок
            bool playerVisible = false;

            if (frontHit.collider != null && frontHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                playerVisible = true;
            if (backHit.collider != null && backHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                playerVisible = true;

            // Если игрок не виден или находится в слое Hide, прекращаем погоню
            if (!playerVisible ||
                (frontHit.collider?.gameObject.layer == LayerMask.NameToLayer("Hidden") ||
                 backHit.collider?.gameObject.layer == LayerMask.NameToLayer("Hidden")))
            {
                StopChasing();
            }
        }
    }

    protected void StartChasing(Transform player)
    {
        if (!isChasing)
        {
            isChasing = true;
            playerTransform = player;
            lastWaypointPosition = transform.position;
            lastKnownWaypointIndex = currentWaypointIndex;

            // Отключаем анимацию ходьбы и включаем анимацию погони
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsChasing", true);

            StopAllCoroutines();
        }
    }

    protected void ChasePlayer()
    {
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = direction * chaseSpeed;
            // Убираем установку IsWalking в true во время погони
            // animator.SetBool("IsWalking", true); - эту строку удаляем
        }
    }

    protected void StopChasing()
    {
        if (isChasing)
        {
            isChasing = false;
            playerTransform = null;

            // Сбрасываем обе анимации
            animator.SetBool("IsChasing", false);
            animator.SetBool("IsWalking", false);

            rb.velocity = Vector2.zero;

            // Восстанавливаем индекс точки маршрута
            currentWaypointIndex = lastKnownWaypointIndex;

            // Запускаем движение к следующей точке
            MoveToNextWaypoint();
        }
    }
    private IEnumerator ReturnToLastPosition()
    {
        Vector2 returnPosition = waypoints[lastKnownWaypointIndex].point.position;

        while (Vector2.Distance(transform.position, returnPosition) > 0.1f)
        {
            Vector2 direction = (returnPosition - (Vector2)transform.position).normalized;
            rb.velocity = direction * moveSpeed;
            animator.SetBool("IsWalking", true);
            yield return null;
        }

        rb.velocity = Vector2.zero;
        animator.SetBool("IsWalking", false);

        // Восстанавливаем индекс точки маршрута
        currentWaypointIndex = lastKnownWaypointIndex;

        // Проверяем, были ли прерваны какие-то действия на точке
        WaypointData currentData = waypoints[currentWaypointIndex];
        if (Vector2.Distance(transform.position, currentData.point.position) < 0.1f)
        {
            // Если мы вернулись точно на точку, выполняем действия и идем дальше
            StartCoroutine(HandleWaypointActions());
        }
        else
        {
            // Если мы не точно на точке, просто продолжаем движение к ней
            isMoving = true;
            // НЕ увеличиваем currentWaypointIndex, продолжаем идти к текущей точке
        }
    }

    private IEnumerator HandleWaypointActions()
    {
        isMoving = false;
        WaypointData currentData = waypoints[currentWaypointIndex];

        if (currentData.enableParticles && actionParticles != null)
        {
            actionParticles.Play();
        }

        animator.SetBool("IsActing", true);
        yield return new WaitForSeconds(currentData.waitDuration);
        animator.SetBool("IsActing", false);

        if (currentData.enableParticles && actionParticles != null)
        {
            actionParticles.Stop();
        }

        if (currentData.openDoor)
        {
            Door door = currentData.point.GetComponent<Door>();
            if (door != null)
            {
                door.Interact(gameObject);
            }
        }

        // Переходим к следующей точке
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

        // Запускаем движение к следующей точке
        MoveToNextWaypoint();
    }

    protected void MoveToNextWaypoint()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            isMoving = true;
        }
    }

    protected void MoveTowardsWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length) return;

        Transform targetPoint = waypoints[currentWaypointIndex].point;
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        animator.SetBool("IsWalking", true);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
            StartCoroutine(HandleWaypointActions());
        }
    }
    protected abstract void CheckIfDefeated();


    public void Teleport(Vector3 destination)
    {
        transform.position = destination;
    }
    private void OnDrawGizmos()
    {
        // Направление взгляда врага (если есть SpriteRenderer)
        float direction = (spriteRenderer != null && spriteRenderer.flipX) ? -1 : 1;
        Vector2 position = transform.position;

        // Цвета для лучей
        Color frontRayColor = Color.red;
        Color backRayColor = Color.blue;
        frontRayColor.a = 0.5f; // Полупрозрачный
        backRayColor.a = 0.5f;

        // Рисуем передний луч
        Gizmos.color = frontRayColor;
        float currentFrontLength = isChasing ? extendedFrontRayLength : frontRayLength;
        Gizmos.DrawWireSphere(position, 0.2f); // Точка начала луча
        Gizmos.DrawLine(position, position + new Vector2(direction * currentFrontLength, 0));
        Gizmos.DrawWireSphere(position + new Vector2(direction * currentFrontLength, 0), 0.2f);

        // Рисуем задний луч
        Gizmos.color = backRayColor;
        Gizmos.DrawLine(position, position + new Vector2(-direction * backRayLength, 0));
        Gizmos.DrawWireSphere(position + new Vector2(-direction * backRayLength, 0), 0.2f);
    }
}