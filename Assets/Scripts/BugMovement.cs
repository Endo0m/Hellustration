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
    private AudioSource movementAudioSource; // ��������� �������� ��� �������� (3D)
    private AudioSource deathAudioSource;    // ��������� �������� ��� ������ (2D)
    [SerializeField] private string[] movementSoundKeys = { "bugMove1", "bugMove2", "bugMove3" };
    [SerializeField] private string[] deathSoundKeys = { "bugDeath1", "bugDeath2", "bugDeath3", "bugDeath4", "bugDeath5", "bugDeath6" };
    [SerializeField] private float movementSoundInterval = 0.5f;

    private Vector2 currentDirection;
    private float directionChangeTimer;
    private bool isDead = false;
    private float movementSoundTimer;

    private void Start()
    {
        // ��������� 3D ����� ��������� ��� ��������
        movementAudioSource = gameObject.AddComponent<AudioSource>();
        SetupMovementAudioSource(movementAudioSource);

        // ��������� 2D ����� ��������� ��� ������
        deathAudioSource = gameObject.AddComponent<AudioSource>();
        SetupDeathAudioSource(deathAudioSource);

        if (movementZone == null)
        {
            movementZone = GetComponentInParent<BugZone>();
        }
        if (movementZone != null)
        {
            transform.position = movementZone.GetRandomPointInZone();
        }

        ChooseNewDirection();
    }

    private void SetupMovementAudioSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.spatialBlend = 1f; // ��������� 3D ����
        source.minDistance = 0.2f;
        source.maxDistance = 3f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.volume = 0.08f;
    }

    private void SetupDeathAudioSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.spatialBlend = 0f; // ��������� 2D ����
        source.minDistance = 1f;
        source.maxDistance = 500f;
        source.rolloffMode = AudioRolloffMode.Linear;
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

        // ��������������� ���������� ����� �������� � ���������� (3D)
        movementSoundTimer -= Time.deltaTime;
        if (movementSoundTimer <= 0 && movementSoundKeys.Length > 0)
        {
            string randomMovementSound = movementSoundKeys[Random.Range(0, movementSoundKeys.Length)];
            SoundManager.Instance.PlaySound(randomMovementSound, movementAudioSource);
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

        // ��������������� ���������� ����� ������ (2D)
        if (deathSoundKeys.Length > 0)
        {
            string randomDeathSound = deathSoundKeys[Random.Range(0, deathSoundKeys.Length)];
            SoundManager.Instance.PlaySound(randomDeathSound, deathAudioSource);
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

    public void SetMovementZone(BugZone zone)
    {
        movementZone = zone;
        if (zone != null && Application.isPlaying)
        {
            transform.position = zone.GetRandomPointInZone();
        }
    }
}