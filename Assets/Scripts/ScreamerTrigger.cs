using System.Collections;
using UnityEngine;

public class ScreamerTrigger : MonoBehaviour
{
    [SerializeField] private GameObject screamerPrefab; // ������ ��������
    [SerializeField] private Transform spawnPoint; // ����� �������� ��������
    [SerializeField] private float screamerLifetime = 3.0f; // ����� ����� �������� � ��������
    private bool hasTriggered = false; // ���� ��� ������������ ������������

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���������, ���� ������, �������� � �������, �������� ������� � ������ ��� �� ����������
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // ������������� ����, ����� ������ ������ �� ����������
            StartCoroutine(SpawnScreamer());
        }
    }

    private IEnumerator SpawnScreamer()
    {
        // ���� �� ������ ����� ��������, ���������� ������� ��������
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        // ������� �������
        GameObject screamer = Instantiate(screamerPrefab, spawnPosition, spawnRotation);

        // ���� �������� �����
        yield return new WaitForSeconds(screamerLifetime);

        // ������� �������
        if (screamer != null)
        {
            Destroy(screamer);
        }
    }
}
