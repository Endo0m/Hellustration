using UnityEngine;
using UnityEngine.UI;

public class UIFluctuateFill : MonoBehaviour
{
    public Image targetImage; // ������ �� Image, ��� ������� ���������� fillAmount
    public float speed = 1f; // �������� ��������� fillAmount
    private float direction = 1f; // ����������� ���������: 1 - ������, -1 - �����

    private void Update()
    {
        if (targetImage != null)
        {
            // ��������� fillAmount � ������ ����������� � ��������
            targetImage.fillAmount += direction * speed * Time.deltaTime;

            // �������� �� ������� (�� 0 �� 1) � ����� �����������
            if (targetImage.fillAmount >= 1f)
            {
                targetImage.fillAmount = 1f;
                direction = -1f; // ������ ����������� �� ��������
            }
            else if (targetImage.fillAmount <= 0f)
            {
                targetImage.fillAmount = 0f;
                direction = 1f; // ������ ����������� �� ��������
            }
        }
    }
}
