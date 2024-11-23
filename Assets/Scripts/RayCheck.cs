using UnityEngine;

public class RayCheck : MonoBehaviour
{
    [Header("Слой для определения врагов")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("Слой для определения зон телепортации")]
    [SerializeField] private LayerMask teleportLayer;

    [Header("Дальность луча")]
    [SerializeField] private float rayDistance = 10f;

    [Header("Цвет луча в редакторе")]
    [SerializeField] private Color rayColor = Color.red;

    [Header("Ссылка на UI компонент пульса игрока")]
    [SerializeField] private PlayerPulseUI playerPulseUI;

    [Header("Интервал проверки луча")]
    [SerializeField] private float checkInterval = 1f;

    [Header("Контроллер пульса")]
    [SerializeField] private PulseController pulseController;

    // Флаг направления персонажа (вправо/влево)
    private bool isFacingRight = true;
    // Время следующей проверки
    private float nextCheckTime;
    // Флаг обнаружения врага
    private bool enemyDetected = false;
    // Ссылка на контроллер игрока
    private PlayerController playerController;

    private void Start()
    {
        // Получаем компонент контроллера игрока
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // Проверяем направление персонажа
        HandlePlayerFlip();

        // Выполняем проверку лучом через заданный интервал
        if (Time.time >= nextCheckTime)
        {
            PerformRayCheck();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void PerformRayCheck()
    {
        // Определяем направление луча в зависимости от поворота персонажа
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        // Проверяем на наличие врага с учетом телепортов
        bool enemyFound = CastRayWithTeleportCheck(transform.position, direction);

        // Обрабатываем обнаружение врага
        if (enemyFound && !enemyDetected)
        {
            enemyDetected = true;
            playerPulseUI?.StartPulse();
            pulseController?.StartIncreasingPulse();
        }
        // Обрабатываем потерю врага из виду
        else if (!enemyFound && enemyDetected)
        {
            enemyDetected = false;
            playerPulseUI?.StopPulse();
            pulseController?.StopIncreasingPulse();
        }
    }

    private bool CastRayWithTeleportCheck(Vector2 origin, Vector2 direction)
    {
        // Если игрок спрятан, прекращаем проверку
        if (playerController.IsHidden)
        {
            return false;
        }

        // Запускаем луч для проверки
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance, enemyLayer | teleportLayer);
        // Отрисовываем луч в редакторе для отладки
        Debug.DrawRay(origin, direction * rayDistance, rayColor);

        if (hit.collider != null)
        {
            // Проверяем попадание по врагу
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
        // Отрисовываем луч в редакторе
        Gizmos.color = rayColor;
        Gizmos.DrawLine(transform.position, transform.position + (isFacingRight ? Vector3.right : Vector3.left) * rayDistance);

        // Отрисовываем сферу на конце луча
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (isFacingRight ? Vector3.right : Vector3.left) * rayDistance, 0.2f);
    }
}