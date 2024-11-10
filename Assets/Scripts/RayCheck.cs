using UnityEngine;

public class RayCheck : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer; // ���� ������
    [SerializeField] private LayerMask teleportLayer; // ���� ����������
    [SerializeField] private float rayDistance = 10f; // ����� ����
    [SerializeField] private Color rayColor = Color.red; // ���� ����
    [SerializeField] private PlayerPulseUI playerPulseUI; // ������ �� ��������� UI
    [SerializeField] private float checkInterval = 1f; // �������� �������� � ��������

    private bool isFacingRight = true; // ����������� �������� ����������� ������
    private float nextCheckTime; // ����� ��������� ��������
    private bool enemyDetected = false; // ���� ����������� �����

    private void Update()
    {
        // ��������� ����������� ������
        HandlePlayerFlip();

        // ��������� ��������, ���� ������ ������ �����
        if (Time.time >= nextCheckTime)
        {
            PerformRayCheck();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void PerformRayCheck()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        bool enemyFound = CastRayWithTeleportCheck(transform.position, direction);

        // ��������� ���������� UI � ����������� �� ������� �����
        if (enemyFound && !enemyDetected)
        {
            enemyDetected = true;
            playerPulseUI?.StartPulse(); // ��������� ���������
        }
        else if (!enemyFound && enemyDetected)
        {
            enemyDetected = false;
            playerPulseUI?.StopPulse(); // ������������� ���������
        }
    }

    private bool CastRayWithTeleportCheck(Vector2 origin, Vector2 direction)
    {
        

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance, enemyLayer | teleportLayer);
        Debug.DrawRay(origin, direction * rayDistance, rayColor); // ������������ ����

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector2.Distance(origin, hit.point);
                return true; // ���� ������
            }
            else if (hit.collider.CompareTag("TeleportZone"))
            {
                TeleportZone teleport = hit.collider.GetComponent<TeleportZone>();
                if (teleport != null)
                {
                    Vector2 newOrigin = teleport.GetDestination().position;
                    return CastRayWithTeleportCheck(newOrigin, direction);
                }
            }
        }
        return false; // ���� �� ������
    }

    private void HandlePlayerFlip()
    {
        // ��������� ����������� ������ �� scale (��������, ��� ����� �� ��� X)
        if (transform.localScale.x > 0)
        {
            isFacingRight = true;
        }
        else if (transform.localScale.x < 0)
        {
            isFacingRight = false;
        }
    }
}
