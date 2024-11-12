using UnityEngine;

public class ItemLifespan : MonoBehaviour
{
    public float lifespan = 5f; // Время жизни предмета в секундах
    private bool isDestroyed = false; // Флаг для предотвращения повторного уничтожения

    private void Start()
    {
        // Запуск таймера для уничтожения объекта
        Invoke(nameof(DestroyItem), lifespan);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверка на вхождение врага в триггерный коллайдер
        if (!isDestroyed && other.CompareTag("Enemy"))
        {
            DestroyItem();
        }
    }

    private void DestroyItem()
    {
        if (!isDestroyed)
        {
            Destroy(gameObject);
            isDestroyed = true; // Установка флага для предотвращения повторного уничтожения
        }
    }
}
