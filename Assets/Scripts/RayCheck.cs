using UnityEngine;

public class RayCheck : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer; // ���� ������
    [SerializeField] private LayerMask teleportLayer; // ���� ����������
    [SerializeField] private float rayDistance = 10f; // ����� ����
    [SerializeField] private Color rayColor = Color.red; // ���� ����
    [SerializeField] private PlayerPulseUI playerPulseUI; // ������ �� ��������� UI
    [SerializeField] private float checkInterval = 1f; // �������� �������� � ��������
    [SerializeField] private PulseController pulseController;

    private bool isFacingRight = true; // ����������� �������� ����������� ������
    private float nextCheckTime; // ����� ��������� ��������
    private bool enemyDetected = false; // ���� ����������� �����
    private PlayerController playerController; // ������ �� ���������� ������

    private void Start()
    {
        playerController = GetComponent<PlayerController>(); // �������� ������ �� ���������� ������
    }

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
            pulseController?.StartIncreasingPulse();
        }
        else if (!enemyFound && enemyDetected)
        {
            enemyDetected = false;
            playerPulseUI?.StopPulse(); // ������������� ���������
            pulseController?.StopIncreasingPulse();
        }
    }

    private bool CastRayWithTeleportCheck(Vector2 origin, Vector2 direction)
    {
        // ���������, �� ����� �� �����
        if (playerController.IsHidden)
        {
            Debug.Log("����� �����, �� ����� ��� ������.");
            return false; // ����� �����, � ��� �� ����� ����� ���
        }

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance, enemyLayer | teleportLayer);
        Debug.DrawRay(origin, direction * rayDistance, rayColor); // ������������ ����

        if (hit.collider != null)
        {
            Debug.Log("��� ��������� � ��������: " + hit.collider.name); // �����, ���������� ��� �������, � ������� ��������� ���

            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("���� ���������!");
                return true; // ���� ������
            }
            else if (hit.collider.CompareTag("TeleportZone"))
            {
                Debug.Log("����������� ���� ����������.");
                TeleportZone teleport = hit.collider.GetComponent<TeleportZone>();
                if (teleport != null)
                {
                    Vector2 newOrigin = teleport.GetDestination().position;
                    return CastRayWithTeleportCheck(newOrigin, direction);
                }
            }
        }
        else
        {
            Debug.Log("��� �� �������� ������� ��������.");
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

    // ����������� ���� � ���������� � ��������� ����� Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = rayColor;
        Gizmos.DrawLine(transform.position, transform.position + (isFacingRight ? Vector3.right : Vector3.left) * rayDistance); // ������ ���

        // �����������, ����� �������� �������������� ����������
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (isFacingRight ? Vector3.right : Vector3.left) * rayDistance, 0.2f); // ����� ����
    }
}
