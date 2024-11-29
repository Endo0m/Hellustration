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
            // ������ ������ �����
            currentFrameTime = Time.realtimeSinceStartup;

            // ���� ���������� ���� Update() � LateUpdate()
            yield return new WaitForEndOfFrame();

            // ���������, ������� ������� ����� ���������
            float timeToWait = (1f / targetFrameRate) - (Time.realtimeSinceStartup - currentFrameTime);

            if (timeToWait > 0)
            {
                // ���� ���������� �����
                yield return new WaitForSecondsRealtime(timeToWait);
            }
        }
    }
}