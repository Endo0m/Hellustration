using UnityEngine;

public class RayCheck : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer; // Слой врагов
    [SerializeField] private LayerMask teleportLayer; // Слой телепортов
    [SerializeField] private float rayDistance = 10f; // Длина луча
    [SerializeField] private Color rayColor = Color.red; // Цвет луча
    [SerializeField] private PlayerPulseUI playerPulseUI; // Ссылка на пульсацию UI
    [SerializeField] private float checkInterval = 1f; // Интервал проверки в секундах

    private bool isFacingRight = true; // Определение текущего направления игрока
    private float nextCheckTime; // Время следующей проверки
    private bool enemyDetected = false; // Флаг обнаружения врага
    private PlayerController playerController; // Ссылка на контроллер игрока

    private void Start()
    {
        playerController = GetComponent<PlayerController>(); // Получаем ссылку на контроллер игрока
    }

    private void Update()
    {
        // Обновляем направление игрока
        HandlePlayerFlip();

        // Выполняем проверку, если прошло нужное время
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

        // Управляем пульсацией UI в зависимости от наличия врага
        if (enemyFound && !enemyDetected)
        {
            enemyDetected = true;
            playerPulseUI?.StartPulse(); // Запускаем пульсацию
        }
        else if (!enemyFound && enemyDetected)
        {
            enemyDetected = false;
            playerPulseUI?.StopPulse(); // Останавливаем пульсацию
        }
    }

    private bool CastRayWithTeleportCheck(Vector2 origin, Vector2 direction)
    {
        // Проверяем, не скрыт ли игрок
        if (playerController.IsHidden)
        {
            Debug.Log("Игрок скрыт, не виден для врагов.");
            return false; // Игрок скрыт, и его не видно через луч
        }

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance, enemyLayer | teleportLayer);
        Debug.DrawRay(origin, direction * rayDistance, rayColor); // Визуализация луча

        if (hit.collider != null)
        {
            Debug.Log("Луч пересекся с объектом: " + hit.collider.name); // Дебаг, показываем имя объекта, с которым пересекся луч

            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Враг обнаружен!");
                return true; // Враг найден
            }
            else if (hit.collider.CompareTag("TeleportZone"))
            {
                Debug.Log("Телепортная зона обнаружена.");
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
            Debug.Log("Луч не встретил никаких объектов.");
        }

        return false; // Враг не найден
    }

    private void HandlePlayerFlip()
    {
        // Проверяем направление игрока по scale (например, для флипа по оси X)
        if (transform.localScale.x > 0)
        {
            isFacingRight = true;
        }
        else if (transform.localScale.x < 0)
        {
            isFacingRight = false;
        }
    }

    // Отображение луча и информации в редакторе через Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = rayColor;
        Gizmos.DrawLine(transform.position, transform.position + (isFacingRight ? Vector3.right : Vector3.left) * rayDistance); // Рисуем луч

        // Опционально, можно добавить дополнительную информацию
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (isFacingRight ? Vector3.right : Vector3.left) * rayDistance, 0.2f); // Конец луча
    }
}
