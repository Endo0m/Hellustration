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
    private AudioSource audioSource; // Источник для воспроизведения звуков

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

        audioSource = GetComponent<AudioSource>();
    }

    // Метод для вызова звука по имени
    public void PlaySound(string soundName)
    {
        Sound sound = System.Array.Find(sounds, s => s.name == soundName);

        if (sound.clip != null)
        {
            audioSource.PlayOneShot(sound.clip); // Воспроизведение звука
        }
        else
        {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }
}
