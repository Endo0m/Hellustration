using UnityEngine;

public class ItemLifespan : MonoBehaviour
{
    [SerializeField] private float lifespan = -1f;
    [SerializeField] private int requiredWaypoint = 5; // Значение по умолчанию
    private bool isDestroyed = false;
    private int spawnCycle = -1;
    private bool isInitialized = false;

    public void Initialize(int spawnRequiredWaypoint)
    {
        // Используем requiredWaypoint из инспектора, если он не был инициализирован
        if (!isInitialized)
        {
            isInitialized = true;
            // Используем значение из инспектора, если оно больше чем переданное
            requiredWaypoint = Mathf.Max(requiredWaypoint, spawnRequiredWaypoint);
        }

        if (lifespan > 0)
        {
            Invoke(nameof(DestroyItem), lifespan);
        }

        Debug.Log($"Item initialized with requiredWaypoint: {requiredWaypoint}, lifespan: {lifespan}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDestroyed && other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                int currentWaypoint = enemy.CurrentWaypoint;
                int waypointCount = enemy.WaypointCount;
                int currentCycle = currentWaypoint / waypointCount;
                int waypointInCycle = currentWaypoint % waypointCount;

                // Запоминаем цикл при первом входе в триггер
                if (spawnCycle == -1)
                {
                    spawnCycle = currentCycle;
                }

                Debug.Log($"Item check: Cycle {currentCycle}, Waypoint {waypointInCycle}, " +
                    $"Required {requiredWaypoint}, SpawnCycle {spawnCycle}, " +
                    $"Will destroy: {currentCycle == spawnCycle && waypointInCycle >= requiredWaypoint}");

                // Проверяем, что это тот же цикл и достигнута нужная точка
                if (currentCycle == spawnCycle && waypointInCycle >= requiredWaypoint)
                {
                    Debug.Log($"Destroying item at waypoint {waypointInCycle} (required: {requiredWaypoint})");
                    DestroyItem();
                }
            }
        }
    }

    private void DestroyItem()
    {
        if (!isDestroyed)
        {
            Debug.Log($"Item destroyed! Required waypoint was: {requiredWaypoint}");
            Destroy(gameObject);
            isDestroyed = true;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Проверка в редакторе для уведомления о некорректных значениях
        if (requiredWaypoint < 0)
        {
            Debug.LogWarning("Required waypoint should not be negative!");
            requiredWaypoint = 0;
        }
    }
#endif
}