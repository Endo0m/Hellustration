using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButcherEnemy : EnemyBase
{
    [SerializeField] private List<string> requiredItems;
    [SerializeField] private float detectionRadius = 3f; // ������ ����������� ��� "�����" ������
    private HashSet<string> collectedItems = new HashSet<string>();
    private bool isPlayerCaptured = false;
    private bool hasAllItems = false; // ����, ������������, ��� ��� �������� �������

    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemIdentifier item = other.GetComponent<ItemIdentifier>();
        if (item != null && requiredItems.Contains(item.ItemName))
        {
            collectedItems.Add(item.ItemName);
            Debug.Log($"������ ������� �������: {item.ItemName}");
            CheckIfAllItemsCollected();
        }
    }

    private void CheckIfAllItemsCollected()
    {
        if (collectedItems.Count >= requiredItems.Count)
        {
            hasAllItems = true; // ������������� ����, ��� ��� �������� �������
            Debug.Log("��� �������� �������. ������ ��������� �������.");
        }
    }

    protected override void Update()
    {
        if (!isPlayerCaptured)
        {
            base.Update(); // ��������� ������� ������ �����
            DetectAndCapturePlayer();
        }

        // ��������, ������ �� ���� ��������� ����� ��������
        if (hasAllItems && currentWaypointIndex == waypoints.Length - 1)
        {
            float distanceToLastWaypoint = Vector2.Distance(transform.position, waypoints[waypoints.Length - 1].point.position);
            if (distanceToLastWaypoint < 0.1f) // �������� �� �������� � ��������� �����
            {
                CheckIfDefeated();
            }
        }
    }


    protected override void CheckIfDefeated()
    {
        if (hasAllItems && currentWaypointIndex >= waypoints.Length - 1)
        {
            Debug.Log("������ �������� �� ��������� ����� ��������!");
            Destroy(gameObject);
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void DetectAndCapturePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // ���������, ��������� �� ����� �� ���� "Hidden"
            if (player.layer == LayerMask.NameToLayer("Hidden"))
            {
                // ����� �������, �� ������������ � �� ����������� ���
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= detectionRadius)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    // ������������� ����, ����� ���������� �����
                    isPlayerCaptured = true;

                    // ��������� ������������������ ������ ������
                    playerController.TriggerDeathSequence();

                    // ������������� �������� �����
                    StopEnemyActions();
                }
            }
        }
    }


    private void StopEnemyActions()
    {
        // ������������� �������� �����
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsChasing", false);

        // ������������� ��������
        rb.velocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // ������ ������ ����������� ������
    }
}
