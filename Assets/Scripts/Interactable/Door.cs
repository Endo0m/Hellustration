using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator doorAnimator; // Аниматор для управления анимацией двери
    [SerializeField] private GameObject transitionTrigger; // Ссылка на триггер для перехода между листами
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
            CloseDoor();
        }
        else
        {
            OpenDoor();
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
}
