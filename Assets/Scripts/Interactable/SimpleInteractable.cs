using UnityEngine;

public class SimpleInteractable : MonoBehaviour
{
    [SerializeField] private Animator animator; // Аниматор для управления анимацией
    [SerializeField] private string animationTrigger = "Wobble"; // Название триггера для анимации
    [SerializeField] private AudioClip[] audioClips; // Массив звуков
    [SerializeField] private AudioSource audioSource; // Источник звука для воспроизведения аудио

    private int currentAudioIndex = 0; // Текущий индекс звука для проигрывания

    private void OnMouseDown()
    {
        // Проверяем наличие анимации и запускаем её
        if (animator != null)
        {
            animator.SetTrigger(animationTrigger);
        }

        // Проигрываем звук из массива по индексу
        PlayAudio();
    }

    private void PlayAudio()
    {
        if (audioClips != null && audioClips.Length > 0 && audioSource != null)
        {
            // Проигрываем текущий звук
            audioSource.clip = audioClips[currentAudioIndex];
            audioSource.Play();

            // Переходим к следующему звуку
            currentAudioIndex = (currentAudioIndex + 1) % audioClips.Length; // Циклическое переключение индекса
        }
    }
}
