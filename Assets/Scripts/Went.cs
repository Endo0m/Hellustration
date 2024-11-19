using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Went : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // �������� �������� � ������
    [SerializeField] private float stopDistance = 0.1f; // ����������, �� ������� ������ ����� ������

    private Transform playerTransform;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("����� �� ������! ���������, ��� ������ � ����� 'Player' ����������.");
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // ����������� � ������
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // ����������� � ������
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);

        // �������� �� ���������� ������
        if (Vector2.Distance(transform.position, playerTransform.position) <= stopDistance)
        {
            Destroy(gameObject); // ������� ������
        }
    }
}
