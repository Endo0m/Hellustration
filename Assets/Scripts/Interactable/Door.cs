using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator doorAnimator; // �������� ��� ���������� ��������� �����
    [SerializeField] private GameObject transitionTrigger; // ������ �� ������� ��� �������� ����� �������
    [SerializeField] private float autoCloseDelay = 3f; // �������� ����� �������������� ��������� �����
    private bool isOpen = false; // ������� ��������� �����

    private void Start()
    {
        if (transitionTrigger != null)
        {
            transitionTrigger.SetActive(false); // ��������� ������� �� ���������
        }
    }

    public void Interact(PlayerController player)
    {
        if (isOpen)
        {
            CloseDoor(); // ���� ����� �������, ��������� �
        }
        else
        {
            OpenDoor(); // ���� ����� �������, ��������� �
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        doorAnimator.SetTrigger("OpenDoor"); // ��������� �������� ��������
        if (transitionTrigger != null)
        {
            transitionTrigger.SetActive(true); // ���������� ������� ��������
        }
        Debug.Log("����� �������");

        // ��������� �������� ��� ��������������� �������� �����
        StartCoroutine(AutoCloseDoor());
    }

    private void CloseDoor()
    {
        isOpen = false;
        doorAnimator.SetTrigger("CloseDoor"); // ��������� �������� ��������
        if (transitionTrigger != null)
        {
            transitionTrigger.SetActive(false); // ������������ ������� ��������
        }
        Debug.Log("����� �������");
    }

    private IEnumerator AutoCloseDoor()
    {
        yield return new WaitForSeconds(autoCloseDelay); // ��� �������� ���������� ������
        CloseDoor(); // ��������� ����� �������������
    }
}
