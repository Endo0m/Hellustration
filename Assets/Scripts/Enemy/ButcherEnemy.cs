using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButcherEnemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    [SerializeField] private WaypointData[] waypoints; // Массив точек для патрулирования
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private ParticleSystem actionParticles;

    private int currentWaypointIndex = 0;
    private bool isMoving = true;
    [SerializeField] private List<string> requiredItems; // Список имен необходимых предметов
    private HashSet<string> collectedItems = new HashSet<string>(); // Список собранных предметов

    [System.Serializable]
    public struct WaypointData
    {
        public Transform point;
        public bool enableParticles;
        public bool openDoor;
        public float waitDuration;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (waypoints.Length > 0)
        {
            MoveToNextWaypoint();
        }
    }

    private void Update()
    {
        if (isMoving && currentWaypointIndex < waypoints.Length)
        {
            MoveTowardsWaypoint();
        }
    }

    private void MoveTowardsWaypoint()
    {
        Transform targetPoint = waypoints[currentWaypointIndex].point;
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        animator.SetBool("IsWalking", true);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
            StartCoroutine(HandleWaypointActions());
        }
    }

    private IEnumerator HandleWaypointActions()
    {
        isMoving = false;
        WaypointData currentData = waypoints[currentWaypointIndex];

        if (currentData.enableParticles && actionParticles != null)
        {
            actionParticles.Play();
        }

        animator.SetBool("IsActing", true);
        yield return new WaitForSeconds(currentData.waitDuration);
        animator.SetBool("IsActing", false);

        if (currentData.enableParticles && actionParticles != null)
        {
            actionParticles.Stop();
        }

        if (currentData.openDoor)
        {
            Door door = currentData.point.GetComponent<Door>();
            if (door != null)
            {
                Debug.Log("Мясник взаимодействует с дверью");
                door.Interact(gameObject);
            }
            else
            {
                Debug.Log("Дверь не найдена на точке");
            }
        }

        if (currentWaypointIndex == waypoints.Length - 1)
        {
            CheckIfDefeated();
        }

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Переход к следующей точке (патрулирование по кругу)
        MoveToNextWaypoint();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверка пересечения с предметами ItemIdentifier
        ItemIdentifier item = other.GetComponent<ItemIdentifier>();
        if (item != null)
        {
            string itemName = item.ItemName;
            if (requiredItems.Contains(itemName))
            {
                collectedItems.Add(itemName);
                Debug.Log($"Мясник получил предмет: {itemName}");
            }
        }
    }
    private void CheckIfDefeated()
    {
        if (collectedItems.Count >= requiredItems.Count)
        {
            Debug.Log("Мясник побежден!");
            Destroy(gameObject);
        }
    }

    private void MoveToNextWaypoint()
    {
        isMoving = true;
    }

    public void Teleport(Vector3 destination)
    {
        transform.position = destination;
    }
}
