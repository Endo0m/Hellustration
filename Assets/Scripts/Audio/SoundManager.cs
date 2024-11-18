using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Статическая переменная для реализации Singleton
    public static SoundManager Instance { get; private set; }

    // Структура для хранения ключа и аудиоклипа
    [System.Serializable]
    public struct Sound
    {
        public string key; // Имя ключа
        public AudioClip clip; // Аудиоклип
    }

    public List<Sound> sounds; // Список всех звуков

    // Словарь для быстрого поиска по ключу
    private Dictionary<string, AudioClip> soundDictionary;

    private void Awake()
    {
        // Реализация Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Сохраняем объект при смене сцен (опционально)
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Удаляем дублирующий экземпляр
            return;
        }

        // Инициализация словаря
        soundDictionary = new Dictionary<string, AudioClip>();
        foreach (Sound sound in sounds)
        {
            if (!soundDictionary.ContainsKey(sound.key))
            {
                soundDictionary.Add(sound.key, sound.clip);
            }
            else
            {
                Debug.LogWarning($"Звук с ключом '{sound.key}' уже существует в словаре.");
            }
        }
    }

    // Метод для получения аудиоклипа по ключу
    public AudioClip GetAudioClip(string key)
    {
        if (soundDictionary.TryGetValue(key, out AudioClip clip))
        {
            return clip;
        }
        else
        {
            Debug.LogWarning($"Аудиоклип с ключом '{key}' не найден.");
            return null;
        }
    }

    // Метод для воспроизведения аудиоклипа через переданный AudioSource
    public void PlaySound(string key, AudioSource audioSource)
    {
        AudioClip clip = GetAudioClip(key);
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
