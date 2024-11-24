using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float interactionRadius = 1f;

    [Header("Components")]
    [SerializeField] private GameObject playerLight;
    [SerializeField] private GameObject deathCanvas;

    [Header("Sound Settings")]
    [SerializeField] private string walkSoundKey = "player_walk";
    [SerializeField] private string runSoundKey = "player_run";
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.3f;
    [SerializeField] private float audioVolume = 0.05f;
    private float scale = 1f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isHidden = false;
    private float lastTime = 0f;
    private float lastStepTime = 0f;
    public bool IsHidden { get { return isHidden; } }

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
            string soundKey = isRunning ? runSoundKey : walkSoundKey;
            soundManager.PlaySound(soundKey, audioSource);
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
        // Disable player controls
        this.enabled = false;

        // Stop movement
        rb.velocity = Vector2.zero;

        // Optionally, disable the player's sprite renderer to make them invisible
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Activate the death canvas
        if (deathCanvas != null)
        {
            deathCanvas.SetActive(true);
        }
    }

    public void TriggerDeathAnimation(System.Action onDeathComplete)
    {
        animator.SetTrigger("Die"); // Предполагается, что у вас есть триггер "Die" для анимации смерти
        StartCoroutine(HandleDeathAnimation(onDeathComplete));
    }

    private IEnumerator HandleDeathAnimation(System.Action onDeathComplete)
    {
        // Ждем пока начнется анимация
        yield return new WaitForEndOfFrame();
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Проверяем, что анимация действительно перешла в состояние "Die"
        if (stateInfo.IsName("Die"))
        {
            yield return new WaitForSeconds(stateInfo.length);
        }
        else
        {
            // Если анимация не началась, устанавливаем некоторое время ожидания по умолчанию
            yield return new WaitForSeconds(1f);
        }

        onDeathComplete?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}