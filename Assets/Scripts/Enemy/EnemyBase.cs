using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask boundaryLayer;

    [Header("Patrol Settings")]
    [SerializeField] private float minPatrolDistance = 2f; // ����������� ��������� �������
    [SerializeField] private float maxPatrolDistance = 5f; // ������������ ��������� �������
    private float patrolDistance; // ������� ��������� �������
    private bool isMovingRight = true;

    [Header("Detection Settings")]
    [SerializeField] private float forwardDetectionRange = 5f; // ����� ���� �������
    [SerializeField] private float backwardDetectionRange = 3f; // ����� ���� �����
    private bool isChasing = false;

    private Rigidbody2D rb;
    private PlayerController player;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerController>();
        SetRandomPatrolDistance();
    }

    private void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
            DetectPlayer();
        }
    }

    private void Patrol()
    {
        if (patrolDistance <= 0)
        {
            SetRandomPatrolDistance();
            isMovingRight = !isMovingRight; // ������ �����������
        }

        float direction = isMovingRight ? 1 : -1;
        rb.velocity = new Vector2(direction * patrolSpeed, rb.velocity.y);
        patrolDistance -= Time.deltaTime * patrolSpeed;
        FlipSprite(direction);
    }

    private void SetRandomPatrolDistance()
    {
        patrolDistance = Random.Range(minPatrolDistance, maxPatrolDistance);
    }

    private void DetectPlayer()
    {
        Vector2 forwardDirection = isMovingRight ? Vector2.right : Vector2.left;
        Vector2 backwardDirection = -forwardDirection;

        // �������� ������ �������
        RaycastHit2D forwardHit = Physics2D.Raycast(transform.position, forwardDirection, forwardDetectionRange, playerLayer);
        if (forwardHit.collider != null && forwardHit.collider.CompareTag("Player"))
        {
            isChasing = true;
            Debug.Log("���� ��������� ������ ������� � �������� �������������");
            return;
        }

        // �������� ������ �����
        RaycastHit2D backwardHit = Physics2D.Raycast(transform.position, backwardDirection, backwardDetectionRange, playerLayer);
        if (backwardHit.collider != null && backwardHit.collider.CompareTag("Player"))
        {
            isChasing = true;
            Debug.Log("���� ��������� ������ ����� � �������� �������������");
        }
    }

    private void ChasePlayer()
    {
        if (player.IsHidden || Vector2.Distance(transform.position, player.transform.position) > Mathf.Max(forwardDetectionRange, backwardDetectionRange) * 1.5f)
        {
            isChasing = false;
            SetRandomPatrolDistance();
            Debug.Log("���� ������� ������ � ������������ � ��������������");
            return;
        }

        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);
        FlipSprite(direction.x);
    }

    private void FlipSprite(float directionX)
    {
        if ((directionX > 0 && transform.localScale.x < 0) || (directionX < 0 && transform.localScale.x > 0))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == Mathf.Log(boundaryLayer.value, 2))
        {
            isMovingRight = !isMovingRight;
            SetRandomPatrolDistance();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // ������������ ����� ��� ����������� ������
        Gizmos.color = Color.red;
        Vector2 forwardDirection = isMovingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)forwardDirection * forwardDetectionRange);

        Gizmos.color = Color.blue;
        Vector2 backwardDirection = -forwardDirection;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)backwardDirection * backwardDetectionRange);
    }

    protected abstract void Attack();
}
