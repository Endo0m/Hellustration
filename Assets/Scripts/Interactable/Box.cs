using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour
{
    [SerializeField] private Animator boxAnimator; // �������� ��� ���������� ��������� �������
    [SerializeField] private GameObject[] items; // ������ ��������, ������� ����� ��������������
    [SerializeField] private float autoCloseDelay = 5f; // �������� ����� �������������� ��������� �������

    private bool isOpen = false; // ���� ��������� �������

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

        // ���������� ������� � �������
        foreach (GameObject item in items)
        {
            if (item != null)
            {
                item.SetActive(true);
            }
        }

        // �������� �������� ��� ��������������� ��������
        StartCoroutine(AutoCloseBox());
    }

    private void CloseBox()
    {
        isOpen = false;
        boxAnimator.SetTrigger("CloseBox");

        // ������������ ������� � �������, ���� ��� ��� �� ���� �������
        foreach (GameObject item in items)
        {
            if (item != null && item.activeInHierarchy)
            {
                item.SetActive(false);
            }
        }
    }

    private IEnumerator AutoCloseBox()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        CloseBox();
    }
}
