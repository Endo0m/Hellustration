using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightningEffect : MonoBehaviour
{
    // Ссылки на UI-изображения
    [SerializeField] private Image lightningImage1;
    [SerializeField] private Image lightningImage2;
    [SerializeField] private Image lightningImage3;

    // Диапазон времени задержки
    [SerializeField] private float minDelay = 5f;
    [SerializeField] private float maxDelay = 13f;

    // Звук молнии
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip lightningSound;

    private void Start()
    {
        // Запускаем корутину для имитации молнии
        StartCoroutine(LightningCycle());
    }

    private IEnumerator LightningCycle()
    {
        while (true)
        {
            // Генерируем случайное время задержки
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            // Проигрываем звук молнии
            if (lightningSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(lightningSound);
            }

            // Включаем первую картинку, выключаем вторую
            lightningImage1.gameObject.SetActive(true);
            lightningImage2.gameObject.SetActive(false);
            lightningImage3.gameObject.SetActive(false);

            // Короткая задержка для имитации вспышки молнии
            yield return new WaitForSeconds(0.2f);

            // Выключаем первую картинку, включаем вторую
            lightningImage1.gameObject.SetActive(false);
            lightningImage2.gameObject.SetActive(true);
            lightningImage3.gameObject.SetActive(true);

            // Ещё одна короткая задержка для завершения эффекта
            yield return new WaitForSeconds(0.2f);

            // Снова отключаем обе картинки до следующей молнии
            lightningImage1.gameObject.SetActive(true);

            lightningImage3.gameObject.SetActive(false);
            lightningImage2.gameObject.SetActive(false);
        }
    }
}
