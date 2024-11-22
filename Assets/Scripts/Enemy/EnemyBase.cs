using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    // ������ � ��������
    protected Rigidbody2D rb;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    // ��������� ��������
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float chaseSpeed = 4f;
    [SerializeField] protected float aggressiveChaseSpeed = 6f;

    // ������� ����� ��������
    [System.Serializable]
    public struct WaypointData
    {
        public Transform point;
        public bool enableParticles;
        public bool openDoor;
        public float waitDuration;
        public string waypointSoundKey; // ���� ����� ��� �����
        public bool loopWaypointSound; // ����������� �� ���� �� �����
    }
    [SerializeField] protected WaypointData[] waypoints;

    // �������
    [SerializeField] protected ParticleSystem actionParticles;

    // ����
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask hideLayer;

    // ������� ������������ ������
    [SerializeField] protected float minTimeUntilAggressive = 180f;
    [SerializeField] protected float maxTimeUntilAggressive = 300f;
    [SerializeField] protected float chaseDuration = 30f;
    protected float currentChaseDuration;
    protected bool isChaseTimerActive = false;
    protected float aggressiveTimer;
    protected bool isAggressiveMode = false;

    // ������� ������
    [Header("Sound Settings")]
    protected Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();
    protected SoundManager soundManager;
    [SerializeField] protected string walkSoundKey = "enemy_walk";
    [SerializeField] protected string runSoundKey = "enemy_run";
    [SerializeField] protected float stepSoundInterval = 0.5f;
    [SerializeField] protected string detectionSoundKey = "enemy_detect";
    [SerializeField] protected float detectionSoundCooldown = 3f;
    [SerializeField] protected string aggressiveSoundKey = "enemy_aggressive";
    protected float lastStepTime;
    protected float lastDetectionSoundTime;
    private AudioSource currentLoopingSource;

    // Raycast ���������
    [SerializeField] protected float backRayLength = 5f;
    [SerializeField] protected float frontRayLength = 8f;
    [SerializeField] protected float extendedFrontRayLength = 12f;
    [SerializeField] private float raycastYOffset = 0f;

    // ��������� � �������
    protected int lastKnownWaypointIndex = 0;
    protected int currentWaypointIndex = 0;
    protected bool isMoving = true;
    protected bool isChasing = false;
    protected Vector3 lastWaypointPosition;
    protected Transform playerTransform;
    protected Vector2 facingDirection;
    public int CurrentWaypoint { get; private set; }

    // �������� ���������
    [SerializeField] private float teleportSearchThresholdY = 3f;
    [SerializeField] private float teleportReachDistance = 0.5f;
    [SerializeField] private float teleportCooldown = 2f;
    private float lastTeleportTime;
    private TeleportZone currentTargetTeleport;
    private bool isMovingToTeleport = false;
    private Coroutine waypointActionCoroutine;

    protected virtual void Start()
    {
        // ������� ������������� �����������
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        soundManager = FindObjectOfType<SoundManager>();

        // ������������� ������� ������
        InitializeAudioSources();

        InitializeAggressiveTimer();
        if (waypoints.Length > 0)
        {
            MoveToNextWaypoint();
        }
    }

    private void InitializeAudioSources()
    {
        CreateAudioSource("footsteps", false);
        CreateAudioSource("detection", false);
        CreateAudioSource("aggressive", false);
        CreateAudioSource("waypoint", true);
    }

    private void CreateAudioSource(string key, bool isLooping)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.maxDistance = 40f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.loop = isLooping;
        audioSources[key] = source;
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
        CheckPlayerDetection();

        if (isChasing && playerTransform != null)
        {
            HandleChasingLogic();
        }
        else if (!isChasing && isMoving && currentWaypointIndex < waypoints.Length && !isAggressiveMode)
        {
            MoveTowardsWaypoint();
        }

        UpdateFacingDirection();
    }

    protected void CheckFootsteps()
    {
        if ((isMoving || isChasing) && rb.velocity.magnitude > 0.1f)
        {
            if (Time.time - lastStepTime >= stepSoundInterval)
            {
                PlayFootstepSound();
                lastStepTime = Time.time;
            }
        }
    }

    protected virtual void PlayFootstepSound()
    {
        string soundKey = isChasing ? runSoundKey : walkSoundKey;
        PlaySound("footsteps", soundKey, false);
    }

    protected void PlaySound(string sourceKey, string soundKey, bool loop)
    {
        if (string.IsNullOrEmpty(soundKey)) return;

        if (audioSources.TryGetValue(sourceKey, out AudioSource source))
        {
            source.loop = loop;
            if (soundManager != null)
            {
                if (loop)
                {
                    AudioClip clip = soundManager.GetAudioClip(soundKey);
                    if (clip != null)
                    {
                        source.clip = clip;
                        source.Play();
                    }
                }
                else
                {
                    soundManager.PlaySound(soundKey, source);
                }
            }
        }
    }

    private void PlayWaypointSound(string soundKey, bool loop)
    {
        if (string.IsNullOrEmpty(soundKey)) return;

        StopWaypointSound();

        if (audioSources.TryGetValue("waypoint", out AudioSource source))
        {
            source.loop = loop;
            if (soundManager != null)
            {
                AudioClip clip = soundManager.GetAudioClip(soundKey);
                if (clip != null)
                {
                    source.clip = clip;
                    source.Play();
                    currentLoopingSource = source;
                }
            }
        }
    }

    private void StopWaypointSound()
    {
        if (currentLoopingSource != null && currentLoopingSource.isPlaying)
        {
            currentLoopingSource.Stop();
            currentLoopingSource = null;
        }
    }

    private void PlayDetectionSound()
    {
        if (Time.time - lastDetectionSoundTime >= detectionSoundCooldown)
        {
            PlaySound("detection", detectionSoundKey, false);
            lastDetectionSoundTime = Time.time;
        }
    }


    public void UpdateWaypoint(int waypoint)
    {
        CurrentWaypoint = waypoint;
        Debug.Log($"Waypoint updated to: {CurrentWaypoint}");
    }

    protected virtual void InitializeAggressiveTimer()
    {
        aggressiveTimer = Random.Range(minTimeUntilAggressive, maxTimeUntilAggressive);
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
        Vector2 position = transform.position + new Vector3(0, raycastYOffset, 0);

        float frontLength = isChasing ? extendedFrontRayLength : frontRayLength;

        // ������ ���� ��� �������
        Debug.DrawRay(position, Vector2.right * direction * frontLength, Color.red);
        Debug.DrawRay(position, Vector2.right * -direction * backRayLength, Color.blue);

        bool playerDetected = false;
        Transform detectedPlayer = null;

        // ��������� ������� ������ �������
        RaycastHit2D[] frontHits = Physics2D.RaycastAll(position, Vector2.right * direction, frontLength);
        foreach (RaycastHit2D hit in frontHits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                GameObject playerObj = hit.collider.gameObject;
                if (playerObj.layer == LayerMask.NameToLayer("Player"))
                {
                    // ����� �����
                    playerDetected = true;
                    detectedPlayer = hit.collider.transform;
                    break;
                }
                else if (playerObj.layer == LayerMask.NameToLayer("Hidden"))
                {
                    // ����� ���������, ���� �� ����� ���
                    playerDetected = false;
                    break;
                }
            }
        }

        // ���� ����� �� ��������� �������, ��������� �����
        if (!playerDetected)
        {
            RaycastHit2D[] backHits = Physics2D.RaycastAll(position, Vector2.right * -direction, backRayLength);
            foreach (RaycastHit2D hit in backHits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    GameObject playerObj = hit.collider.gameObject;
                    if (playerObj.layer == LayerMask.NameToLayer("Player"))
                    {
                        playerDetected = true;
                        detectedPlayer = hit.collider.transform;
                        break;
                    }
                    else if (playerObj.layer == LayerMask.NameToLayer("Hidden"))
                    {
                        playerDetected = false;
                        break;
                    }
                }
            }
        }

        // ��������� ����������� �����������
        if (playerDetected && detectedPlayer != null)
        {
            playerTransform = detectedPlayer;
            if (!isChasing)
            {
                StartChasing(playerTransform);
            }
        }
        else if (isChasing && !isAggressiveMode)
        {
            StopChasing();
        }
    }


    protected void HandlePlayerDetection(RaycastHit2D frontHit, RaycastHit2D backHit)
    {
        bool playerVisible = false;
        bool playerHidden = false;
        Transform detectedPlayerTransform = null;

        if (frontHit.collider != null)
        {
            if (frontHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                playerVisible = true;
                detectedPlayerTransform = frontHit.collider.transform;
            }
            else if (frontHit.collider.gameObject.layer == LayerMask.NameToLayer("Hidden"))
            {
                playerHidden = true;
            }
        }

        if (backHit.collider != null)
        {
            if (backHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                playerVisible = true;
                detectedPlayerTransform = backHit.collider.transform;
            }
            else if (backHit.collider.gameObject.layer == LayerMask.NameToLayer("Hidden"))
            {
                playerHidden = true;
            }
        }

        if (playerVisible && !playerHidden)
        {
            // Update playerTransform
            playerTransform = detectedPlayerTransform;

            if (!isChasing)
            {
                StartChasing(playerTransform);
            }
        }
        else
        {
            // Player is not visible or is hidden
            if (isChasing)
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

  


    protected void HandleChasingLogic()
    {
        if (playerTransform == null) return;

        // ���������, �� ��������� �� �����
        if (playerTransform.gameObject.layer == LayerMask.NameToLayer("Hidden"))
        {
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

            // ���������, ���������� �� ��� �������� � ������ �� Y
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

        // ���������, �������� �� ���������
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
            // ������������ �������� �� Y, ���� �� ���������� ��������
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

        // ������������� ���� �����, ���� �� �����
        if (!string.IsNullOrEmpty(currentData.waypointSoundKey))
        {
            PlayWaypointSound(currentData.waypointSoundKey, currentData.loopWaypointSound);
        }

        if (currentData.enableParticles && actionParticles != null)
        {
            actionParticles.Play();
        }

        animator.SetBool("IsActing", true);
        yield return new WaitForSeconds(currentData.waitDuration);
        while (isAggressiveMode || isChasing)
        {
            yield return new WaitForSeconds(0.1f);
        }
        animator.SetBool("IsActing", false);

        StopWaypointSound();

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

    protected void StartChasing(Transform player)
    {
        if (player == null) return;

        isChasing = true;
        playerTransform = player;
        PlayDetectionSound();
        StopWaypointSound();

        if (!isAggressiveMode)
        {
            lastWaypointPosition = transform.position;
            lastKnownWaypointIndex = currentWaypointIndex;
        }

        if (waypointActionCoroutine != null)
        {
            StopCoroutine(waypointActionCoroutine);
            waypointActionCoroutine = null;

            if (actionParticles != null && actionParticles.isPlaying)
            {
                actionParticles.Stop();
            }
        }

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsChasing", true);
        animator.SetBool("IsActing", false);
    }

    protected void StopChasing()
    {
        isChasing = false;
        playerTransform = null;

        animator.SetBool("IsChasing", false);
        animator.SetBool("IsWalking", false);

        rb.velocity = Vector2.zero;

        if (!isAggressiveMode)
        {
            currentWaypointIndex = lastKnownWaypointIndex;
            MoveToNextWaypoint();
        }
    }

    private void OnDestroy()
    {
        StopWaypointSound();
        foreach (var source in audioSources.Values)
        {
            if (source != null)
            {
                source.Stop();
            }
        }
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