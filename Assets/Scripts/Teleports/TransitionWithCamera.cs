using System.Collections;
using UnityEngine;

public class TransitionWithCamera : MonoBehaviour
{
    [SerializeField] private Transform destinationPoint; // Точка назначения для игрока
    [SerializeField] private Transform cameraTargetPoint; // Точка назначения для камеры
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

            // Запускаем плавное перемещение камеры к новой точке
            if (cameraTargetPoint != null)
            {
                StartCoroutine(MoveCameraSmoothly(cameraTargetPoint.position));
            }
        }
    }

    private IEnumerator MoveCameraSmoothly(Vector3 targetPosition)
    {
        // Получаем начальную позицию камеры и изменяем только её X и Y к целевой точке
        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetCameraPosition = new Vector3(targetPosition.x, targetPosition.y, startPosition.z);

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            // Используем линейную интерполяцию для плавного перемещения камеры
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetCameraPosition, elapsedTime);
            elapsedTime += Time.deltaTime * cameraMoveSpeed;

            yield return null;
        }

        // Устанавливаем камеру точно на позицию назначения
        mainCamera.transform.position = targetCameraPosition;
    }

    private void OnDrawGizmos()
    {
        if (destinationPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, destinationPoint.position);
            Gizmos.DrawSphere(destinationPoint.position, 0.2f);
        }

        if (cameraTargetPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, cameraTargetPoint.position);
            Gizmos.DrawSphere(cameraTargetPoint.position, 0.2f);
        }
    }
}
