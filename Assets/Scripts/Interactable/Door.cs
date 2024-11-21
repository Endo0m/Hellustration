using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject[] transitionTriggers;
    [SerializeField] private float autoCloseDelay = 3f;
    [SerializeField] private string openSoundKey; // Ключ для звука открытия
    [SerializeField] private string closeSoundKey; // Ключ для звука закрытия
    private AudioSource audioSource;
    private Animator doorAnimator;
    private bool isOpen = false;

    private void Start()
    {
        doorAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        // Отключаем все триггеры при запуске
        if (transitionTriggers != null)
        {
            foreach (var trigger in transitionTriggers)
            {
                if (trigger != null)
                {
                    trigger.SetActive(false);
                }
            }
        }
    }

    public void Interact(GameObject interactor)
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }

        // Пример логики взаимодействия на основе объекта
        if (interactor.CompareTag("Player"))
        {
            Debug.Log("Игрок взаимодействует с дверью");
        }
        else if (interactor.CompareTag("Enemy"))
        {
            Debug.Log("Враг взаимодействует с дверью");
        }
    }

    private void OpenDoor()
    {
        SoundManager.Instance.PlaySound(openSoundKey, audioSource);

        isOpen = true;
        doorAnimator.SetTrigger("OpenDoor");
        if (transitionTriggers != null)
        {
            foreach (var trigger in transitionTriggers)
            {
                if (trigger != null)
                {
                    trigger.SetActive(true);
                }
            }
        }

        StartCoroutine(AutoCloseDoor());
    }

    private void CloseDoor()
    {
        SoundManager.Instance.PlaySound(closeSoundKey, audioSource);

        isOpen = false;
        doorAnimator.SetTrigger("CloseDoor");
        if (transitionTriggers != null)
        {
            foreach (var trigger in transitionTriggers)
            {
                if (trigger != null)
                {
                    trigger.SetActive(false);
                }
            }
        }
    }

    private IEnumerator AutoCloseDoor()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        CloseDoor();
    }
}
