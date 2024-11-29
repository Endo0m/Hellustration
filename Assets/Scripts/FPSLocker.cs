using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLocker : MonoBehaviour
{
    public int targetFrameRate = 60;

    private float currentFrameTime;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }

    void Start()
    {
        StartCoroutine(WaitForNextFrame());
    }

    IEnumerator WaitForNextFrame()
    {
        while (true)
        {
            // Начало нового кадра
            currentFrameTime = Time.realtimeSinceStartup;

            // Ждем завершения всех Update() и LateUpdate()
            yield return new WaitForEndOfFrame();

            // Вычисляем, сколько времени нужно подождать
            float timeToWait = (1f / targetFrameRate) - (Time.realtimeSinceStartup - currentFrameTime);

            if (timeToWait > 0)
            {
                // Ждем оставшееся время
                yield return new WaitForSecondsRealtime(timeToWait);
            }
        }
    }
}