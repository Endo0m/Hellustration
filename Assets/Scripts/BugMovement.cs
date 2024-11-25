using UnityEngine;

public class BugMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minChangeDirectionTime = 1f;
    [SerializeField] private float maxChangeDirectionTime = 3f;

    [Header("References")]
    [SerializeField] private Transform headPivot;
    [SerializeField] private SpriteRenderer bugSprite;
    [SerializeField] private Sprite deadBugSprite;
    [SerializeField] private BugZone movementZone;

    private Vector2 currentDirection;
    private float directionChangeTimer;
    private bool isDead = false;

    private void Start()
    {
        if (movementZone == null)
        {
            // Попытка найти зону в родительском объекте
            movementZone = GetComponentInParent<BugZone>();
        }

        if (movementZone != null)
        {
            // Устанавливаем начальную позицию внутри зоны
            transform.position = movementZone.GetRandomPointInZone();
        }

        ChooseNewDirection();
    }

    private void Update()
    {
        if (isDead || movementZone == null) return;

        // Обработка клика мыши
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

            if (hitCollider != null && hitCollider.gameObject == gameObject)
            {
                Die();
                return;
            }
        }

        // Изменение направления по таймеру
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0)
        {
            ChooseNewDirection();
        }

        // Движение
        Vector2 newPosition = (Vector2)transform.position + currentDirection * moveSpeed * Time.deltaTime;

        // Если жук пытается выйти за пределы зоны
        if (!movementZone.IsPointInZone(newPosition))
        {
            newPosition = movementZone.ClampPointToZone(newPosition);
            ChooseNewDirection(); // Меняем направление при столкновении с границей
        }

        transform.position = newPosition;

        // Поворот спрайта в направлении движения
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ChooseNewDirection()
    {
        currentDirection = Random.insideUnitCircle.normalized;
        directionChangeTimer = Random.Range(minChangeDirectionTime, maxChangeDirectionTime);

        if (headPivot != null)
        {
            headPivot.localPosition = currentDirection;
        }
    }

    private void Die()
    {
        isDead = true;
        if (bugSprite != null && deadBugSprite != null)
        {
            bugSprite.sprite = deadBugSprite;
        }
    }

    private void OnDrawGizmos()
    {
        if (!isDead)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, currentDirection);
        }
    }

    // Метод для установки зоны движения (можно вызывать из редактора или кода)
    public void SetMovementZone(BugZone zone)
    {
        movementZone = zone;
        if (zone != null && Application.isPlaying)
        {
            transform.position = zone.GetRandomPointInZone();
        }
    }
}