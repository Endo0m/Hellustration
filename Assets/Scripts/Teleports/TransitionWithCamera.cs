using System.Collections;
using UnityEngine;

public class TransitionWithCamera : MonoBehaviour
{
    [SerializeField] private Transform destinationPoint; // ����� ���������� ��� ������
    [SerializeField] private Transform cameraTargetPoint; // ����� ���������� ��� ������
    [SerializeField] private float cameraMoveSpeed = 2f; // �������� ����������� ������

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main; // �������� �������� ������
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ���������� ������ ��������� �� ����� �������
            other.transform.position = destinationPoint.position;

            // ��������� ������� ����������� ������ � ����� �����
            if (cameraTargetPoint != null)
            {
                StartCoroutine(MoveCameraSmoothly(cameraTargetPoint.position));
            }
        }
    }

    private IEnumerator MoveCameraSmoothly(Vector3 targetPosition)
    {
        // �������� ��������� ������� ������ � �������� ������ � X � Y � ������� �����
        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetCameraPosition = new Vector3(targetPosition.x, targetPosition.y, startPosition.z);

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            // ���������� �������� ������������ ��� �������� ����������� ������
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetCameraPosition, elapsedTime);
            elapsedTime += Time.deltaTime * cameraMoveSpeed;

            yield return null;
        }

        // ������������� ������ ����� �� ������� ����������
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
