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

    // public IEnumerator PulseCoroutine()
    // {
    //     DecreasePulseCounter();
    //     StartPulse();
    //     yield return new WaitForSeconds(5f); // ждем 5 секунд
    //     StopPulse();
    //     yield return StartCoroutine(RestorePulseCounter());
    // }

    // private void DecreasePulseCounter()
    // {
    //     if (pulseCounter > 0)
    //     {
    //         pulseCounter -= 20;
    //     }
    // }

    // private IEnumerator RestorePulseCounter()
    // {
    //     int startValue = pulseCounter;
    //     int endValue = 100;

    //     while (pulseCounter < endValue)
    //     {
    //         pulseCounter = Mathf.Min(endValue, pulseCounter + 1);
    //         yield return new WaitForSeconds(1f); // восстановление пульса происходит каждые 0.1 секунды
    //     }
    // }

    // public void PulseDecreaseRun(bool isRunning)
    // {
    //     if (isRunning)
    //     {
    //         InvokeRepeating("DecreasePulseCounterByOne", 1f, 1f); // Уменьшить pulseCounter на 1 каждую секунду
    //         if (restoreCoroutine != null)
    //         {
    //             StopCoroutine(restoreCoroutine);
    //             restoreCoroutine = null;
    //         }
    //     }
    //     else
    //     {
    //         CancelInvoke("DecreasePulseCounterByOne"); // Остановить уменьшение pulseCounter, когда игрок перестает бежать
    //         restoreCoroutine = StartCoroutine(RestorePulseCounter()); // Восстановить pulseCounter
    //     }
    // }

    // private void DecreasePulseCounterByOne()
    // {
    //     if (pulseCounter > 0)
    //     {
    //         pulseCounter -= 5;
    //     }
    // }

}