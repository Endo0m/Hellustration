using UnityEngine;

public class FPSLocker : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
}