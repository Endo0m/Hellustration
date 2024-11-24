using UnityEngine;

public class RayCheck : MonoBehaviour
{
    [Header("���� ��� ����������� ������")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("���� ��� ����������� ��� ������������")]
    [SerializeField] private LayerMask teleportLayer;

    [Header("��������� ����")]
    [SerializeField] private float rayDistance = 10f;

    [Header("���� ���� � ���������")]
    [SerializeField] private Color rayColor = Color.red;

    [Header("������ �� UI ��������� ������ ������")]
    [SerializeField] private PlayerPulseUI playerPulseUI;

    [Header("�������� �������� ����")]
    [SerializeField] private float checkInterval = 1f;

    [Header("���������� ������")]
    

    // ���� ����������� ��������� (������/�����)
    private bool isFacingRight = true;
    // ����� ��������� ��������
    private float nextCheckTime;
    // ���� ����������� �����
    private bool enemyDetected = false;
    // ������ �� ���������� ������
    private PlayerController playerController;

    private void Start()
    {
        // �������� ��������� ����������� ������
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // ��������� ����������� ���������
        HandlePlayerFlip();

        // ��������� �������� ����� ����� �������� ��������
        if (Time.time >= nextCheckTime)
        {
            PerformRayCheck();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void PerformRayCheck()
    {
        // ���������� ����������� ���� � ����������� �� �������� ���������
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        // ��������� �� ������� ����� � ������ ����������
        bool enemyFound = CastRayWithTeleportCheck(transform.position, direction);

        // ������������ ����������� �����
        if (enemyFound && !enemyDetected)
        {
            enemyDetected = true;
            playerPulseUI?.StartPulse();
        }
        // ������������ ������ ����� �� ����
        else if (!enemyFound && enemyDetected)
        {
            enemyDetected = false;
            playerPulseUI?.StopPulse();
        }
    }

    private bool CastRayWithTeleportCheck(Vector2 origin, Vector2 direction)
    {
        // ���� ����� �������, ���������� ��������
        if (playerController.IsHidden)
        {
            return false;
        }

        // ��������� ��� ��� ��������
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance, enemyLayer | teleportLayer);
        // ������������ ��� � ��������� ��� �������
        Debug.DrawRay(origin, direction * rayDistance, rayColor);

        if (hit.collider != null)
        {
            // ��������� ��������� �� �����
            if (hit.collider.CompareTag("Enemy"))
            {
                return true;
            }
            // ��������� ��������� � ���� ������������
            else if (hit.collider.CompareTag("TeleportZone"))
            {
                TeleportZone teleport = hit.collider.GetComponent<TeleportZone>();
                if (teleport != null)
                {
                    // ���������� �������� �� ����� ������ ���������
                    Vector2 newOrigin = teleport.GetDestination().position;
                    return CastRayWithTeleportCheck(newOrigin, direction);
                }
            }
        }
        return false;
    }

    private void HandlePlayerFlip()
    {
        // ���������� ����������� ��������� �� ��� scale
        if (transform.localScale.x > 0)
        {
            isFacingRight = true;
        }
        else if (transform.localScale.x < 0)
        {
            isFacingRight = false;
        }
    }

    private void OnDrawGizmos()
    {
        // ������������ ��� � ���������
        Gizmos.color = rayColor;
        Gizmos.DrawLine(transform.position, transform.position + (isFacingRight ? Vector3.right : Vector3.left) * rayDistance);

        // ������������ ����� �� ����� ����
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (isFacingRight ? Vector3.right : Vector3.left) * rayDistance, 0.2f);
    }
}