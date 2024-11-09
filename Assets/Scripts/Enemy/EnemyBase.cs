using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> patrolPoints; // Точки патрулирования
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private List<float> waitTimesAtPoints; // Время ожидания на каждой точке
    [SerializeField] protected Animator animator;

    [Header("Detection Settings")]
    [SerializeField] private float rayLengthForward = 8f;
    [SerializeField] private float rayLengthBackward = 5f;
    [SerializeField] private LayerMask playerLayer;

    private int currentPatrolIndex = 0;
    private bool waitingAtPoint = false;
    private bool chasingPlayer = false;
    private Rigidbody2D rb;
    private Transform playerTransform;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("EnemyBase requires a Rigidbody2D component.");
        }
    }

    protected virtual void Start()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning("No patrol points set for enemy.");
            return;
        }

        if (waitTimesAtPoints.Count != patrolPoints.Count)
        {
            Debug.LogError("Number of wait times must match number of patrol points.");
            return;
        }

        StartCoroutine(PatrolRoutine());
        PlayWalkingAnimation(); // Начинаем с анимации ходьбы
    }

    private void Update()
    {
        if (!waitingAtPoint)
        {
            DetectPlayerWithRays();
        }
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (!chasingPlayer && !waitingAtPoint)
            {
                Transform targetPoint = patrolPoints[currentPatrolIndex];
                MoveToTarget(targetPoint.position);

                if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
                {
                    StartCoroutine(HandlePointInteraction(currentPatrolIndex));
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
                }
            }
            yield return null;
        }
    }

    private void MoveToTarget(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }
    }

    private IEnumerator HandlePointInteraction(int pointIndex)
    {
        waitingAtPoint = true;
        rb.velocity = Vector2.zero; // Остановка движения
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; // Заморозка позиции по X
        animator.SetBool("IsWalking", false); // Отключение анимации ходьбы

        animator.SetBool($"Point_{pointIndex + 1}", true); // Включение анимации точки

        float waitTime = waitTimesAtPoints[pointIndex];
        yield return new WaitForSeconds(waitTime);

        animator.SetBool($"Point_{pointIndex + 1}", false); // Выключение анимации точки

        animator.SetBool("IsWalking", true);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Снятие заморозки позиции
        waitingAtPoint = false;
    }

    private void DetectPlayerWithRays()
    {
        Vector2 forwardDirection = transform.right;
        Vector2 backwardDirection = -transform.right;
        RaycastHit2D hitForward = Physics2D.Raycast(transform.position, forwardDirection, rayLengthForward, playerLayer);
        RaycastHit2D hitBackward = Physics2D.Raycast(transform.position, backwardDirection, rayLengthBackward, playerLayer);

        if (hitForward.collider != null)
        {
            chasingPlayer = true;
            playerTransform = hitForward.transform;
            StopAllCoroutines();
            StartCoroutine(ChasePlayer());
        }
        else if (hitBackward.collider != null)
        {
            chasingPlayer = true;
            playerTransform = hitBackward.transform;
            StopAllCoroutines();
            StartCoroutine(ChasePlayer());
        }
        else if (chasingPlayer)
        {
            chasingPlayer = false;
            StartCoroutine(PatrolRoutine());
        }
    }

    private IEnumerator ChasePlayer()
    {
        while (chasingPlayer)
        {
            if (playerTransform != null)
            {
                MoveToTarget(playerTransform.position);
            }

            if (Vector2.Distance(transform.position, playerTransform.position) > rayLengthForward &&
                Vector2.Distance(transform.position, playerTransform.position) > rayLengthBackward)
            {
                chasingPlayer = false;
            }

            yield return null;
        }

        StartCoroutine(PatrolRoutine());
    }

    public void TriggerDeath()
    {
        rb.velocity = Vector2.zero;
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(2f); // Время зависит от длительности анимации смерти
        Destroy(gameObject);
    }

    private void PlayWalkingAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }
    }
}
