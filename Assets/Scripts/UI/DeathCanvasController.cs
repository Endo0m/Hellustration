using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeathCanvasController : MonoBehaviour
{
    [SerializeField] private Image[] imagesToDisplay; // Массив изображений для анимации
    [SerializeField] private float fillDuration = 1f; // Время, за которое изображение заполняется
    [SerializeField] private float timeBetweenImages = 0.5f; // Задержка между изображениями
    [SerializeField] private float timeBeforeMenu = 2f; // Время перед переходом к меню
    [SerializeField] private Canvas mainCanvas; // Ссылка на основной Canvas для отключения

    private void OnEnable()
    {
        StartCoroutine(DisplayImagesSequence());
    }

    private IEnumerator DisplayImagesSequence()
    {
        // Отключение основного Canvas
        if (mainCanvas != null)
        {
            mainCanvas.gameObject.SetActive(false);
        }

        foreach (Image img in imagesToDisplay)
        {
            img.fillAmount = 0f; // Устанавливаем начальное состояние
            img.gameObject.SetActive(true); // Активируем изображение

            // Плавное изменение fillAmount
            for (float t = 0; t < fillDuration; t += Time.deltaTime)
            {
                float normalizedTime = t / fillDuration;
                img.fillAmount = Mathf.Lerp(0f, 1f, normalizedTime);
                yield return null; // Ждем следующий кадр
            }

            // Устанавливаем окончательный fillAmount на 1
            img.fillAmount = 1f;

            yield return new WaitForSeconds(timeBetweenImages);
        }

        // Задержка перед переходом к главному меню
        yield return new WaitForSeconds(timeBeforeMenu);

        // Переход к сцене главного меню
        SceneManager.LoadScene("MainMenu");
    }
}
