using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportForPlayer : MonoBehaviour
{
    [SerializeField] private Transform destinationPointPlayer; // ������� ��� ����������� ������� ��� ��������

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �������� ������ other.gameObject � ����� Teleport
            Teleport(other.gameObject);
        }
    }

    public void Teleport(GameObject obj)
    {
        // ����������� �������
        if (destinationPointPlayer != null)
        {
            obj.transform.position = destinationPointPlayer.position;
        }
        else
        {
            Debug.LogWarning("Destination Point �� ����� ��� TeleportZone.");
        }
    }

    public Transform GetDestination()
    {
        return destinationPointPlayer;
    }

    private void OnDrawGizmos()
    {
        if (destinationPointPlayer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, destinationPointPlayer.position);
            Gizmos.DrawSphere(destinationPointPlayer.position, 0.2f);
        }
    }
}
