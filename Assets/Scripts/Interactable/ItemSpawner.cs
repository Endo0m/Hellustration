using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] itemPrefabs; // ������ �������� ���������
    [SerializeField] private Transform spawnPoint; // ����� ��������� ���������
    [SerializeField] private float spawnCooldown = 10f; // ������� �� ���������� ��������� ���� ���������
    [SerializeField] private float timeUntilNextItem = 5f; // �������� ����� ���������� ���������� �������� ����� ����������� ��������
    private GameObject modifiedItemPrefab; // ���������� ������ ������� ��������
    private bool useModifiedItem = false; // ����, �����������, ������������ �� ���������� �������

    private bool hasSpawned = false;
    private bool hasEnemyPassedFirstWaypoint = false; // ����, �����������, ������ �� ���� ������ �����

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // ���������, ��� ���� ������ ������ �����
            if (hasEnemyPassedFirstWaypoint && !hasSpawned)
            {
                StartCoroutine(SpawnItemsSequentially());
            }
        }
    }

    // ���� ����� ���������� �� EnemyBase, ����� ��������, ��� ���� ������ ������ �����
    public void MarkEnemyPassedFirstWaypoint()
    {
        hasEnemyPassedFirstWaypoint = true;
    }

    public void SetModifiedItem(GameObject newPrefab)
    {
        modifiedItemPrefab = newPrefab;
        useModifiedItem = true; // ����������� �� ������������� ����������������� ��������
    }

    private IEnumerator SpawnItemsSequentially()
    {
        hasSpawned = true;

        // ����� ������� ��������
        if (itemPrefabs.Length > 0)
        {
            Instantiate(itemPrefabs[0], spawnPoint.position, spawnPoint.rotation);
        }

        // �������� ����� ������� ������� ��������
        yield return new WaitForSeconds(timeUntilNextItem);

        // ����� ������� �������� (����������������� ��� ������������)
        GameObject itemToSpawn = useModifiedItem && modifiedItemPrefab != null ? modifiedItemPrefab : itemPrefabs[1];
        if (itemToSpawn != null)
        {
            Instantiate(itemToSpawn, spawnPoint.position, spawnPoint.rotation);
        }

        // �������� ����� ��������� �������
        yield return new WaitForSeconds(spawnCooldown);
        hasSpawned = false; // ��������� ��������� ����� ��� ��������� ��������
    }
}
