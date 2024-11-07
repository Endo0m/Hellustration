using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator doorAnimator; // Аниматор для управления анимацией двери
    [SerializeField] private GameObject transitionTrigger; // Ссылка на триггер для перехода между листами
    [SerializeField] private float autoCloseDelay = 3f; // Задержка перед автоматическим закрытием двери
    private bool isOpen = false; // Текущее состояние двери

    private void Start()
    {
        if (transitionTrigger != null)
        {
            transitionTrigger.SetActive(false); // Отключаем триггер по умолчанию
        }
    }

    public void Interact(PlayerController player)
    {
        if (isOpen)
        {
            CloseDoor(); // Если дверь открыта, закрываем её
        }
        else
        {
            OpenDoor(); // Если дверь закрыта, открываем её
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        doorAnimator.SetTrigger("OpenDoor"); // Запускаем анимацию открытия
        if (transitionTrigger != null)
        {
            transitionTrigger.SetActive(true); // Активируем триггер перехода
        }
        Debug.Log("Дверь открыта");

        // Запускаем корутину для автоматического закрытия двери
        StartCoroutine(AutoCloseDoor());
    }

    private void CloseDoor()
    {
        isOpen = false;
        doorAnimator.SetTrigger("CloseDoor"); // Запускаем анимацию закрытия
        if (transitionTrigger != null)
        {
            transitionTrigger.SetActive(false); // Деактивируем триггер перехода
        }
        Debug.Log("Дверь закрыта");
    }

    private IEnumerator AutoCloseDoor()
    {
        yield return new WaitForSeconds(autoCloseDelay); // Ждём заданное количество секунд
        CloseDoor(); // Закрываем дверь автоматически
    }
}
