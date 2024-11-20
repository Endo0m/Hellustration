using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private GameObject[] transitionTriggers;
    [SerializeField] private float autoCloseDelay = 3f;
    private bool isOpen = false;

    private void Start()
    {
        // ��������� ��� �������� ��� �������
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

        // ������ ������ �������������� �� ������ �������
        if (interactor.CompareTag("Player"))
        {
            Debug.Log("����� ��������������� � ������");
        }
        else if (interactor.CompareTag("Enemy"))
        {
            Debug.Log("���� ��������������� � ������");
        }
    }

    private void OpenDoor()
    {
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
