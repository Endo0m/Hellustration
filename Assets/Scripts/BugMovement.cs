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

    [Header("Audio Settings")]
    private AudioSource audioSource;
    [SerializeField] private string movementSoundKey = "bugMove";
    [SerializeField] private string deathSoundKey = "bugDeath";
    [SerializeField] private float movementSoundInterval = 0.5f;

    private Vector2 currentDirection;
    private float directionChangeTimer;
    private bool isDead = false;
    private float movementSoundTimer;

    private void Start()
    {
        audioSource=GetComponent<AudioSource>();
        if (movementZone == null)
        {
            movementZone = GetComponentInParent<BugZone>();
        }
        if (movementZone != null)
        {
            transform.position = movementZone.GetRandomPointInZone();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D ����
            audioSource.minDistance = 0.2f;
            audioSource.maxDistance = 3f;
        }

        ChooseNewDirection();
    }

    private void Update()
    {
        if (isDead || movementZone == null) return;

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

        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0)
        {
            ChooseNewDirection();
        }

        Vector2 newPosition = (Vector2)transform.position + currentDirection * moveSpeed * Time.deltaTime;

        if (!movementZone.IsPointInZone(newPosition))
        {
            newPosition = movementZone.ClampPointToZone(newPosition);
            ChooseNewDirection();
        }

        transform.position = newPosition;

        // ��������������� ����� �������� � ���������� ����� SoundManager
        movementSoundTimer -= Time.deltaTime;
        if (movementSoundTimer <= 0)
        {
            SoundManager.Instance.PlaySound(movementSoundKey, audioSource);
            movementSoundTimer = movementSoundInterval;
        }

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

        // ��������������� ����� ������ ����� SoundManager
        SoundManager.Instance.PlaySound(deathSoundKey, audioSource);
    }

    private void OnDrawGizmos()
    {
        if (!isDead)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, currentDirection);
        }
    }

    public void SetMovementZone(BugZone zone)
    {
        movementZone = zone;
        if (zone != null && Application.isPlaying)
        {
            transform.position = zone.GetRandomPointInZone();
        }
    }
}