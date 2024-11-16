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

    private IEnumerator PulseCoroutine()
    {
        yield return new WaitForSeconds(5f); 
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
}
