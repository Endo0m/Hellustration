using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerPulseUI : MonoBehaviour
{
    [SerializeField] private Image pulseImage; // UI-изображение, которое будет пульсировать
    [SerializeField] private float pulseDuration = 0.5f; // Продолжительность одного цикла пульсации
    [SerializeField] private float pulseScale = 1.2f; // Масштаб при пульсации

    private Tween currentPulseTween;

    // Метод для запуска пульсации
    public void StartPulse()
    {
        if (currentPulseTween != null && currentPulseTween.IsActive())
        {
            currentPulseTween.Kill(); // Останавливаем текущую анимацию, если есть
        }

        // Пульсация изображения (увеличение и возврат к исходному размеру)
        currentPulseTween = pulseImage.rectTransform.DOScale(pulseScale, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // Бесконечный цикл с возвратом
    }

    // Метод для остановки пульсации
    public void StopPulse()
    {
        if (currentPulseTween != null && currentPulseTween.IsActive())
        {
            currentPulseTween.Kill(); // Останавливаем текущую анимацию
            pulseImage.rectTransform.localScale = Vector3.one; // Возвращаем к стандартному размеру
        }
    }
}
