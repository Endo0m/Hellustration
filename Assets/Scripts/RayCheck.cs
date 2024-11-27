using UnityEngine;

public class RayCheck : MonoBehaviour
{
    [Header("Настройки лучей")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask teleportLayer;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private Color rayColor = Color.red;
    [SerializeField] private PulseController pulseController;
    [SerializeField] private float checkInterval = 0.1f;

    private bool isFacingRight = true;
    private float nextCheckTime;
    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        HandlePlayerFlip();
        if (Time.time >= nextCheckTime)
        {
            CheckEnemyPresence();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void CheckEnemyPresence()
    {
        if (playerController.IsHidden) return;

        Vector2 frontDir = isFacingRight ? Vector2.right : Vector2.left;
        Vector2 backDir = -frontDir;

        bool enemyInSight = CheckDirectionForEnemy(transform.position, frontDir) ||
                           CheckDirectionForEnemy(transform.position, backDir);

        if (enemyInSight)
        {
            pulseController.EnemyDetected();
        }
        else
        {
            pulseController.EnemyLost();
        }
    }

    private bool CheckDirectionForEnemy(Vector2 origin, Vector2 direction)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, rayDistance, enemyLayer | teleportLayer);
        Debug.DrawRay(origin, direction * rayDistance, rayColor);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                return true;
            }
            else if (hit.collider.CompareTag("TeleportZone"))
            {
                TeleportZone teleport = hit.collider.GetComponent<TeleportZone>();
                if (teleport != null)
                {
                    if (CheckDirectionForEnemy(teleport.GetDestination().position, direction))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        // Отрисовка лучей в редакторе
        if (Application.isPlaying) return;

        Gizmos.color = rayColor;
        Vector3 pos = transform.position;
        Gizmos.DrawLine(pos, pos + Vector3.right * rayDistance);
        Gizmos.DrawLine(pos, pos + Vector3.left * rayDistance);
    }

    private void HandlePlayerFlip()
    {
        isFacingRight = transform.localScale.x > 0;
    }
}