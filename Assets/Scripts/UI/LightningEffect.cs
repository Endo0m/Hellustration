using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightningEffect : MonoBehaviour
{
    // ������ �� UI-�����������
    [SerializeField] private Image lightningImage1;
    [SerializeField] private Image lightningImage2;
    [SerializeField] private Image lightningImage3;

    // �������� ������� ��������
    [SerializeField] private float minDelay = 5f;
    [SerializeField] private float maxDelay = 13f;

    // ���� ������
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip lightningSound;

    private void Start()
    {
        // ��������� �������� ��� �������� ������
        StartCoroutine(LightningCycle());
    }

    private IEnumerator LightningCycle()
    {
        while (true)
        {
            // ���������� ��������� ����� ��������
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            // ����������� ���� ������
            if (lightningSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(lightningSound);
            }

            // �������� ������ ��������, ��������� ������
            lightningImage1.gameObject.SetActive(true);
            lightningImage2.gameObject.SetActive(false);
            lightningImage3.gameObject.SetActive(false);

            // �������� �������� ��� �������� ������� ������
            yield return new WaitForSeconds(0.2f);

            // ��������� ������ ��������, �������� ������
            lightningImage1.gameObject.SetActive(false);
            lightningImage2.gameObject.SetActive(true);
            lightningImage3.gameObject.SetActive(true);

            // ��� ���� �������� �������� ��� ���������� �������
            yield return new WaitForSeconds(0.2f);

            // ����� ��������� ��� �������� �� ��������� ������
            lightningImage1.gameObject.SetActive(true);

            lightningImage3.gameObject.SetActive(false);
            lightningImage2.gameObject.SetActive(false);
        }
    }
}
