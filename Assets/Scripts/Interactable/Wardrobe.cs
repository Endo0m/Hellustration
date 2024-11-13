using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wardrobe : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private GameObject[] questObjects;
    private bool isOpen = false;

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
      
    }

    private void OpenDoor()
    {
        isOpen = true;
        doorAnimator.SetTrigger("OpenDoor");

        // Включаем все объекты из массива questObjects
        foreach (GameObject questObject in questObjects)
        {
            if (questObject != null)
            {
                questObject.SetActive(true);
            }
        }
    }

    private void CloseDoor()
    {
        isOpen = false;
        doorAnimator.SetTrigger("CloseDoor");

        // Отключаем все объекты из массива questObjects
        foreach (GameObject questObject in questObjects)
        {
            if (questObject != null)
            {
                questObject.SetActive(false);
            }
        }
    }
}
