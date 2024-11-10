using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // ������� �������� �������� ������
    [SerializeField] private float runSpeed = 8f; // �������� ����
    [SerializeField] private float interactionRadius = 1f; // ������ �������������� � ���������
    [SerializeField] private PlayerPulseUI playerPulseUI;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isHidden = false;
    private bool isRunningFast = false;

    public bool IsHidden { get { return isHidden; } }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

        movement.x = Input.GetAxisRaw("Horizontal");
        RotateToMouse();

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

             if (Mathf.Abs(rb.velocity.x) > 6f && !isRunningFast)
            {
                playerPulseUI.PulseDecreaseRun(true);
                isRunningFast = true;
            }
            else if (Mathf.Abs(rb.velocity.x) <= 6f && isRunningFast)
            {
                playerPulseUI.PulseDecreaseRun(false);
                isRunningFast = false;
            }
        }
    }

    private void RotateToMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePosition - transform.position).normalized;

        if (direction.x < 0)
            transform.localScale = new Vector3(-2, 2, 2); // ������� �����
        else if (direction.x > 0)
            transform.localScale = new Vector3(2, 2, 2); // ������� ������
    }

    private void Interact()
    {
        // ����� �������� ��� �������������� � �������
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
        gameObject.layer = LayerMask.NameToLayer("Hidden");
        rb.velocity = Vector2.zero;
    }

    public void Reveal()
    {
        isHidden = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
