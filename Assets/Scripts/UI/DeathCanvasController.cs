using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeathCanvasController : MonoBehaviour
{
    [SerializeField] private Image[] imagesToDisplay; // ������ ����������� ��� ��������
    [SerializeField] private float fillDuration = 1f; // �����, �� ������� ����������� �����������
    [SerializeField] private float timeBetweenImages = 0.5f; // �������� ����� �������������
    [SerializeField] private float timeBeforeMenu = 2f; // ����� ����� ��������� � ����
    [SerializeField] private Canvas mainCanvas; // ������ �� �������� Canvas ��� ����������

    private void OnEnable()
    {
        StartCoroutine(DisplayImagesSequence());
    }

    private IEnumerator DisplayImagesSequence()
    {
        // ���������� ��������� Canvas
        if (mainCanvas != null)
        {
            mainCanvas.gameObject.SetActive(false);
        }

        foreach (Image img in imagesToDisplay)
        {
            img.fillAmount = 0f; // ������������� ��������� ���������
            img.gameObject.SetActive(true); // ���������� �����������

            // ������� ��������� fillAmount
            for (float t = 0; t < fillDuration; t += Time.deltaTime)
            {
                float normalizedTime = t / fillDuration;
                img.fillAmount = Mathf.Lerp(0f, 1f, normalizedTime);
                yield return null; // ���� ��������� ����
            }

            // ������������� ������������� fillAmount �� 1
            img.fillAmount = 1f;

            yield return new WaitForSeconds(timeBetweenImages);
        }

        // �������� ����� ��������� � �������� ����
        yield return new WaitForSeconds(timeBeforeMenu);

        // ������� � ����� �������� ����
        SceneManager.LoadScene("MainMenu");
    }
}
