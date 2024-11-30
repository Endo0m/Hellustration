using UnityEngine;
using DG.Tweening;

public class FadeOutDestroyer : MonoBehaviour
{
    [SerializeField] private float delayBeforeFade = 2f; // Задержка перед началом исчезновения
    [SerializeField] private float fadeDuration = 1f; // Длительность анимации исчезновения
    [SerializeField] private SpriteRenderer spriteRenderer; // Для 2D спрайтов

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Последовательность анимаций
        Sequence sequence = DOTween.Sequence();

        // Ждем заданное время
        sequence.AppendInterval(delayBeforeFade);

        // Плавно меняем прозрачность до 0
        sequence.Append(spriteRenderer.DOFade(0f, fadeDuration));

        // Уничтожаем объект после завершения анимации
        sequence.OnComplete(() => Destroy(gameObject));
    }

    private void OnDestroy()
    {
        // Убираем анимацию при уничтожении объекта
        DOTween.Kill(spriteRenderer);
    }
}