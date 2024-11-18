using UnityEngine;

using System.Collections;

public class Box : MonoBehaviour
{
    [SerializeField] private Animator boxAnimator; // �������� ��� ���������� ��������� �������
    [SerializeField] private GameObject[] items; // ������ ��������, ������� ����� ��������������
    [SerializeField] private float autoCloseDelay = 5f; // �������� ����� �������������� ��������� �������

    [SerializeField] private string openSoundKey; // ���� ��� ����� ��������
    [SerializeField] private string closeSoundKey; // ���� ��� ����� ��������
    [SerializeField] private AudioSource audioSource; // AudioSource ��� ��������������� ������

    private bool isOpen = false; // ���� ��������� �������
    private Coroutine autoCloseCoroutine; // ���������� ��� �������� �������� ��������������� ��������

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

        // ������������� ���� ��������
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(openSoundKey, audioSource);
        }

        // ���������� ������� � �������
        foreach (GameObject item in items)
        {
            if (item != null)
            {
                item.SetActive(true);
            }
        }

        // ������������� ������������ ��������, ���� ��� �������
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
        }

        // ��������� ����� �������� ��� ��������������� ��������
        autoCloseCoroutine = StartCoroutine(AutoCloseBox());
    }

    private void CloseBox()
    {
        isOpen = false;
        boxAnimator.SetTrigger("CloseBox");

        // ������������� ���� ��������
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(closeSoundKey, audioSource);
        }

        // ������������ ������� � �������, ���� ��� ��� �� ���� �������
        foreach (GameObject item in items)
        {
            if (item != null && item.activeInHierarchy)
            {
                item.SetActive(false);
            }
        }

        // ������������� ��������, ���� ������� ����������� �������
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
