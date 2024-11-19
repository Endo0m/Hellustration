using System.Collections;
using UnityEngine;

public class PieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject piePrefab; // ������ ������
    [SerializeField] private Transform spawnPoint; // ����� ��������� ������
    [SerializeField] private float spawnCooldown = 15f; // ������� �� ���������� ��������� ������
    [SerializeField] private int requiredWaypointForSpawn = 1; // �����, ������� ���� ������ ������ ��� ��������� ������

    private bool hasSpawned = false; // ����, ��������������� ��������� �����

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemyBase = other.GetComponent<EnemyBase>();

            // ���������, ������ �� ���� ������ ����� � �� ��� �� ������� ����� ���������
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
        hasSpawned = false; // ��������� ��������� ����� ����� ��������
    }
}
