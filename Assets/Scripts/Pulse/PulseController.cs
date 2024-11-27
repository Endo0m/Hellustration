using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PulseController : MonoBehaviour
{
    [SerializeField] private PlayerPulseUI playerPulseUI;
    [SerializeField] private float calmDownDelay = 3f;

    private Coroutine calmDownCoroutine;
    private bool isScared = false;
    private bool enemyInSight = false;

    private void Start()
    {
        // Запускаем нормальное состояние при старте
        playerPulseUI.SetNormalState();
    }

    // Вызывается для скримера
    public void Panic()
    {
        isScared = true;
        if (calmDownCoroutine != null)
        {
            StopCoroutine(calmDownCoroutine);
        }

        playerPulseUI.SetPanicState();
        calmDownCoroutine = StartCoroutine(ScareRoutine());
    }

    // Вызывается при обнаружении врага
    public void EnemyDetected()
    {
        enemyInSight = true;
        playerPulseUI.SetPanicState();
    }

    // Вызывается когда враг пропадает из виду
    public void EnemyLost()
    {
        enemyInSight = false;
        if (!isScared) // Возвращаемся к нормальному состоянию только если нет активного испуга
        {
            playerPulseUI.SetNormalState();
        }
    }

    private System.Collections.IEnumerator ScareRoutine()
    {
        yield return new WaitForSeconds(calmDownDelay);
        isScared = false;

        // Возвращаемся к нормальному состоянию только если нет врага в поле зрения
        if (!enemyInSight)
        {
            playerPulseUI.SetNormalState();
        }
    }
}
