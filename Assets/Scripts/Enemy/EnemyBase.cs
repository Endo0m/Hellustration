using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float walkSpeed = 2f;
    [SerializeField] protected float runSpeed = 4f;
    [SerializeField] protected Transform[] patrolPoints;
    [SerializeField] protected float stopDuration = 2f;

    [Header("Detection Settings")]
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected float detectionRayLength = 5f;
    [SerializeField] protected Transform detectionOrigin;

    [Header("Pathfinding Settings")]
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float groundCheckDistance = 1f;
    [SerializeField] protected LayerMask teleportLayer;
    [SerializeField] protected float levelDifferenceThreshold = 5f; // Порог по оси Y для поиска телепорта

    protected Animator animator;
    protected Rigidbody2D rb;
    protected int currentPatrolIndex = 0;
    protected bool isChasing = false;
    protected bool isPlayingAnimation = false;
    protected bool isMovingRight = true;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(PatrolRoutine());
    }

    protected virtual void Update()
    {
        HandleDetection();
        if (isChasing)
        {
            StopAllCoroutines();
            StartCoroutine(ChasePlayer());
        }
    }

    protected virtual IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (!isPlayingAnimation && !isChasing)
            {
                Transform targetPoint = patrolPoints[currentPatrolIndex];
                Vector2 targetPosition = targetPoint.position;

                if (Mathf.Abs(transform.position.y - targetPosition.y) > levelDifferenceThreshold)
                {
                    UseTeleportToReachTarget(targetPosition);
                }
                else
                {
                    while (Vector2.Distance(transform.position, targetPosition) > 0.1f && !isChasing)
                    {
                        MoveTowards(targetPosition, walkSpeed);
                        yield return null;
                    }

                    StopMovement();
                    if (!isChasing)
                    {
                        StartCoroutine(PlayAnimationAtPoint(targetPoint));
                        yield return new WaitUntil(() => !isPlayingAnimation);
                    }

                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                }
            }
            yield return null;
        }
    }
    protected virtual IEnumerator PlayAnimationAtPoint(Transform targetPoint)
    {
        isPlayingAnimation = true;
        PatrolPoint patrolPoint = targetPoint.GetComponent<PatrolPoint>();

        if (patrolPoint != null)
        {
            float animationDuration = patrolPoint.AnimationDuration;
            float elapsedTime = 0f;

            animator.SetBool("IsPlaying", true); // Предполагается, что в вашем Animator есть параметр "IsPlaying"
            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            animator.SetBool("IsPlaying", false);
        }

        isPlayingAnimation = false;
    }

    protected virtual void UseTeleportToReachTarget(Vector2 targetPosition)
    {
        Collider2D[] teleports = Physics2D.OverlapCircleAll(transform.position, detectionRayLength, teleportLayer);
        Transform bestTeleport = null;
        float closestDistance = Mathf.Infinity;

        foreach (var teleport in teleports)
        {
            TeleportZone teleportZone = teleport.GetComponent<TeleportZone>();
            if (teleportZone != null)
            {
                float distanceToTarget = Vector2.Distance(teleportZone.GetDestination().position, targetPosition);
                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    bestTeleport = teleportZone.transform;
                }
            }
        }

        if (bestTeleport != null)
        {
            transform.position = bestTeleport.position;
        }
    }

    protected virtual IEnumerator ChasePlayer()
    {
        while (isChasing)
        {
            Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRayLength, playerLayer);
            if (player != null)
            {
                MoveTowards(player.transform.position, runSpeed);
            }
            else
            {
                isChasing = false;
                StartCoroutine(PatrolRoutine());
            }
            yield return null;
        }
    }

    protected virtual void MoveTowards(Vector2 targetPosition, float speed)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

        if (direction.x > 0 && !isMovingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && isMovingRight)
        {
            Flip();
        }
    }

    protected virtual void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    protected virtual void HandleDetection()
    {
        Vector2 forwardDirection = isMovingRight ? Vector2.right : Vector2.left;
        Vector2 backwardDirection = isMovingRight ? Vector2.left : Vector2.right;

        RaycastHit2D hitForward = Physics2D.Raycast(detectionOrigin.position, forwardDirection, detectionRayLength, playerLayer);
        RaycastHit2D hitBackward = Physics2D.Raycast(detectionOrigin.position, backwardDirection, detectionRayLength, playerLayer);

        Debug.DrawRay(detectionOrigin.position, forwardDirection * detectionRayLength, Color.red);
        Debug.DrawRay(detectionOrigin.position, backwardDirection * detectionRayLength, Color.blue);

        if ((hitForward.collider != null && hitForward.collider.CompareTag("Player")) ||
            (hitBackward.collider != null && hitBackward.collider.CompareTag("Player")))
        {
            isChasing = true;
        }
    }

    protected virtual void Flip()
    {
        isMovingRight = !isMovingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (detectionOrigin != null)
        {
            Gizmos.color = Color.red;
            Vector3 forwardDirection = isMovingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(detectionOrigin.position, detectionOrigin.position + forwardDirection * detectionRayLength);

            Gizmos.color = Color.blue;
            Vector3 backwardDirection = isMovingRight ? Vector3.left : Vector3.right;
            Gizmos.DrawLine(detectionOrigin.position, detectionOrigin.position + backwardDirection * detectionRayLength);
        }
    }
}
