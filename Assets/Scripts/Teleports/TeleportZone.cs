using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TeleportZone : MonoBehaviour
{
    [SerializeField] private Transform destinationPoint; // Позиция для перемещения объекта при переходе

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Передаем объект other.gameObject в метод Teleport
            Teleport(other.gameObject);
        }
    }

    public void Teleport(GameObject obj)
    {
        // Перемещение объекта
        if (destinationPoint != null)
        {
            obj.transform.position = destinationPoint.position;
        }
        else
        {
            Debug.LogWarning("Destination Point не задан для TeleportZone.");
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
