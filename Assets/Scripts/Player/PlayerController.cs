using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Скорость движения игрока
    [SerializeField] private float interactionRadius = 1f; // Радиус взаимодействия с объектами

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isHidden = false;

    public bool IsHidden { get { return isHidden; } }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isHidden)
        {
            // Проверка на выход из укрытия
            if (Input.GetKeyDown(KeyCode.E))
            {
                Reveal();
            }
            return;
        }

        // Управление движением
        movement.x = Input.GetAxisRaw("Horizontal");
        RotateToMouse();

        // Проверка на взаимодействие
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void FixedUpdate()
    {
        if (!isHidden)
        {
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
        }
    }

    private void RotateToMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePosition - transform.position).normalized;

        if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1); // Поворот влево
        else if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1); // Поворот вправо
    }

    private void Interact()
    {
        // Поиск объектов для взаимодействия в радиусе
        Collider2D[] interactables = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
        foreach (var obj in interactables)
        {
            IInteractable interactable = obj.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(this); // Взаимодействие с объектом
                break;
            }
        }
    }

    public void HideInObject(Transform hideout)
    {
        isHidden = true;
        transform.position = hideout.position;
        gameObject.layer = LayerMask.NameToLayer("Hidden");
        rb.velocity = Vector2.zero;
        Debug.Log("Игрок спрятался");
    }

    public void Reveal()
    {
        isHidden = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        Debug.Log("Игрок вышел из укрытия");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
