using System.Collections;
using UnityEngine;

public class ScreamerTrigger : MonoBehaviour
{
    [SerializeField] private GameObject screamerPrefab; // Префаб скримера
    [SerializeField] private Transform spawnPoint; // Точка создания скримера
    [SerializeField] private float screamerLifetime = 3.0f; // Время жизни скримера в секундах
    private bool hasTriggered = false; // Флаг для отслеживания срабатывания

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, если объект, вошедший в триггер, является игроком и скрипт еще не срабатывал
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // Устанавливаем флаг, чтобы скрипт больше не срабатывал
            StartCoroutine(SpawnScreamer());
        }
    }

    private IEnumerator SpawnScreamer()
    {
        // Если не задана точка создания, используем позицию триггера
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        // Создаем скример
        GameObject screamer = Instantiate(screamerPrefab, spawnPosition, spawnRotation);

        // Ждем заданное время
        yield return new WaitForSeconds(screamerLifetime);

        // Удаляем скример
        if (screamer != null)
        {
            Destroy(screamer);
        }
    }
}
