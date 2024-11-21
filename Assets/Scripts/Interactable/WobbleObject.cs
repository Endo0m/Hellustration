using UnityEngine;

public class WobbleObject : MonoBehaviour
{
    [SerializeField] private Animator animator; // Аниматор для управления покачиванием
    [SerializeField] private GameObject itemToDrop; // Предмет, который выпадет
    [SerializeField] private Transform dropPoint; // Точка, в которой выпадет предмет
    [SerializeField] private Vector2 forceDirection = new Vector2(1f, 1f); // Направление толчка
    [SerializeField] private float forceStrength = 5f; // Сила толчка
    [SerializeField] private string openSoundKey; // Ключ для звука открытия
    private AudioSource audioSource;
    private bool hasDroppedItem = false; // Флаг, предотвращающий повторное выпадение

    public void Interact(GameObject interactor)
    {
        if (hasDroppedItem) return; // Предотвращаем повторное взаимодействие
        SoundManager.Instance.PlaySound(openSoundKey, audioSource);

        // Запуск анимации покачивания
        if (animator != null)
        {
            animator.SetTrigger("Wobble");
        }

        // Выпадение предмета
        DropItem();
    }

    private void DropItem()
    {
        if (itemToDrop != null && !hasDroppedItem)
        {
            // Создаем экземпляр предмета
            GameObject droppedItem = Instantiate(itemToDrop, dropPoint != null ? dropPoint.position : transform.position, Quaternion.identity);

            // Применение силы к Rigidbody2D, если он есть
            Rigidbody2D rb2D = droppedItem.GetComponent<Rigidbody2D>();
            if (rb2D != null)
            {
                rb2D.AddForce(forceDirection.normalized * forceStrength, ForceMode2D.Impulse);
            }
            else
            {
                // Применение силы к Rigidbody (для 3D)
                Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(new Vector3(forceDirection.x, forceDirection.y, 0f).normalized * forceStrength, ForceMode.Impulse);
                }
            }

            hasDroppedItem = true; // Устанавливаем флаг после первого выпадения
        }
    }

    // Добавление гизмо для направления толчка
    private void OnDrawGizmosSelected()
    {
        if (dropPoint == null) return;

        // Устанавливаем цвет для гизмо
        Gizmos.color = Color.red;

        // Начальная точка - точка падения или текущая позиция
        Vector3 startPoint = dropPoint.position;
        Vector3 endPoint = startPoint + new Vector3(forceDirection.x, forceDirection.y, 0f).normalized * forceStrength;

        // Рисуем стрелку
        Gizmos.DrawLine(startPoint, endPoint);
        Gizmos.DrawSphere(endPoint, 0.1f); // Маленький кружок на конце стрелки
    }
}
