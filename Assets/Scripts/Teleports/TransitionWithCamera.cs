using System.Collections;
using UnityEngine;

public class TransitionWithCamera : MonoBehaviour
{
    [SerializeField] private Transform destinationPoint; // Точка назначения для игрока
    [SerializeField] private float cameraMoveSpeed = 2f; // Скорость перемещения камеры

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main; // Получаем основную камеру
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Перемещаем игрока мгновенно на новую позицию
            other.transform.position = destinationPoint.position;

          
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
