using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PulseController : MonoBehaviour
{
    [SerializeField] private PlayerPulseUI playerPulseUI;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private int pulseCounter = 60;
    private Coroutine pulseIncreaseCoroutine;

    private void Update()
    {
        if (pulseCounter > 65)
        {
            playerPulseUI.StartPulse();
        }
        else if (pulseCounter < 65)
        {
            playerPulseUI.StopPulse();
        }
        textMeshPro.text = pulseCounter.ToString();
        if (pulseCounter > 800)
        {
            playerPulseUI.GameOver();
        }
    }

    private IEnumerator PulseCoroutine()
    {
        yield return new WaitForSeconds(2f);
        if (pulseCounter > 60)
        {
            pulseCounter -= 1;
        }
    }

    public void RestorePulse()
    {
        StartCoroutine(PulseCoroutine());
    }

    public void UpPulseCounter()
    {
        if (pulseCounter >= 60)
        {
            pulseCounter += 1;
        }
    }

    public void UpPulseScream()
    {
        if (pulseCounter >= 60)
        {
            pulseCounter += 10;
        }
    }


    public void StartIncreasingPulse()
    {
        UpPulseCounter(5);
        if (pulseIncreaseCoroutine == null) // Проверяем, не запущена ли уже корутина
        {
            pulseIncreaseCoroutine = StartCoroutine(IncreasePulseCoroutine());
        }
    }

    public void StopIncreasingPulse()
    {
        if (pulseIncreaseCoroutine != null)
        {
            StopCoroutine(pulseIncreaseCoroutine);
            pulseIncreaseCoroutine = null; // Сбрасываем ссылку на корутину
        }
    }

    private IEnumerator IncreasePulseCoroutine()
    {
         yield return new WaitForSeconds(1f); // Ждем 1 секунду
        UpPulseCounter(5); // Увеличиваем пульс на 5

        StopIncreasingPulse(); // Останавливаем корутину после завершения
    }

    public void UpPulseCounter(int amount = 1)
    {
        pulseCounter += amount; // Увеличиваем пульс без проверки на 60
        textMeshPro.text = pulseCounter.ToString(); // Обновляем текст
    }

}
