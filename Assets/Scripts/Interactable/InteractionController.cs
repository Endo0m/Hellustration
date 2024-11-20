using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private float interactionWidth = 2f;   // ������ ������� ��������������
    [SerializeField] private float interactionHeight = 4f;  // ������ ������� ��������������
    [SerializeField] private LayerMask interactableLayer;   // ���� ��� ����������� ����������������� ��������

    private Camera mainCamera;
    private CollectController collectController; // ������ �� CollectController

    private void Start()
    {
        mainCamera = Camera.main;
        collectController = GetComponent<CollectController>();
    }

    private void Update()
    {
        // �������� �� ������� ���
        if (Input.GetMouseButtonDown(0))
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        // �������� �� �������������� � UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            HandleUIInteraction();
            return;
        }

        // ���������� ��������� ������� ��������������
        Vector2 capsuleCenter = transform.position; // ����� ������� � ������� ������
        Vector2 capsuleSize = new Vector2(interactionWidth, interactionHeight);
        float capsuleAngle = 0f; // ��� ��������

        // �������� ��� ���������� ������ ������� ��������������
        Collider2D[] hits = Physics2D.OverlapCapsuleAll(
            capsuleCenter,
            capsuleSize,
            CapsuleDirection2D.Vertical,
            capsuleAngle,
            interactableLayer
        );

        if (hits.Length > 0)
        {
            // �������� ������� ���� � ������� �����������
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePosition.x, mousePosition.y);

            // ������������ ������ ��������� ���������
            foreach (Collider2D hit in hits)
            {
                // ���������, ��������� �� ������ ���� ��� �����������
                if (hit.OverlapPoint(mousePos2D))
                {
                    // ��������� ���������� ���������
                    ICollectible collectible = hit.GetComponent<ICollectible>();
                    if (collectible != null)
                    {
                        if (collectController != null)
                        {
                            collectible.Collect(collectController);
                        }
                        return;
                    }

                    // ��������� ��������� (HintItem)
                    HintItem hintItem = hit.GetComponent<HintItem>();
                    if (hintItem != null)
                    {
                        if (collectController != null)
                        {
                            collectController.AddHintToInventory(hintItem);
                            hintItem.gameObject.SetActive(false);
                        }
                        return;
                    }

                    // ��������� ������ ��������������
                    IInteractable interactable = hit.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        interactable.Interact(gameObject);
                        return;
                    }

                    // ��������� WobbleObject
                    WobbleObject wobbleObject = hit.GetComponent<WobbleObject>();
                    if (wobbleObject != null)
                    {
                        wobbleObject.Interact(gameObject);
                        return;
                    }

                    // ��������� Box
                    Box box = hit.GetComponent<Box>();
                    if (box != null)
                    {
                        box.Interact(gameObject);
                        return;
                    }
                }
            }
        }
    }

    private void HandleUIInteraction()
    {
        // ������ ��������� �������������� � UI (��������, �������������� ���������)
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            DraggableUIItem draggableItem = result.gameObject.GetComponent<DraggableUIItem>();
            if (draggableItem != null)
            {
                // ������ �������������� � UI-���������
                Debug.Log($"�������������� � UI-���������: {draggableItem.ItemName}");
                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // ������������ ������� �������������� � ���� �����
        Gizmos.color = Color.yellow;

        Vector2 capsuleCenter = transform.position;
        Vector2 capsuleSize = new Vector2(interactionWidth, interactionHeight);
        float capsuleAngle = 0f;

        DrawWireCapsule(capsuleCenter, capsuleSize, capsuleAngle);
    }

    private void DrawWireCapsule(Vector2 center, Vector2 size, float angle)
    {
        // ��������� ������� ������� Gizmos
        Matrix4x4 oldMatrix = Gizmos.matrix;

        // ��������� ������� � �������
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0f, 0f, angle), Vector3.one);

        float radius = size.x / 2f;
        float height = size.y - (radius * 2f);

        // ������ ����������� �������������
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, height, 0f));

        // ������ ������� ���������
        Gizmos.DrawWireSphere(new Vector3(0f, height / 2f, 0f), radius);

        // ������ ������ ���������
        Gizmos.DrawWireSphere(new Vector3(0f, -height / 2f, 0f), radius);

        // ��������������� ������� Gizmos
        Gizmos.matrix = oldMatrix;
    }
}
