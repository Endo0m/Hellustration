using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private GameObject transitionTrigger;
    [SerializeField] private float autoCloseDelay = 3f;
    private bool isOpen = false;

    private void Start()
    {
        if (transitionTrigger != null)
        {
            transitionTrigger.SetActive(false);
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
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        doorAnimator.SetTrigger("OpenDoor");
        if (transitionTrigger != null)
        {
            transitionTrigger.SetActive(true);
        }

        StartCoroutine(AutoCloseDoor());
    }

    private void CloseDoor()
    {
        isOpen = false;
        doorAnimator.SetTrigger("CloseDoor");
        if (transitionTrigger != null)
        {
            transitionTrigger.SetActive(false);
        }
    }

    private IEnumerator AutoCloseDoor()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        CloseDoor();
    }
}
