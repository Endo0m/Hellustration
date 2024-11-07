using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class PlayerPulseUI : MonoBehaviour
{
    [SerializeField] private Image pulseImage; // UI-изображение, которое будет мигать
    [SerializeField] private float pulseDuration = 0.5f; // Длительность одного мигания
    [SerializeField] private float pulseScale = 1.2f; // Коэффициент масштабирования мигания
    private float pulseCounter = 100f;

    private Tween currentPulseTween;

    private void Update()
    {
        pulseImage.fillAmount = pulseCounter / 100f;
        Debug.Log(pulseCounter);
    }

    // Запускает мигание
    public void StartPulse()
    {
        if (currentPulseTween != null && currentPulseTween.IsActive())
        {
            currentPulseTween.Kill(); // Останавливает текущее мигание, если оно уже запущено
        }

        // Создаёт новое мигание (увеличивает размер изображения и затем уменьшает его обратно)
        currentPulseTween = pulseImage.rectTransform.DOScale(pulseScale, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // Повторяет мигание бесконечно
    }


    // Останавливает мигание
    public void StopPulse()
    {
        if (currentPulseTween != null && currentPulseTween.IsActive())
        {
            currentPulseTween.Kill(); // Останавливает текущее мигание
            pulseImage.rectTransform.localScale = Vector3.one; // Возвращает изображению исходный размер
        }
    }

    public IEnumerator PulseCoroutine()
    {
        DecreasePulseCounter();
        StartPulse();
        yield return new WaitForSeconds(5f); // ждем 5 секунд
        StopPulse();
        yield return StartCoroutine(RestorePulseCounter());
    }

    private void DecreasePulseCounter()
    {
        if (pulseCounter > 0)
        {
            pulseCounter -= 20f;
        }
    }

    private IEnumerator RestorePulseCounter()
    {
        float startTime = Time.time;
        float duration = 10f; // время восстановления
        float startValue = pulseCounter;
        float endValue = 100f;

        while (Time.time - startTime < duration)
        {
            pulseCounter = Mathf.Lerp(startValue, endValue, (Time.time - startTime) / duration);
            yield return null;
        }

        pulseCounter = endValue;
    }
}
