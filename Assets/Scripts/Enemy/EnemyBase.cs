using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator animator;
    [SerializeField] protected WaypointData[] waypoints;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float chaseSpeed = 4f;
    [SerializeField] protected float aggressiveChaseSpeed = 6f;
    [SerializeField] protected ParticleSystem actionParticles;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask hideLayer;


    [SerializeField] protected float minTimeUntilAggressive = 180f; // Минимальное время (3 минуты)
    [SerializeField] protected float maxTimeUntilAggressive = 300f; // Максимальное время (5 минут)
    [SerializeField] protected float chaseDuration = 30f; // Длительность преследования
    protected float currentChaseDuration; // Текущее время преследования
    protected bool isChaseTimerActive = false; // Активен ли таймер преследования
    protected float aggressiveTimer;
    protected bool isAggressiveMode = false;
    protected AudioSource audioSource;
    protected SoundManager soundManager;
    [SerializeField] protected float stepSoundInterval = 0.5f; // Интервал между звуками шагов
    protected float lastStepTime;
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
    [SerializeField] private float teleportSearchThresholdY = 3f; // Пороговое значение разницы по Y для поиска телепорта
    [SerializeField] private float teleportReachDistance = 0.5f; // Расстояние, на котором считается, что враг достиг телепорта
    [SerializeField] private float teleportCooldown = 2f; // Кулдаун между использованиями телепортов
    private float lastTeleportTime;
    private TeleportZone currentTargetTeleport;
    private bool isMovingToTeleport = false;
    private Coroutine waypointActionCoroutine;


    public int CurrentWaypoint { get; private set; }

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

        // Добавляем инициализацию звука
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        soundManager = FindObjectOfType<SoundManager>();

        InitializeAggressiveTimer();
        if (waypoints.Length > 0)
        {
            MoveToNextWaypoint();
        }
        audioSource.spatialBlend = 1;
    }

    public void UpdateWaypoint(int waypoint)
    {
        CurrentWaypoint = waypoint;
        Debug.Log($"Waypoint updated to: {CurrentWaypoint}");
    }

    protected virtual void Update()
    {
        if (!isAggressiveMode)
        {
            aggressiveTimer -= Time.deltaTime;
            if (aggressiveTimer <= 0)
            {
                EnableAggressiveMode();
            }
        }
        else if (isChaseTimerActive)
        {
            currentChaseDuration -= Time.deltaTime;
            if (currentChaseDuration <= 0)
            {
                DisableAggressiveMode();
            }
        }

        CheckFootsteps();

        if (isAggressiveMode)
        {
            if (!isChasing)
            {
                FindAndChasePlayer();
            }
            else if (playerTransform != null)
            {
                HandleChasingLogic();
            }
        }
        else
        {
            if (!isChasing && isMoving && currentWaypointIndex < waypoints.Length)
            {
                MoveTowardsWaypoint();
            }
            else if (isChasing && playerTransform != null)
            {
                HandleChasingLogic();
            }
        }

        if (!isAggressiveMode)
        {
            CheckPlayerDetection();
        }
        UpdateFacingDirection();
    }
    protected virtual void InitializeAggressiveTimer()
    {
        aggressiveTimer = Random.Range(minTimeUntilAggressive, maxTimeUntilAggressive);
    }
    protected void CheckFootsteps()
    {
        // Проверяем, движется ли враг
        if ((isMoving || isChasing) && rb.velocity.magnitude > 0.1f)
        {
            // Проверяем, прошло ли достаточно времени с последнего звука
            if (Time.time - lastStepTime >= stepSoundInterval)
            {
                PlayFootstepSound();
                lastStepTime = Time.time;
            }
        }
    }
    protected virtual void PlayFootstepSound()
    {
        if (soundManager != null && audioSource != null)
        {
            // Воспроизводим разные звуки в зависимости от состояния
            string soundKey = isChasing ? "enemy_run" : "enemy_walk";
            soundManager.PlaySound(soundKey, audioSource);
        }
    }
    protected void EnableAggressiveMode()
    {
        // Check if player is available before enabling aggressive mode
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null || player.layer != LayerMask.NameToLayer("Player"))
        {
            // Player not found or is hidden, reset aggressive timer
            InitializeAggressiveTimer();
            return;
        }

        isAggressiveMode = true;
        isMoving = false;

        if (waypointActionCoroutine != null)
        {
            StopCoroutine(waypointActionCoroutine);
            waypointActionCoroutine = null;

            // Stop particles if they are playing
            if (actionParticles != null && actionParticles.isPlaying)
            {
                actionParticles.Stop();
            }

            // Reset animation parameters
            animator.SetBool("IsActing", false);

            // Increment the waypoint index
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        animator.SetBool("IsChasing", true);
        animator.SetBool("IsWalking", false);

        currentChaseDuration = chaseDuration;
        isChaseTimerActive = true;
    }


    protected void DisableAggressiveMode()
    {
        isAggressiveMode = false;
        isChaseTimerActive = false;
        StopChasing();

        // Start a new aggressive timer
        InitializeAggressiveTimer();

        // Resume patrolling
        MoveToNextWaypoint();
    }


    protected void FindAndChasePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.layer == LayerMask.NameToLayer("Player"))
        {
            StartChasing(player.transform);
        }
        else
        {
            // Player not found or is hidden, exit aggressive mode
            if (isAggressiveMode)
            {
                DisableAggressiveMode();
            }
        }
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
        float direction = spriteRenderer.flipX ? -1 : 1;
        Vector2 position = transform.position;

        Debug.DrawRay(position, Vector2.right * direction * (isChasing ? extendedFrontRayLength : frontRayLength), Color.red);
        Debug.DrawRay(position, Vector2.right * -direction * backRayLength, Color.blue);

        RaycastHit2D frontHit = Physics2D.Raycast(position, Vector2.right * direction,
            isChasing ? extendedFrontRayLength : frontRayLength, playerLayer | hideLayer);

        RaycastHit2D backHit = Physics2D.Raycast(position, Vector2.right * -direction,
            backRayLength, playerLayer | hideLayer);

        HandlePlayerDetection(frontHit, backHit);
    }

    protected void HandlePlayerDetection(RaycastHit2D frontHit, RaycastHit2D backHit)
    {
        if (!isChasing)
        {
            if ((frontHit.collider != null || backHit.collider != null) &&
                ((frontHit.collider?.gameObject.layer == LayerMask.NameToLayer("Player")) ||
                 (backHit.collider?.gameObject.layer == LayerMask.NameToLayer("Player"))))
            {
                StartChasing(frontHit.collider != null ? frontHit.collider.transform : backHit.collider.transform);
            }
        }
        else
        {
            bool playerVisible = false;
            bool playerHidden = false;

            if (frontHit.collider != null)
            {
                if (frontHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                    playerVisible = true;
                else if (frontHit.collider.gameObject.layer == LayerMask.NameToLayer("Hidden"))
                    playerHidden = true;
            }

            if (backHit.collider != null)
            {
                if (backHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                    playerVisible = true;
                else if (backHit.collider.gameObject.layer == LayerMask.NameToLayer("Hidden"))
                    playerHidden = true;
            }

            if (!playerVisible || playerHidden)
            {
                if (isAggressiveMode)
                {
                    DisableAggressiveMode();
                }
                else
                {
                    StopChasing();
                }
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

            animator.SetBool("IsWalking", false);
            animator.SetBool("IsChasing", true);

            if (!isAggressiveMode)
            {
                //StopAllCoroutines();
            }
        }
    }

    protected void HandleChasingLogic()
    {

        if (playerTransform != null)
        {
            if (playerTransform.gameObject.layer == LayerMask.NameToLayer("Hidden"))
            {
                // Player is hidden, exit aggressive mode
                if (isAggressiveMode)
                {
                    DisableAggressiveMode();
                }
                else
                {
                    StopChasing();
                }
                return;
            }
            float yDifference = Mathf.Abs(playerTransform.position.y - transform.position.y);

            if (yDifference > teleportSearchThresholdY)
            {
                if (!isMovingToTeleport)
                {
                    SearchForTeleport();
                }
                else if (currentTargetTeleport != null)
                {
                    MoveToTeleport();
                }
            }
            else
            {
                isMovingToTeleport = false;
                currentTargetTeleport = null;
                ChasePlayer();
            }
        }
    }

    protected void SearchForTeleport()
    {
        if (Time.time - lastTeleportTime < teleportCooldown) return;

        TeleportZone[] teleports = FindObjectsOfType<TeleportZone>();
        float playerY = playerTransform.position.y;

        TeleportZone bestTeleport = null;
        float bestScore = float.MaxValue;

        foreach (var teleport in teleports)
        {
            if (teleport.GetDestination() == null) continue;

            // Проверяем, приближает ли нас телепорт к игроку по Y
            float currentYDiff = Mathf.Abs(transform.position.y - playerY);
            float afterTeleportYDiff = Mathf.Abs(teleport.GetDestination().position.y - playerY);

            if (afterTeleportYDiff < currentYDiff)
            {
                float distanceToTeleport = Vector2.Distance(transform.position, teleport.transform.position);
                float score = distanceToTeleport + afterTeleportYDiff;

                if (score < bestScore)
                {
                    bestScore = score;
                    bestTeleport = teleport;
                }
            }
        }

        if (bestTeleport != null)
        {
            currentTargetTeleport = bestTeleport;
            isMovingToTeleport = true;
        }
    }

    protected void MoveToTeleport()
    {
        if (currentTargetTeleport == null) return;

        Vector2 directionToTeleport = (currentTargetTeleport.transform.position - transform.position).normalized;
        rb.velocity = directionToTeleport * moveSpeed;

        // Проверяем, достигли ли телепорта
        if (Vector2.Distance(transform.position, currentTargetTeleport.transform.position) < teleportReachDistance)
        {
            lastTeleportTime = Time.time;
            currentTargetTeleport.Teleport(gameObject);
            isMovingToTeleport = false;
            currentTargetTeleport = null;
        }
    }

    protected void ChasePlayer()
    {
        if (playerTransform != null)
        {
            Vector2 directionToPlayer = playerTransform.position - transform.position;
            // Ограничиваем движение по Y, если не используем телепорт
            if (!isMovingToTeleport && Mathf.Abs(directionToPlayer.y) > teleportSearchThresholdY)
            {
                directionToPlayer.y = 0;
            }

            rb.velocity = directionToPlayer.normalized * (isAggressiveMode ? aggressiveChaseSpeed : chaseSpeed);
        }
    }

    protected void MoveToPosition(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        animator.SetBool("IsWalking", true);
    }



    protected void StopChasing()
    {
        if (isChasing && !isAggressiveMode)
        {
            isChasing = false;
            playerTransform = null;

            animator.SetBool("IsChasing", false);
            animator.SetBool("IsWalking", false);

            rb.velocity = Vector2.zero;

            currentWaypointIndex = lastKnownWaypointIndex;

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

        currentWaypointIndex = lastKnownWaypointIndex;

        WaypointData currentData = waypoints[currentWaypointIndex];
        if (Vector2.Distance(transform.position, currentData.point.position) < 0.1f)
        {
            StartCoroutine(HandleWaypointActions());
        }
        else
        {
            isMoving = true;
        }
    }

    protected IEnumerator HandleWaypointActions()
    {
        if (isAggressiveMode) yield break;

        isMoving = false;
        WaypointData currentData = waypoints[currentWaypointIndex];

        if (currentData.enableParticles && actionParticles != null)
        {
            actionParticles.Play();
        }

        animator.SetBool("IsActing", true);
        //yield return new WaitForSeconds(currentData.waitDuration);
        yield return new WaitForSeconds(currentData.waitDuration);
        while (isAggressiveMode || isChasing)
        {
            yield return new WaitForSeconds(0.1f);
        }
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

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

        MoveToNextWaypoint();
    }

    protected void MoveToNextWaypoint()
    {
        if (!isAggressiveMode && currentWaypointIndex < waypoints.Length)
        {
            isMoving = true;
        }
    }

    protected void MoveTowardsWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length) return;

        Transform targetPoint = waypoints[currentWaypointIndex].point;
        float yDifference = Mathf.Abs(targetPoint.position.y - transform.position.y);

        if (yDifference > teleportSearchThresholdY)
        {
            if (!isMovingToTeleport)
            {
                SearchForTeleportToWaypoint(targetPoint.position);
            }
            else if (currentTargetTeleport != null)
            {
                MoveToTeleport();
            }
        }
        else
        {
            isMovingToTeleport = false;
            currentTargetTeleport = null;

            Vector2 direction = (targetPoint.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
            animator.SetBool("IsWalking", true);

            if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                rb.velocity = Vector2.zero;
                animator.SetBool("IsWalking", false);
                UpdateWaypoint(currentWaypointIndex);
                waypointActionCoroutine = StartCoroutine(HandleWaypointActions());
            }

        }
    }

    protected void SearchForTeleportToWaypoint(Vector3 targetPosition)
    {
        if (Time.time - lastTeleportTime < teleportCooldown) return;

        TeleportZone[] teleports = FindObjectsOfType<TeleportZone>();
        float targetY = targetPosition.y;

        TeleportZone bestTeleport = null;
        float bestScore = float.MaxValue;

        foreach (var teleport in teleports)
        {
            if (teleport.GetDestination() == null) continue;

            float currentYDiff = Mathf.Abs(transform.position.y - targetY);
            float afterTeleportYDiff = Mathf.Abs(teleport.GetDestination().position.y - targetY);

            if (afterTeleportYDiff < currentYDiff)
            {
                float distanceToTeleport = Vector2.Distance(transform.position, teleport.transform.position);
                float score = distanceToTeleport + afterTeleportYDiff;

                if (score < bestScore)
                {
                    bestScore = score;
                    bestTeleport = teleport;
                }
            }
        }

        if (bestTeleport != null)
        {
            currentTargetTeleport = bestTeleport;
            isMovingToTeleport = true;
        }
    }

    protected abstract void CheckIfDefeated();

    public void Teleport(Vector3 destination)
    {
        transform.position = destination;
    }

    private void OnDrawGizmos()
    {
        float direction = (spriteRenderer != null && spriteRenderer.flipX) ? -1 : 1;
        Vector2 position = transform.position;

        Color frontRayColor = Color.red;
        Color backRayColor = Color.blue;
        frontRayColor.a = 0.5f;
        backRayColor.a = 0.5f;

        Gizmos.color = frontRayColor;
        float currentFrontLength = isChasing ? extendedFrontRayLength : frontRayLength;
        Gizmos.DrawWireSphere(position, 0.2f);
        Gizmos.DrawLine(position, position + new Vector2(direction * currentFrontLength, 0));
        Gizmos.DrawWireSphere(position + new Vector2(direction * currentFrontLength, 0), 0.2f);

        Gizmos.color = backRayColor;
        Gizmos.DrawLine(position, position + new Vector2(-direction * backRayLength, 0));
        Gizmos.DrawWireSphere(position + new Vector2(-direction * backRayLength, 0), 0.2f);
    }
}