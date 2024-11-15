using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour
{
    [SerializeField] private Animator boxAnimator; // Аниматор для управления анимацией коробки
    [SerializeField] private GameObject[] items; // Массив объектов, которые будут активироваться
    [SerializeField] private float autoCloseDelay = 5f; // Задержка перед автоматическим закрытием коробки

    private bool isOpen = false; // Флаг состояния коробки

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

        // Активируем объекты в массиве
        foreach (GameObject item in items)
        {
            if (item != null)
            {
                item.SetActive(true);
            }
        }

        // Начинаем корутину для автоматического закрытия
        StartCoroutine(AutoCloseBox());
    }

    private void CloseBox()
    {
        isOpen = false;
        boxAnimator.SetTrigger("CloseBox");

        // Деактивируем объекты в массиве, если они ещё не были забраны
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
