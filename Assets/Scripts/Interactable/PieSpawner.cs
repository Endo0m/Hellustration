using System.Collections;
using UnityEngine;

public class PieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject piePrefab; // Префаб пирога
    [SerializeField] private Transform spawnPoint; // Точка появления пирога
    [SerializeField] private float spawnCooldown = 15f; // Таймаут до повторного появления пирога
    [SerializeField] private int requiredWaypointForSpawn = 1; // Точка, которую враг должен пройти для активации спавна

    private bool hasSpawned = false; // Флаг, предотвращающий повторный спавн

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemyBase = other.GetComponent<EnemyBase>();

            // Проверяем, прошел ли враг нужную точку и не был ли предмет ранее заспавнен
            if (enemyBase != null && enemyBase.CurrentWaypoint >= requiredWaypointForSpawn && !hasSpawned)
            {
                SpawnPie();
            }
        }
    }

    private void SpawnPie()
    {
        hasSpawned = true;
        Instantiate(piePrefab, spawnPoint.position, spawnPoint.rotation);
        StartCoroutine(ResetSpawnCooldown());
    }

    private IEnumerator ResetSpawnCooldown()
    {
        yield return new WaitForSeconds(spawnCooldown);
        hasSpawned = false; // Разрешаем повторный спавн после кулдауна
    }
}
