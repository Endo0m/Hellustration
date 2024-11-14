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
        if (pulseCounter > 80)
        {
            playerPulseUI.GameOver();
        }
    }

    // public IEnumerator PulseCoroutine()
    // {
    //     //DecreasePulseCounterr();
    //     playerPulseUI.StartPulse();
    //     yield return new WaitForSeconds(5f); // ждем 5 секунд
    //     playerPulseUI.StopPulse();
    // }

    public void RestorePulse()
    {
        if (pulseCounter > 60)
        {
            pulseCounter -= 1;
        }
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
}
