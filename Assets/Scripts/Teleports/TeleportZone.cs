using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TeleportZone : MonoBehaviour
{
    [SerializeField] private Transform destinationPoint; // Позиция для перемещения объекта при переходе

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            other.transform.position = destinationPoint.position;
            Debug.Log($"{other.tag} перемещен на новый этаж");
        }
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
