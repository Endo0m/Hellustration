using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float timeUntilNextItem = 5f;
    [SerializeField] private int requiredWaypointForSpawn = 1;

    private GameObject modifiedItemPrefab;
    private bool useModifiedItem = false;
    private bool isSpawning = false;
    private int currentCycle = 0;
    private int lastWaypointSeen = -1;
    private bool spawnedInCurrentCycle = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                int currentWaypoint = enemy.CurrentWaypoint;
                int waypointCount = enemy.WaypointCount;
                int waypointInCycle = currentWaypoint % waypointCount;

                // Определяем начало нового круга (когда точка меньше предыдущей или равна 0)
                if (waypointInCycle == 0 && lastWaypointSeen > 0)
                {
                    currentCycle++;
                    spawnedInCurrentCycle = false;
                    Debug.Log($"New cycle started: {currentCycle}, resetting spawn flags");
                }

                lastWaypointSeen = waypointInCycle;

                Debug.Log($"Enemy entered spawner: Cycle {currentCycle}, Waypoint {waypointInCycle}, " +
                    $"Required {requiredWaypointForSpawn}, Spawned in cycle: {spawnedInCurrentCycle}");

                // Проверяем условия для спавна:
                // 1. Достигли или прошли требуемую точку
                // 2. Еще не спавнили в этом цикле
                // 3. Не в процессе спавна
                if (waypointInCycle == requiredWaypointForSpawn &&
                    !spawnedInCurrentCycle &&
                    !isSpawning)
                {
                    Debug.Log($"Spawning items for cycle {currentCycle} at waypoint {waypointInCycle}");
                    spawnedInCurrentCycle = true;
                    StartCoroutine(SpawnItemsSequentially());
                }
            }
        }
    }

    private IEnumerator SpawnItemsSequentially()
    {
        isSpawning = true;

        // Спавним первый предмет
        if (itemPrefabs.Length > 0)
        {
            GameObject itemToSpawn = useModifiedItem && modifiedItemPrefab != null ?
                modifiedItemPrefab : itemPrefabs[0];

            GameObject item = Instantiate(itemToSpawn, spawnPoint.position, spawnPoint.rotation);
            ItemLifespan lifespan = item.GetComponent<ItemLifespan>();
            if (lifespan != null)
            {
                lifespan.Initialize(requiredWaypointForSpawn);
                Debug.Log($"Spawned first item in cycle {currentCycle}");
            }
        }

        yield return new WaitForSeconds(timeUntilNextItem);

        // Спавним второй предмет
        if (itemPrefabs.Length > 1)
        {
            GameObject itemToSpawn = useModifiedItem && modifiedItemPrefab != null ?
                modifiedItemPrefab : itemPrefabs[1];

            if (itemToSpawn != null)
            {
                GameObject item = Instantiate(itemToSpawn, spawnPoint.position, spawnPoint.rotation);
                ItemLifespan lifespan = item.GetComponent<ItemLifespan>();
                if (lifespan != null)
                {
                    lifespan.Initialize(requiredWaypointForSpawn);
                    Debug.Log($"Spawned second item in cycle {currentCycle}");
                }
            }
        }

        yield return new WaitForSeconds(0.1f);
        isSpawning = false;
        Debug.Log($"Finished spawning for cycle {currentCycle}");
    }

    public void SetModifiedItem(GameObject newPrefab)
    {
        modifiedItemPrefab = newPrefab;
        useModifiedItem = true;
        Debug.Log($"Modified item set to: {newPrefab.name}");
    }

    public void ResetSpawner()
    {
        currentCycle = 0;
        lastWaypointSeen = -1;
        spawnedInCurrentCycle = false;
        isSpawning = false;
        useModifiedItem = false;
        modifiedItemPrefab = null;
    }

    private void OnDisable()
    {
        ResetSpawner();
    }
}