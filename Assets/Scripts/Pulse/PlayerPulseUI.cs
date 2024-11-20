using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class PlayerPulseUI : MonoBehaviour
{
    [SerializeField] public Image pulseImage; // UI-изображение, которое будет мигать
    [SerializeField] private float pulseDuration = 0.5f; // Длительность одного мигания
    [SerializeField] private float pulseScale = 1.2f; // Коэффициент масштабирования мигания
    [SerializeField] private GameObject overGame;
    private Tween currentPulseTween;
    private bool isPulseRunning = false;

    // Запускает мигание
    public void StartPulse()
    {
        if (!isPulseRunning)
        {
            isPulseRunning = true;
            if (currentPulseTween != null && currentPulseTween.IsActive())
            {
                currentPulseTween.Kill(); // Останавливает текущее мигание, если оно уже запущено
            }

            // Создаёт новое мигание (увеличивает размер изображения и затем уменьшает его обратно)
            currentPulseTween = pulseImage.rectTransform.DOScale(pulseScale, pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo); // Повторяет мигание бесконечно
        }
    }


    // Останавливает мигание
    public void StopPulse()
    {
         if (isPulseRunning)
        {
            isPulseRunning = false;
            if (currentPulseTween != null && currentPulseTween.IsActive())
            {
                currentPulseTween.Kill(); // Останавливает текущее мигание
                pulseImage.rectTransform.localScale = Vector3.one; // Возвращает изображению исходный размер
            }
        }
    }

    public void GameOver()
    {
        overGame.SetActive(true);
    }
}
