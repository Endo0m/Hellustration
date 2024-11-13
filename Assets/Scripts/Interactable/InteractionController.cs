using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f; // Дистанция взаимодействия для объектов
    [SerializeField] private LayerMask interactableLayer; // Слой для определения взаимодействуемых объектов

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Проверка на нажатие ЛКМ
        if (Input.GetMouseButtonDown(0))
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        // Проверка на взаимодействие с UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            HandleUIInteraction();
            return;
        }

        // Взаимодействие с игровыми объектами
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
                    collectible.Collect(collectController); // Передаем collectController вместо InteractionController
                }
                return;
            }


            // Проверка на WobbleObject (активация покачивания)
            WobbleObject wobbleObject = hit.collider.GetComponent<WobbleObject>();
            if (wobbleObject != null)
            {
                wobbleObject.Interact(gameObject);
                return;
            }

            // Проверка на IInteractable (общее взаимодействие)
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
            }
        }
    }

    private void HandleUIInteraction()
    {
        // Логика обработки взаимодействия с UI (например, перетаскивание предметов)
        // Здесь можно использовать логику для DraggableUIItem
        // Получаем компонент под курсором, если необходимо
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            DraggableUIItem draggableItem = result.gameObject.GetComponent<DraggableUIItem>();
            if (draggableItem != null)
            {
                // Логика взаимодействия с UI-предметом
                // Например, вызов метода для начала перетаскивания или другого действия
                Debug.Log($"UI Item interacted: {draggableItem.ItemName}");
                return;
            }
        }
    }
}
