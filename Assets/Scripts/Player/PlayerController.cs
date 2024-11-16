using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Скорость движения игрока
    [SerializeField] private float runSpeed = 8f; // Скорость бега
    [SerializeField] private float interactionRadius = 1f; // Радиус взаимодействия с объектами
    [SerializeField] private PulseController pulseController;
    private float scale = 1.5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isRunning = false;
    private bool isHidden = false;
    private float lastTime = 0f;
    public bool IsHidden { get { return isHidden; } }
    //private SoundManager soundManager;
    private AudioSource audioSource;
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        //soundManager = FindObjectOfType<SoundManager>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            // Добавление компонента AudioSource, если его нет
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
        // Проверка на движение игрока
        if (movement != Vector2.zero && !audioSource.isPlaying)
        {
            // Воспроизведение звука шагов, если игрок движется
            //soundManager?.PlaySound("footstep", audioSource);
        }
        movement.x = Input.GetAxisRaw("Horizontal");
        if (movement != Vector2.zero)
        {
            Debug.Log("Player is moving");
        }
        else
        {
            Debug.Log("Player is idle");
        }
        RotateToMouse();
        animator.SetBool("isWalking", movement != Vector2.zero);
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void FixedUpdate()
    {
        if (!isHidden)
        {
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;
            rb.velocity = new Vector2(movement.x * currentSpeed, rb.velocity.y);

            if (Time.time - lastTime >= 1f)
            {
                lastTime = Time.time;

                if (rb.velocity.magnitude > 3f)
                {
                    pulseController.UpPulseCounter();
                }
                else
                {
                    pulseController.RestorePulse();
                }
            }
        }
    }

    private void RotateToMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePosition - transform.position).normalized;

        if (direction.x < 0)
            transform.localScale = new Vector3(-scale, scale, scale); // Поворот влево
        else if (direction.x > 0)
            transform.localScale = new Vector3(scale, scale, scale); // Поворот вправо
    }

    private void Interact()
    {
        // Проверка на взаимодействие с объектами в радиусе
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

    public void HideInObject(Transform hideout)
    {
        isHidden = true;
        transform.position = hideout.position;
        gameObject.layer = LayerMask.NameToLayer("Hidden"); // Меняем слой на "Hidden"
        rb.velocity = Vector2.zero;

        // Замораживаем физику (Kinematic)
        rb.isKinematic = true;

        // Изменяем ордер слоя (например, на -3)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = -3; // Остановим взаимодействие с физикой
        }
    }

    public void Reveal()
    {
        isHidden = false;
        gameObject.layer = LayerMask.NameToLayer("Player"); // Возвращаем слой обратно на "Player"
        rb.isKinematic = false; // Возвращаем физику

        // Восстанавливаем ордер слоя в нормальное положение (например, на 0)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 0; // Восстанавливаем ордер слоя
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}