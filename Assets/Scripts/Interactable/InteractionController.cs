using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f; // ��������� �������������� ��� ��������
    [SerializeField] private LayerMask interactableLayer; // ���� ��� ����������� ����������������� ��������

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
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

        // �������������� � �������� ���������
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, interactionDistance, interactableLayer);

        if (hit.collider != null)
        {
            ICollectible collectible = hit.collider.GetComponent<ICollectible>();
            if (collectible != null)
            {
                CollectController collectController = GetComponent<CollectController>();
                if (collectController != null)
                {
                    collectible.Collect(collectController); // �������� collectController ������ InteractionController
                }
                return;
            }


            // �������� �� WobbleObject (��������� �����������)
            WobbleObject wobbleObject = hit.collider.GetComponent<WobbleObject>();
            if (wobbleObject != null)
            {
                wobbleObject.Interact(gameObject);
                return;
            }

            // �������� �� IInteractable (����� ��������������)
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
            }
        }
    }

    private void HandleUIInteraction()
    {
        // ������ ��������� �������������� � UI (��������, �������������� ���������)
        // ����� ����� ������������ ������ ��� DraggableUIItem
        // �������� ��������� ��� ��������, ���� ����������
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            DraggableUIItem draggableItem = result.gameObject.GetComponent<DraggableUIItem>();
            if (draggableItem != null)
            {
                // ������ �������������� � UI-���������
                // ��������, ����� ������ ��� ������ �������������� ��� ������� ��������
                Debug.Log($"UI Item interacted: {draggableItem.ItemName}");
                return;
            }
        }
    }
}
