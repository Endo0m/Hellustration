using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] itemPrefabs; // Массив префабов предметов
    [SerializeField] private Transform spawnPoint; // Точка появления предметов
    [SerializeField] private float spawnCooldown = 10f; // Таймаут до повторного появления всех предметов
    [SerializeField] private float timeUntilNextItem = 5f; // Задержка перед появлением следующего предмета
    [SerializeField] private int requiredWaypointForSpawn = 1; // Точка, которую враг должен пройти для активации спавна

    private GameObject modifiedItemPrefab; // Измененная версия второго предмета
    private bool useModifiedItem = false; // Флаг, указывающий, использовать ли измененный предмет
    private bool hasSpawned = false; // Флаг, предотвращающий повторный спавн

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemyBase = other.GetComponent<EnemyBase>();

            // Add debug logging to verify conditions
            Debug.Log($"Enemy detected. Current Waypoint: {enemyBase.CurrentWaypoint}, Required Waypoint: {requiredWaypointForSpawn}");

            if (enemyBase != null && enemyBase.CurrentWaypoint >= requiredWaypointForSpawn && !hasSpawned)
            {
                StartCoroutine(SpawnItemsSequentially());
            }
        }
    }

    // Устанавливает измененный префаб для спавна
    public void SetModifiedItem(GameObject newPrefab)
    {
        modifiedItemPrefab = newPrefab;
        useModifiedItem = true; // Переключаем на использование модифицированного предмета
    }

    private IEnumerator SpawnItemsSequentially()
    {
        hasSpawned = true;

        // Спавн первого предмета
        if (itemPrefabs.Length > 0)
        {
            Instantiate(itemPrefabs[0], spawnPoint.position, spawnPoint.rotation);
        }

        // Ожидание перед спавном следующего предмета
        yield return new WaitForSeconds(timeUntilNextItem);

        // Спавн второго предмета: модифицированного или стандартного
        GameObject itemToSpawn = useModifiedItem && modifiedItemPrefab != null ? modifiedItemPrefab : itemPrefabs[1];
        if (itemToSpawn != null)
        {
            Instantiate(itemToSpawn, spawnPoint.position, spawnPoint.rotation);
        }

        // Задержка перед повторным спавном
        yield return new WaitForSeconds(spawnCooldown);
        hasSpawned = false; // Разрешаем повторный спавн при следующем триггере
    }
}
