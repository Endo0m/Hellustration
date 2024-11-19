using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Went : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // Скорость движения к игроку
    [SerializeField] private float stopDistance = 0.1f; // Расстояние, на котором объект будет удален

    private Transform playerTransform;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Игрок не найден! Убедитесь, что объект с тегом 'Player' существует.");
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Направление к игроку
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Перемещение к игроку
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);

        // Проверка на достижение игрока
        if (Vector2.Distance(transform.position, playerTransform.position) <= stopDistance)
        {
            Destroy(gameObject); // Удаляем объект
        }
    }
}
