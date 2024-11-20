using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButcherEnemy : EnemyBase
{
    [SerializeField] private List<string> requiredItems;
    [SerializeField] private float detectionRadius = 3f; // Радиус обнаружения для "ловли" игрока
    private HashSet<string> collectedItems = new HashSet<string>();
    private bool isPlayerCaptured = false;
    private bool hasAllItems = false; // Флаг, показывающий, что все предметы собраны

    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemIdentifier item = other.GetComponent<ItemIdentifier>();
        if (item != null && requiredItems.Contains(item.ItemName))
        {
            collectedItems.Add(item.ItemName);
            Debug.Log($"Мясник получил предмет: {item.ItemName}");
            CheckIfAllItemsCollected();
        }
    }

    private void CheckIfAllItemsCollected()
    {
        if (collectedItems.Count >= requiredItems.Count)
        {
            hasAllItems = true; // Устанавливаем флаг, что все предметы собраны
            Debug.Log("Все предметы собраны. Мясник завершает маршрут.");
        }
    }

    protected override void Update()
    {
        if (!isPlayerCaptured)
        {
            base.Update(); // Выполняем обычную логику врага
            DetectAndCapturePlayer();
        }

        // Проверка, достиг ли враг последней точки маршрута
        if (hasAllItems && currentWaypointIndex == waypoints.Length - 1)
        {
            float distanceToLastWaypoint = Vector2.Distance(transform.position, waypoints[waypoints.Length - 1].point.position);
            if (distanceToLastWaypoint < 0.1f) // Проверка на близость к последней точке
            {
                CheckIfDefeated();
            }
        }
    }


    protected override void CheckIfDefeated()
    {
        if (hasAllItems && currentWaypointIndex >= waypoints.Length - 1)
        {
            Debug.Log("Мясник побежден на последней точке маршрута!");
            Destroy(gameObject);
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void DetectAndCapturePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Проверяем, находится ли игрок на слое "Hidden"
            if (player.layer == LayerMask.NameToLayer("Hidden"))
            {
                // Игрок спрятан, не обнаруживаем и не захватываем его
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= detectionRadius)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    // Устанавливаем флаг, чтобы остановить врага
                    isPlayerCaptured = true;

                    // Запускаем последовательность смерти игрока
                    playerController.TriggerDeathSequence();

                    // Останавливаем действия врага
                    StopEnemyActions();
                }
            }
        }
    }


    private void StopEnemyActions()
    {
        // Останавливаем анимации врага
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsChasing", false);

        // Останавливаем движение
        rb.velocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Рисуем радиус обнаружения игрока
    }
}
