using UnityEngine;
using UnityEngine.UI;

public class UIFluctuateFill : MonoBehaviour
{
    public Image targetImage; // Ссылка на Image, для которой изменяется fillAmount
    public float speed = 1f; // Скорость изменения fillAmount
    private float direction = 1f; // Направление изменения: 1 - вперед, -1 - назад

    private void Update()
    {
        if (targetImage != null)
        {
            // Изменение fillAmount с учетом направления и скорости
            targetImage.fillAmount += direction * speed * Time.deltaTime;

            // Проверка на границы (от 0 до 1) и смена направления
            if (targetImage.fillAmount >= 1f)
            {
                targetImage.fillAmount = 1f;
                direction = -1f; // Меняем направление на обратное
            }
            else if (targetImage.fillAmount <= 0f)
            {
                targetImage.fillAmount = 0f;
                direction = 1f; // Меняем направление на обратное
            }
        }
    }
}
