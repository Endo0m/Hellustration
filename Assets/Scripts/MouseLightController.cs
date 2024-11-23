using UnityEngine;
using UnityEngine.Rendering.Universal; 

public class MouseLightController : MonoBehaviour
{
    [SerializeField] private Light2D spotlight; // ������ �� Light2D ������

    private void Update()
    {
        // �������� ������� ���� � ������� �����������
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ������������� ����� ������� ���� (Z ������� 0, ��� ��� ��� 2D)
        spotlight.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);
    }
}
