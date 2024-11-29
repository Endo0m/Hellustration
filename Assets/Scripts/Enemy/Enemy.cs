using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
 
    [Header("Components")]
    private IMoveable movementController;
    private IAnimatable animationController;
    private IAudible audioController;
    private IPlayerDetector playerDetector;
    private IItemCollector itemCollector;
    [SerializeField] protected ParticleSystem actionParticles;
    [SerializeField] private PulseController pulseController;
    [Header("Detection Settings")]
    [SerializeField] private float frontRayLength = 8f;
    [SerializeField] private float backRayLength = 5f;
    [SerializeField] private float raycastYOffset = 0f;
    [SerializeField] private float captureRadius = 1.5f;
    [SerializeField] private List<string> requiredItems;
    [SerializeField] private HunterModeSettings hunterSettings;

    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private WaypointData[] waypoints;

    [Header("Sound Settings")]
    [SerializeField] private string walkSoundKey = "enemy_walk";
    [SerializeField] private string runSoundKey = "enemy_run";
    [SerializeField] private string detectionSoundKey = "enemy_detect";
    [SerializeField] private string[] breathSoundKey;
    [SerializeField] private string[] stomachSoundKey;
    [SerializeField] private float stepSoundInterval = 0.5f;
    [SerializeField] private float detectionSoundCooldown = 3f;

    [Header("Layer Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask hideLayer;
    private MusicController musicController;
    public int WaypointCount => waypoints.Length;
    // ���������
    private bool isPlayerCaptured = false;
    private bool hasAllItems = false;
    private bool readyToComplete = false;
    private int currentWaypointIndex = 0;
    private int lastKnownWaypointIndex = 0;
    private bool isChasing = false;
    private bool isMoving = true;
    private Transform playerTransform;
    private float lastDetectionSoundTime;
    private float lastStepTime;
    private Vector2 facingDirection = Vector2.right;
    private Coroutine waypointActionCoroutine;

    // ����� �����
    private float hunterModeTimer;
    private float currentHunterModeDuration;
    private bool isHunterMode = false;
    private TeleportZone currentTargetTeleport;
    private bool isMovingToTeleport = false;
    private float lastTeleportTime;

    private void Awake()
    {

        InitializeComponents();
        InitializeHunterMode();
    }

    private void Start()
    {
        musicController = FindObjectOfType<MusicController>();
    }

    public void RoarSound()
    {
        var index = Random.Range(0, breathSoundKey.Length -1);
        var clip = breathSoundKey[index];
        audioController.PlaySound(clip);
    }   
    public void StomatchSound()
    {
        var index = Random.Range(0, stomachSoundKey.Length -1);
        var clip = stomachSoundKey[index];
        audioController.PlaySound(clip);
    } 
    private void InitializeComponents()
    {
        movementController = gameObject.AddComponent<MovementController>();
        animationController = gameObject.AddComponent<AnimationController>();
        audioController = gameObject.AddComponent<AudioController>();
        playerDetector = gameObject.AddComponent<PlayerDetector>();
        itemCollector = gameObject.AddComponent<ItemCollector>();

        audioController.Initialize(new[] { "footsteps", "detection", "waypoint" });
        playerDetector.Initialize(playerLayer, hideLayer, frontRayLength, backRayLength);
        itemCollector.Initialize(requiredItems);
    }

    private void InitializeHunterMode()
    {
        hunterModeTimer = Random.Range(hunterSettings.minTimeUntilHunterMode,
            hunterSettings.maxTimeUntilHunterMode);
    }

    private void Update()
    {
        if (!isPlayerCaptured && !readyToComplete)
        {
            CheckCaptureRadius(); // ��������� �������� ������� � �������� ����
            UpdateHunterMode();
            if (!isHunterMode)
            {
                CheckPlayerDetection();
            }
            UpdateMovement();
            CheckFootsteps();
        }
    }

    private void UpdateHunterMode()
    {
        if (isHunterMode)
        {
            currentHunterModeDuration -= Time.deltaTime;
            if (currentHunterModeDuration <= 0 ||
                (playerTransform != null && playerTransform.gameObject.layer == LayerMask.NameToLayer("Hidden")))
            {
                StopHunterMode();
            }
        }
        else
        {
            hunterModeTimer -= Time.deltaTime;
            if (hunterModeTimer <= 0)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null && player.layer != LayerMask.NameToLayer("Hidden"))
                {
                    StartHunterMode();
                }
                else
                {
                    InitializeHunterMode(); // ���������� ������ ���� ����� �������
                }
            }
        }
    }
    private void HandleHunterMode()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && player.layer != LayerMask.NameToLayer("Hidden"))
            {
                playerTransform = player.transform;
            }
            else
            {
                StopHunterMode();
                return;
            }
        }

        float yDifference = Mathf.Abs(playerTransform.position.y - transform.position.y);
        if (yDifference > hunterSettings.teleportSearchThresholdY)
        {
            if (!isMovingToTeleport)
            {
                SearchForTeleport(playerTransform.position.y > transform.position.y);
                Vector2 horizontalDirection = new Vector2(
                    Mathf.Sign(playerTransform.position.x - transform.position.x),
                    0
                );
                movementController.Move(horizontalDirection, hunterSettings.hunterSpeed);
                UpdateFacingDirection(horizontalDirection.x);
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
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            movementController.Move(direction, hunterSettings.hunterSpeed);
            UpdateFacingDirection(direction.x);
        }
    }

    private void StartHunterMode()
    {
        isHunterMode = true;
        currentHunterModeDuration = hunterSettings.hunterModeDuration;
        lastKnownWaypointIndex = currentWaypointIndex;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;

            // Reset the last detection sound time to ensure sound plays
            lastDetectionSoundTime = 0f;

            // Play detection sound at max volume in 2D
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.spatialBlend = 0f; // 0 = 2D sound
                audioSource.volume = 1f; // Maximum volume
            }
            audioController.PlaySound(detectionSoundKey);

            StopAllCoroutines();

            if (actionParticles != null && actionParticles.isPlaying)
            {
                actionParticles.Stop();
            }
            if (musicController != null)
            {
                musicController.StartHuntMode();
            }
            if (pulseController != null)
            {
                pulseController.Panic();
            }

            animationController.PlayAnimation("IsWalking", false);
            animationController.PlayAnimation("IsActing", false);
            animationController.PlayAnimation("IsChasing", true);
        }
    }

    private void StopHunterMode()
    {
        isHunterMode = false;
        playerTransform = null;
        InitializeHunterMode();

        // Reset the last detection sound time
        lastDetectionSoundTime = 0f;

        if (pulseController != null)
        {
            pulseController.EnemyLost();
        }

        animationController.PlayAnimation("IsChasing", false);
        movementController.Stop();
        if (musicController != null)
        {
            musicController.StopHuntMode();
        }
        currentWaypointIndex = lastKnownWaypointIndex;
        isMoving = true;
    }

    protected void CheckPlayerDetection()
    {
        if (isHunterMode || isPlayerCaptured) return;

        Vector2 position = transform.position + new Vector3(0, raycastYOffset);

        // ��������� ����������� ������ ������ ���� ����� �� �� ���� Hidden
        Transform detectedPlayer = playerDetector.GetDetectedPlayer(position, facingDirection,
            isChasing ? frontRayLength * 2 : frontRayLength,
            isChasing ? backRayLength * 2 : backRayLength);

        if (detectedPlayer != null && detectedPlayer.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            float yDifference = Mathf.Abs(detectedPlayer.position.y - transform.position.y);
            if (yDifference > hunterSettings.teleportSearchThresholdY)
            {
                if (!isMovingToTeleport)
                {
                    SearchForTeleport(detectedPlayer.position.y > transform.position.y);
                }
                else if (currentTargetTeleport != null)
                {
                    MoveToTeleport();
                }
            }
            else
            {
                if (!isChasing)
                {
                    StartChasing(detectedPlayer);
                }
            }
        }
        else if (isChasing)
        {
            StopChasing();
        }
    }
    private void CheckCaptureRadius()
    {
        if (isPlayerCaptured) return;

        Vector2 position = transform.position + new Vector3(0, raycastYOffset);
        Collider2D playerCollider = Physics2D.OverlapCircle(position, captureRadius, playerLayer);

        if (playerCollider != null && playerCollider.CompareTag("Player"))
        {
            // ���������, ��� ����� �� �������
            if (playerCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                CapturePlayer();
            }
        }
    }
    private void SearchForTeleport(bool searchingUp)
    {
        if (Time.time - lastTeleportTime < 2f) return;

        TeleportZone[] teleports = FindObjectsOfType<TeleportZone>();
        float targetY = playerTransform.position.y;

        TeleportZone bestTeleport = null;
        float bestScore = float.MaxValue;

        foreach (var teleport in teleports)
        {
            if (teleport.GetDestination() == null) continue;

            float currentYDiff = Mathf.Abs(transform.position.y - targetY);
            float afterTeleportYDiff = Mathf.Abs(teleport.GetDestination().position.y - targetY);

            // ��������� ����������� ���������
            bool isTeleportUp = teleport.GetDestination().position.y > transform.position.y;
            if (isTeleportUp == searchingUp && afterTeleportYDiff < currentYDiff)
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

    private void MoveToTeleport()
    {
        if (currentTargetTeleport == null) return;

        Vector2 directionToTeleport = (currentTargetTeleport.transform.position - transform.position).normalized;
        float currentSpeed = isHunterMode ? hunterSettings.hunterSpeed :
                            isChasing ? chaseSpeed : patrolSpeed;

        movementController.Move(directionToTeleport, currentSpeed);
        UpdateFacingDirection(directionToTeleport.x);

        if (Vector2.Distance(transform.position, currentTargetTeleport.transform.position) < 0.5f)
        {
            lastTeleportTime = Time.time;
            currentTargetTeleport.Teleport(gameObject);
            isMovingToTeleport = false;
            currentTargetTeleport = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemIdentifier item = other.GetComponent<ItemIdentifier>();
        if (item != null)
        {
            itemCollector.CollectItem(item.ItemName);
            if (itemCollector.HasAllRequiredItems())
            {
                hasAllItems = true;
            }
        }
    }


    private void CapturePlayer()
    {
        if (isPlayerCaptured) return;

        isPlayerCaptured = true;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            var playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                StopAllCoroutines();
                movementController.Stop();
                animationController.PlayAnimation("IsChasing", false);
                animationController.PlayAnimation("IsWalking", false);

                playerController.TriggerDeathSequence();
            }
        }
    }



    protected void CompleteLevel()
    {
        readyToComplete = true;
        StopAllCoroutines();
        if (actionParticles != null && actionParticles.isPlaying)
        {
            actionParticles.Stop();
        }

        Debug.Log("Enemy has collected all items and reached the final waypoint!");
        SceneManager.LoadScene("CutSceneDieButcher"); 
    }

    private void UpdateMovement()
    {
        if (isHunterMode)
        {
            HandleHunterMode();
        }
        else if (isChasing && playerTransform != null)
        {
            HandleChase();
        }
        else if (isMoving)
        {
            HandlePatrol();
        }
    }

    private void HandleChase()
    {
        if (playerTransform == null || playerTransform.gameObject.layer == LayerMask.NameToLayer("Hidden"))
        {
            StopChasing();
            return;
        }

        float yDifference = Mathf.Abs(playerTransform.position.y - transform.position.y);
        if (yDifference > hunterSettings.teleportSearchThresholdY)
        {
            if (!isMovingToTeleport)
            {
                SearchForTeleport(playerTransform.position.y > transform.position.y);
                Vector2 horizontalDirection = new Vector2(
                    Mathf.Sign(playerTransform.position.x - transform.position.x),
                    0
                );
                movementController.Move(horizontalDirection, chaseSpeed);
                UpdateFacingDirection(horizontalDirection.x);
            }
            else if (currentTargetTeleport != null)
            {
                MoveToTeleport();
            }
        }
        else
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            movementController.Move(direction, chaseSpeed);
            animationController.PlayAnimation("IsChasing", true);
            UpdateFacingDirection(direction.x);
        }
    }


    private void HandlePatrol()
    {
        if (currentWaypointIndex >= waypoints.Length) return;

        Transform targetPoint = waypoints[currentWaypointIndex].point;
        float yDifference = Mathf.Abs(targetPoint.position.y - transform.position.y);

        if (yDifference > hunterSettings.teleportSearchThresholdY)
        {
            if (!isMovingToTeleport)
            {
                SearchForTeleportToWaypoint(targetPoint);
                // ��������� ������������� � ����� �������� ���� ���� ��������
                Vector2 horizontalDirection = new Vector2(
                    Mathf.Sign(targetPoint.position.x - transform.position.x),
                    0
                );
                movementController.Move(horizontalDirection, patrolSpeed);
                UpdateFacingDirection(horizontalDirection.x);
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
            movementController.Move(direction, patrolSpeed);
            animationController.PlayAnimation("IsWalking", true);
            UpdateFacingDirection(direction.x);

            if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                if (hasAllItems && currentWaypointIndex == waypoints.Length - 1)
                {
                    CompleteLevel();
                }
                else
                {
                    StartCoroutine(HandleWaypointActions());
                }
            }
        }
    }
    private void SearchForTeleportToWaypoint(Transform targetPoint)
    {
        if (Time.time - lastTeleportTime < 2f) return;

        TeleportZone[] teleports = FindObjectsOfType<TeleportZone>();
        float targetY = targetPoint.position.y;

        TeleportZone bestTeleport = null;
        float bestScore = float.MaxValue;

        foreach (var teleport in teleports)
        {
            if (teleport.GetDestination() == null) continue;

            float currentYDiff = Mathf.Abs(transform.position.y - targetY);
            float afterTeleportYDiff = Mathf.Abs(teleport.GetDestination().position.y - targetY);

            bool isTeleportUp = teleport.GetDestination().position.y > transform.position.y;
            bool shouldGoUp = targetY > transform.position.y;

            if (isTeleportUp == shouldGoUp && afterTeleportYDiff < currentYDiff)
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
            Debug.Log($"Found teleport to reach waypoint: {bestTeleport.name}");
        }
    }

    private void UpdateFacingDirection(float horizontalDirection)
    {
        if (horizontalDirection != 0)
        {
            facingDirection = new Vector2(Mathf.Sign(horizontalDirection), 0);
            animationController.UpdateFacing(horizontalDirection);
        }
    }

    private IEnumerator HandleWaypointActions()
    {
        WaypointData currentData = waypoints[currentWaypointIndex];
        isMoving = false;
        movementController.Stop();
        animationController.PlayAnimation("IsWalking", false);
        animationController.PlayAnimation("IsActing", true);

        // ������������� ���������� ���� �����, ���� �� �����
        if (!string.IsNullOrEmpty(currentData.waypointSoundKey))
        {
            audioController.PlayWaypointSound(currentData.waypointSoundKey, currentData.loopWaypointSound);
        }

        if (currentData.enableParticles && actionParticles != null)
        {
            actionParticles.Play();
        }

        yield return new WaitForSeconds(currentData.waitDuration);

        if (currentData.openDoor)
        {
            Door door = currentData.point.GetComponent<Door>();
            if (door != null)
            {
                door.Interact(gameObject);
            }
        }

        animationController.PlayAnimation("IsActing", false);
        audioController.StopWaypointSound();

        if (currentData.enableParticles && actionParticles != null)
        {
            actionParticles.Stop();
        }

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        isMoving = true;
    }


    private void StartChasing(Transform player)
    {
        if (player == null || isHunterMode) return;

        isChasing = true;
        playerTransform = player;
        lastKnownWaypointIndex = currentWaypointIndex;

        if (Time.time - lastDetectionSoundTime >= detectionSoundCooldown)
        {
            audioController.PlaySound(detectionSoundKey);
            lastDetectionSoundTime = Time.time;
        }

        audioController.StopWaypointSound();

        StopAllCoroutines();
        if (actionParticles != null && actionParticles.isPlaying)
        {
            actionParticles.Stop();
        }

        animationController.PlayAnimation("IsWalking", false);
        animationController.PlayAnimation("IsActing", false);
        animationController.PlayAnimation("IsChasing", true);
    }

    private void StopChasing()
    {
        isChasing = false;
        playerTransform = null;

        animationController.PlayAnimation("IsChasing", false);
        movementController.Stop();

        currentWaypointIndex = lastKnownWaypointIndex;
        isMoving = true;
    }

    private void CheckFootsteps()
    {
        if ((isMoving || isChasing) && Time.time - lastStepTime >= stepSoundInterval)
        {
            string soundKey = isChasing ? runSoundKey : walkSoundKey;
            audioController.PlaySound(soundKey);
            lastStepTime = Time.time;
        }
    }

    public int CurrentWaypoint => currentWaypointIndex;

    private void OnDrawGizmos()
    {
        Vector2 position = transform.position + new Vector3(0, raycastYOffset);

        // ������ ���� �����������
        Gizmos.color = Color.red;
        float currentFrontLength = isChasing ? frontRayLength * 2 : frontRayLength;
        float currentBackLength = isChasing ? backRayLength * 2 : backRayLength;

        Gizmos.DrawRay(position, facingDirection * currentFrontLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(position, -facingDirection * currentBackLength);

        // ������ ������ ������� � ������� ������� � ����������� �� ������
        if (isPlayerCaptured)
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // �������
        else if (isChasing)
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // ���������
        else
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f); // Ƹ����

        // ������ ������ �������
        Gizmos.DrawWireSphere(position, captureRadius);

        // ������������� ������ ����������� ����� � ������� �������������
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.1f);
        Gizmos.DrawSphere(position, captureRadius);
    }
}