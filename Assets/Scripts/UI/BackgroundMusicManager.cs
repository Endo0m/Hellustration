using System.Collections;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Музыкальные треки")]
    public AudioClip[] musicTracks; // Массив с музыкальными треками

    [Header("Настройки воспроизведения")]
    public bool playRandomly = false; // Выбор порядка воспроизведения: случайно или по порядку

    private AudioSource audioSource; // Источник аудио
    private int currentTrackIndex = 0; // Индекс текущего трека

    void Start()
    {
        // Получаем компонент AudioSource на объекте
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Если AudioSource не добавлен, добавляем его
        }

        // Проверка, что массив треков не пустой
        if (musicTracks.Length > 0)
        {
            PlayNextTrack(); // Начинаем воспроизведение первого трека
        }
        else
        {
            Debug.LogWarning("Нет доступных музыкальных треков для воспроизведения.");
        }
    }

    void Update()
    {
        // Проверка, закончилось ли воспроизведение текущего трека
        if (!audioSource.isPlaying)
        {
            PlayNextTrack(); // Если текущий трек закончился, начинаем воспроизведение следующего
        }
    }

    // Метод для выбора следующего трека
    void PlayNextTrack()
    {
        if (musicTracks.Length == 0) return;

        if (playRandomly)
        {
            // Воспроизведение случайного трека
            currentTrackIndex = Random.Range(0, musicTracks.Length);
        }
        else
        {
            // Воспроизведение следующего трека по порядку
            currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        }

        // Воспроизведение трека
        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.Play();
    }
}
