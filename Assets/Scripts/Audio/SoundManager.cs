using UnityEngine;

// Структура для хранения имени и звука
[System.Serializable]
public struct Sound
{
    public string name;  // Имя звука
    public AudioClip clip; // Ссылка на аудиофайл
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds; // Массив звуков

    // Singleton для удобного доступа
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        // Убедимся, что у нас только один экземпляр SoundManager
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Сохраняем объект между сценами
        }
    }

    // Метод для вызова звука по имени
    public void PlaySound(string soundName)
    {
        Sound sound = System.Array.Find(sounds, s => s.name == soundName);

        if (sound.clip != null)
        {
            // Создание нового GameObject для аудио источника
            GameObject soundGameObject = new GameObject("AudioSource_" + soundName);
            AudioSource newAudioSource = soundGameObject.AddComponent<AudioSource>();

            // Устанавливаем настройки источника и проигрываем звук
            newAudioSource.clip = sound.clip;
            newAudioSource.Play();

            // Удаляем объект после окончания воспроизведения звука
            Destroy(soundGameObject, sound.clip.length);
        }
        else
        {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }
}
