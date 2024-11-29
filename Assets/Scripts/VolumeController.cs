using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public static VolumeController Instance { get; private set; }
    private Slider currentVolumeSlider;
    private const string VOLUME_SLIDER_NAME = "VolumeSlider"; // Имя слайдера для поиска

    private void Awake()
    {
        // Реализация синглтона
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Подписываемся на событие загрузки сцены
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Ищем слайдер на новой сцене
        FindAndInitializeSlider();
    }

    private void FindAndInitializeSlider()
    {
        // Находим слайдер по имени
        GameObject sliderObject = GameObject.Find(VOLUME_SLIDER_NAME);
        if (sliderObject != null)
        {
            currentVolumeSlider = sliderObject.GetComponent<Slider>();
            if (currentVolumeSlider != null)
            {
                // Устанавливаем текущее значение громкости
                currentVolumeSlider.value = AudioListener.volume;
                // Подписываемся на изменение значения слайдера
                currentVolumeSlider.onValueChanged.RemoveAllListeners();
                currentVolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            }
        }
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    // Метод для получения текущей громкости (может пригодиться)
    public float GetCurrentVolume()
    {
        return AudioListener.volume;
    }
}