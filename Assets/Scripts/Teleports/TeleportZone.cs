using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TeleportZone : MonoBehaviour
{
    [SerializeField] private Transform destinationPoint; // ������� ��� ����������� ������� ��� ��������

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // �������� ������ other.gameObject � ����� Teleport
            Teleport(other.gameObject);
        }
    }

    public void Teleport(GameObject obj)
    {
        // ����������� �������
        if (destinationPoint != null)
        {
            obj.transform.position = destinationPoint.position;
        }
        else
        {
            Debug.LogWarning("Destination Point �� ����� ��� TeleportZone.");
        }
    }

    public Transform GetDestination()
    {
        return destinationPoint;
    }

    private void OnDrawGizmos()
    {
        if (destinationPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, destinationPoint.position);
            Gizmos.DrawSphere(destinationPoint.position, 0.2f);
        }
    }
}
