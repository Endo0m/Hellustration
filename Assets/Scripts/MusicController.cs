using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource huntMusic;

    [Header("Volume Settings")]
    [SerializeField] private float maxVolume = 0.4f; // Максимальная громкость (40%)
    [SerializeField] private float fadeSpeed = 2f; // Скорость перехода

    private float currentBackgroundVolume;
    private float currentHuntVolume;
    private bool isHuntMode = false;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        // Настраиваем начальные значения
        if (backgroundMusic != null && huntMusic != null)
        {
            // Включаем обе музыки на loop
            backgroundMusic.loop = true;
            huntMusic.loop = true;

            // Устанавливаем начальную громкость
            backgroundMusic.volume = maxVolume;
            huntMusic.volume = 0f;

            currentBackgroundVolume = maxVolume;
            currentHuntVolume = 0f;

            // Запускаем проигрывание
            backgroundMusic.Play();
            huntMusic.Play();
        }
        else
        {
            Debug.LogError("Music sources not set in MusicController!");
        }
    }

    public void StartHuntMode()
    {
        if (isHuntMode) return;
        isHuntMode = true;

        // Останавливаем предыдущий корутин если он был
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeMusic(true));
    }

    public void StopHuntMode()
    {
        if (!isHuntMode) return;
        isHuntMode = false;

        // Останавливаем предыдущий корутин если он был
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeMusic(false));
    }

    private IEnumerator FadeMusic(bool toHuntMode)
    {
        float startBackgroundVolume = backgroundMusic.volume;
        float startHuntVolume = huntMusic.volume;
        float targetBackgroundVolume = toHuntMode ? 0f : maxVolume;
        float targetHuntVolume = toHuntMode ? maxVolume : 0f;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            float t = Mathf.Clamp01(elapsed);

            // Используем плавную интерполяцию
            backgroundMusic.volume = Mathf.Lerp(startBackgroundVolume, targetBackgroundVolume, t);
            huntMusic.volume = Mathf.Lerp(startHuntVolume, targetHuntVolume, t);

            currentBackgroundVolume = backgroundMusic.volume;
            currentHuntVolume = huntMusic.volume;

            yield return null;
        }

        // Убеждаемся, что достигли целевых значений
        backgroundMusic.volume = targetBackgroundVolume;
        huntMusic.volume = targetHuntVolume;

        currentBackgroundVolume = backgroundMusic.volume;
        currentHuntVolume = huntMusic.volume;
    }

#if UNITY_EDITOR
    // Добавляем визуализацию текущих уровней громкости в редакторе
    private void OnGUI()
    {
        if (!Application.isPlaying) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 100));
        GUILayout.Label($"Background Music Volume: {currentBackgroundVolume:F2}");
        GUILayout.Label($"Hunt Music Volume: {currentHuntVolume:F2}");
        GUILayout.Label($"Hunt Mode Active: {isHuntMode}");
        GUILayout.EndArea();
    }
#endif

    private void OnValidate()
    {
        // Ограничиваем значения в инспекторе
        maxVolume = Mathf.Clamp01(maxVolume);
        fadeSpeed = Mathf.Max(0.1f, fadeSpeed);
    }
}