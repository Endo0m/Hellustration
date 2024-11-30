using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float interactionRadius = 1f;

    [Header("Components")]
    [SerializeField] private GameObject playerLight;

    [Header("Sound Settings")]
    [SerializeField] private string[] walkSoundKeys = { "player_run_1", "player_run_2", "player_run_3", "player_run_4", "player_run_5", "player_run_6" }; // Array of walk sound keys
    [SerializeField] private string[] runSoundKeys = { "player_run_1", "player_run_2", "player_run_3", "player_run_4", "player_run_5", "player_run_6" }; // Array of run sound keys
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.3f;
    [SerializeField] private float audioVolume = 0.05f;
    private float scale = 1f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isHidden = false;
    private float lastStepTime = 0f;
    public bool IsHidden { get { return isHidden; } }
    private string lastPlayedSound = "";
    private AudioSource audioSource;
    private Animator animator;
    private SoundManager soundManager;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        soundManager = FindObjectOfType<SoundManager>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Настройка пространственного звука
        audioSource.spatialBlend = 1f;
        audioSource.volume = audioVolume;
        audioSource.maxDistance = 5f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    private void Update()
    {
        if (isHidden)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Reveal();
            }
            return;
        }

        // Управление движением
        movement.x = Input.GetAxisRaw("Horizontal");
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isShiftPressed ? runSpeed : moveSpeed;

        // Поворот спрайта в направлении движения
        if (movement.x != 0)
        {
            transform.localScale = new Vector3(
                movement.x > 0 ? scale : -scale,
                scale,
                scale
            );
        }

        // Анимация и звуки на основе скорости
        float velocityMagnitude = Mathf.Abs(rb.velocity.x);
        bool isRunning = velocityMagnitude > moveSpeed;
        bool isWalking = velocityMagnitude > 0 && velocityMagnitude <= moveSpeed;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isWalking", isWalking);

        // Проверка и воспроизведение звуков шагов
        if ((isWalking || isRunning) && !isHidden)
        {
            float currentInterval = isRunning ? runStepInterval : walkStepInterval;
            if (Time.time - lastStepTime >= currentInterval)
            {
                PlayStepSound(isRunning);
                lastStepTime = Time.time;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        // Применяем движение
        rb.velocity = new Vector2(movement.x * currentSpeed, rb.velocity.y);
    }

    private void PlayStepSound(bool isRunning)
    {
        if (soundManager != null && audioSource != null)
        {
            string[] currentSoundKeys = isRunning ? runSoundKeys : walkSoundKeys;

            if (currentSoundKeys.Length > 0)
            {
                string randomSoundKey;
                do
                {
                    // Выбираем случайный звук
                    int randomIndex = Random.Range(0, currentSoundKeys.Length);
                    randomSoundKey = currentSoundKeys[randomIndex];
                } while (randomSoundKey == lastPlayedSound && currentSoundKeys.Length > 1); // Повторяем если выпал тот же звук и есть другие варианты

                lastPlayedSound = randomSoundKey; // Сохраняем последний проигранный звук
                soundManager.PlaySound(randomSoundKey, audioSource);
            }
        }
    }

    private void Interact()
    {
        Collider2D[] interactables = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
        foreach (var obj in interactables)
        {
            IInteractable interactable = obj.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
                break;
            }
        }
    }

    public void HideInObject(Transform hideout, bool faceRight)
    {
        // Остановка движения
        movement = Vector2.zero;
        rb.velocity = Vector2.zero;

        // Прятание
        isHidden = true;
        transform.position = hideout.position;
        gameObject.layer = LayerMask.NameToLayer("Hidden");
        rb.isKinematic = true;
        // Установка направления (лицо влево или вправо)
        transform.localScale = new Vector3(faceRight ? scale : -scale, scale, scale);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);

        // Изменение слоя для прорисовки
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = -5;
        }

        // Отключение света
        if (playerLight != null)
        {
            playerLight.SetActive(false);
        }
    }

    public void Reveal()
    {
        isHidden = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        rb.isKinematic = false;

        // Восстановление слоя для прорисовки
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 0;
        }

        // Включение света
        if (playerLight != null)
        {
            playerLight.SetActive(true);
        }
    }

    public void TriggerDeathSequence()
    {
        // Hide player sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Load death scene
        SceneManager.LoadScene("SceneDiePlayer");
    }

    public void TriggerDeathAnimation(System.Action onDeathComplete)
    {
        // Disable controls immediately
        this.enabled = false;
        rb.velocity = Vector2.zero;

        animator.SetTrigger("Die");
        StartCoroutine(HandleDeathAnimation(onDeathComplete));
    }

    private IEnumerator HandleDeathAnimation(System.Action onDeathComplete)
    {
        // Wait for animation length or default time
        yield return new WaitForSeconds(2f); // Adjust time as needed

        onDeathComplete?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}