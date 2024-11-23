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
    private int lastCycleSpawn = -1;
    private bool isSpawning = false;

    // Сохраняем состояние модификации между циклами
    private bool wasModifiedLastCycle = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                int currentWaypoint = enemy.CurrentWaypoint;
                int waypointCount = enemy.WaypointCount;
                int currentCycle = currentWaypoint / waypointCount;
                int waypointInCycle = currentWaypoint % waypointCount;

                Debug.Log($"Enemy entered spawner: Cycle {currentCycle}, Waypoint {waypointInCycle}, Required {requiredWaypointForSpawn}, UseModified: {useModifiedItem}");

                if (waypointInCycle >= requiredWaypointForSpawn &&
                    currentCycle != lastCycleSpawn &&
                    !isSpawning)
                {
                    lastCycleSpawn = currentCycle;

                    // Если предмет не был модифицирован в предыдущем цикле,
                    // начинаем с первого базового предмета
                    if (!wasModifiedLastCycle)
                    {
                        useModifiedItem = false;
                        modifiedItemPrefab = null;
                    }

                    StartCoroutine(SpawnItemsSequentially());
                }
            }
        }
    }

    public void SetModifiedItem(GameObject newPrefab)
    {
        modifiedItemPrefab = newPrefab;
        useModifiedItem = true;
        wasModifiedLastCycle = true;

        Debug.Log($"Item modifier updated. UseModified: {useModifiedItem}, Modified prefab: {newPrefab.name}");
    }

    private IEnumerator SpawnItemsSequentially()
    {
        isSpawning = true;

        // Спавним первый предмет
        if (itemPrefabs.Length > 0)
        {
            GameObject firstItem = wasModifiedLastCycle && modifiedItemPrefab != null ?
                modifiedItemPrefab : itemPrefabs[0];

            GameObject item = Instantiate(firstItem, spawnPoint.position, spawnPoint.rotation);
            ItemLifespan lifespan = item.GetComponent<ItemLifespan>();
            if (lifespan != null)
            {
                lifespan.Initialize(requiredWaypointForSpawn);
                Debug.Log($"Spawned first item with requiredWaypointForSpawn: {requiredWaypointForSpawn}");
            }
        }

        yield return new WaitForSeconds(timeUntilNextItem);

        // Спавним второй предмет
        GameObject itemToSpawn = useModifiedItem && modifiedItemPrefab != null ?
            modifiedItemPrefab :
            (itemPrefabs.Length > 1 ? itemPrefabs[1] : null);

        if (itemToSpawn != null)
        {
            GameObject item = Instantiate(itemToSpawn, spawnPoint.position, spawnPoint.rotation);
            ItemLifespan lifespan = item.GetComponent<ItemLifespan>();
            if (lifespan != null)
            {
                lifespan.Initialize(requiredWaypointForSpawn);
                Debug.Log($"Spawned second item with requiredWaypointForSpawn: {requiredWaypointForSpawn}");
            }
        }

        isSpawning = false;
    }

    // Метод для сброса состояния модификации (вызывать при необходимости)
    public void ResetModification()
    {
        useModifiedItem = false;
        modifiedItemPrefab = null;
        wasModifiedLastCycle = false;
    }

    private void OnDisable()
    {
        // Сбрасываем состояние при отключении компонента
        ResetModification();
    }
}