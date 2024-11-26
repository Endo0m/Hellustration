using UnityEngine;

public class RayCheck : MonoBehaviour
{
    [Header("Слой для обнаружения врагов")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("Слой для обнаружения зон телепортации")]
    [SerializeField] private LayerMask teleportLayer;

    [Header("Дальность луча")]
    [SerializeField] private float rayDistance = 10f;

    [Header("Цвет луча в редакторе")]
    [SerializeField] private Color rayColor = Color.red;

    [Header("Ссылка на UI индикатор пульса игрока")]
    [SerializeField] private PlayerPulseUI playerPulseUI;

    [Header("Интервал проверки луча")]
    [SerializeField] private float checkInterval = 1f;

    // Флаг направления персонажа (вправо/влево)
    private bool isFacingRight = true;
    // Время следующей проверки
    private float nextCheckTime;
    // Флаг обнаружения врага
    private bool enemyDetected = false;
    // Ссылка на контроллер игрока
    private PlayerController playerController;
    // Флаг обнаружения врага сзади
    private bool enemyDetectedBehind = false;

    private void Start()
    {
        // Получаем компонент контроллера игрока
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // Определяем направление персонажа
        HandlePlayerFlip();

        // Выполняем проверку лучей через заданный интервал
        if (Time.time >= nextCheckTime)
        {
            PerformRayCheck();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void PerformRayCheck()
    {
        // Определяем направление луча в зависимости от поворота персонажа
        Vector2 frontDirection = isFacingRight ? Vector2.right : Vector2.left;
        Vector2 backDirection = isFacingRight ? Vector2.left : Vector2.right;

        // Проверяем луч спереди
        bool frontEnemyFound = CastRayWithTeleportCheck(transform.position, frontDirection);
        // Проверяем луч сзади
        bool backEnemyFound = CastRayWithTeleportCheck(transform.position, backDirection);

        // Обрабатываем обнаружение врага спереди
        if (frontEnemyFound && !enemyDetected)
        {
            enemyDetected = true;
            playerPulseUI?.StartPulse();
        }
        else if (!frontEnemyFound && enemyDetected)
        {
            enemyDetected = false;
            playerPulseUI?.StopPulse();
        }

        // Обрабатываем обнаружение врага сзади
        if (backEnemyFound && !enemyDetectedBehind)
        {
            enemyDetectedBehind = true;
            playerPulseUI?.StartPulse();
        }
        else if (!backEnemyFound && enemyDetectedBehind)
        {
            enemyDetectedBehind = false;
            // Останавливаем пульс только если враг не обнаружен ни спереди, ни сзади
            if (!enemyDetected)
            {
                playerPulseUI?.StopPulse();
            }
        }
    }

    private bool CastRayWithTeleportCheck(Vector2 origin, Vector2 direction)
    {
        // Если игрок спрятан, пропускаем проверку
        if (playerController.IsHidden)
        {
            return false;
        }

        // Выпускаем луч для проверки
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance, enemyLayer | teleportLayer);
        // Отображаем луч в редакторе для отладки
        Debug.DrawRay(origin, direction * rayDistance, rayColor);

        if (hit.collider != null)
        {
            // Проверяем попадание во врага
            if (hit.collider.CompareTag("Enemy"))
            {
                return true;
            }
            // Проверяем попадание в зону телепортации
            else if (hit.collider.CompareTag("TeleportZone"))
            {
                TeleportZone teleport = hit.collider.GetComponent<TeleportZone>();
                if (teleport != null)
                {
                    // Продолжаем проверку от точки выхода телепорта
                    Vector2 newOrigin = teleport.GetDestination().position;
                    return CastRayWithTeleportCheck(newOrigin, direction);
                }
            }
        }
        return false;
    }

    private void HandlePlayerFlip()
    {
        // Определяем направление персонажа по его scale
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
        // Отображаем передний луч в редакторе
        Gizmos.color = rayColor;
        Vector3 frontDirection = isFacingRight ? Vector3.right : Vector3.left;
        Gizmos.DrawLine(transform.position, transform.position + frontDirection * rayDistance);

        // Отображаем задний луч в редакторе
        Vector3 backDirection = isFacingRight ? Vector3.left : Vector3.right;
        Gizmos.DrawLine(transform.position, transform.position + backDirection * rayDistance);

        // Отображаем сферы на конце лучей
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + frontDirection * rayDistance, 0.2f);
        Gizmos.DrawWireSphere(transform.position + backDirection * rayDistance, 0.2f);
    }
}