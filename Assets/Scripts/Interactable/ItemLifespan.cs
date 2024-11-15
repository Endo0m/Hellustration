using UnityEngine;

public class ItemLifespan : MonoBehaviour
{
    public float lifespan = -1f; // Время жизни предмета (-1 означает, что предмет не исчезает по времени)
    private bool isDestroyed = false; // Флаг, предотвращающий повторное уничтожение
    public int requiredWaypoint = 1; // Точка, которую враг должен пройти для уничтожения предмета

    private void Start()
    {
        // Если lifespan больше 0, запускаем таймер на уничтожение
        if (lifespan > 0)
        {
            Invoke(nameof(DestroyItem), lifespan);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDestroyed && other.CompareTag("Enemy"))
        {
            EnemyBase enemyBase = other.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                Debug.Log($"Enemy reached waypoint {enemyBase.CurrentWaypoint}. Required waypoint: {requiredWaypoint}");
                if (enemyBase.CurrentWaypoint >= requiredWaypoint)
                {
                    DestroyItem();
                }
            }
        }
    }


    private void DestroyItem()
    {
        if (!isDestroyed)
        {
            Destroy(gameObject);
            isDestroyed = true;
        }
    }
}
