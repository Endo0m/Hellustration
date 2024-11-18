using UnityEngine;

using System.Collections;

public class Box : MonoBehaviour
{
    [SerializeField] private Animator boxAnimator; // Аниматор для управления анимацией коробки
    [SerializeField] private GameObject[] items; // Массив объектов, которые будут активироваться
    [SerializeField] private float autoCloseDelay = 5f; // Задержка перед автоматическим закрытием коробки

    [SerializeField] private string openSoundKey; // Ключ для звука открытия
    [SerializeField] private string closeSoundKey; // Ключ для звука закрытия
    [SerializeField] private AudioSource audioSource; // AudioSource для воспроизведения звуков

    private bool isOpen = false; // Флаг состояния коробки
    private Coroutine autoCloseCoroutine; // Переменная для хранения корутины автоматического закрытия

    public void Interact(GameObject interactor)
    {
        if (isOpen)
        {
            CloseBox();
        }
        else
        {
            OpenBox();
        }
    }

    private void OpenBox()
    {
        isOpen = true;
        boxAnimator.SetTrigger("OpenBox");

        // Воспроизводим звук открытия
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(openSoundKey, audioSource);
        }

        // Активируем объекты в массиве
        foreach (GameObject item in items)
        {
            if (item != null)
            {
                item.SetActive(true);
            }
        }

        // Останавливаем существующую корутину, если она активна
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
        }

        // Запускаем новую корутину для автоматического закрытия
        autoCloseCoroutine = StartCoroutine(AutoCloseBox());
    }

    private void CloseBox()
    {
        isOpen = false;
        boxAnimator.SetTrigger("CloseBox");

        // Воспроизводим звук закрытия
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(closeSoundKey, audioSource);
        }

        // Деактивируем объекты в массиве, если они ещё не были забраны
        foreach (GameObject item in items)
        {
            if (item != null && item.activeInHierarchy)
            {
                item.SetActive(false);
            }
        }

        // Останавливаем корутину, если коробка закрывается вручную
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
            autoCloseCoroutine = null;
        }
    }

    private IEnumerator AutoCloseBox()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        CloseBox();
    }
}
