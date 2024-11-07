using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerPulseUI : MonoBehaviour
{
    [SerializeField] private Image pulseImage; // UI-�����������, ������� ����� ������������
    [SerializeField] private float pulseDuration = 0.5f; // ����������������� ������ ����� ���������
    [SerializeField] private float pulseScale = 1.2f; // ������� ��� ���������

    private Tween currentPulseTween;

    // ����� ��� ������� ���������
    public void StartPulse()
    {
        if (currentPulseTween != null && currentPulseTween.IsActive())
        {
            currentPulseTween.Kill(); // ������������� ������� ��������, ���� ����
        }

        // ��������� ����������� (���������� � ������� � ��������� �������)
        currentPulseTween = pulseImage.rectTransform.DOScale(pulseScale, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // ����������� ���� � ���������
    }

    // ����� ��� ��������� ���������
    public void StopPulse()
    {
        if (currentPulseTween != null && currentPulseTween.IsActive())
        {
            currentPulseTween.Kill(); // ������������� ������� ��������
            pulseImage.rectTransform.localScale = Vector3.one; // ���������� � ������������ �������
        }
    }
}
