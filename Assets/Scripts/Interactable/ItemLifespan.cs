using UnityEngine;

public class ItemLifespan : MonoBehaviour
{
    public float lifespan = 5f; // Время жизни предмета в секундах

    private void Start()
    {
        Destroy(gameObject, lifespan); // Уничтожение объекта через указанное время
    }
}
