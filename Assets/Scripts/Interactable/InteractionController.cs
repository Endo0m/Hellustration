using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private float interactionWidth = 2f;   // Ширина капсулы взаимодействия
    [SerializeField] private float interactionHeight = 4f;  // Высота капсулы взаимодействия
    [SerializeField] private LayerMask interactableLayer;   // Слой для определения взаимодействуемых объектов

    private Camera mainCamera;
    private CollectController collectController; // Ссылка на CollectController

    private void Start()
    {
        mainCamera = Camera.main;
        collectController = GetComponent<CollectController>();
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

        // Определяем параметры капсулы взаимодействия
        Vector2 capsuleCenter = transform.position; // Центр капсулы — позиция игрока
        Vector2 capsuleSize = new Vector2(interactionWidth, interactionHeight);
        float capsuleAngle = 0f; // Без поворота

        // Получаем все коллайдеры внутри капсулы взаимодействия
        Collider2D[] hits = Physics2D.OverlapCapsuleAll(
            capsuleCenter,
            capsuleSize,
            CapsuleDirection2D.Vertical,
            capsuleAngle,
            interactableLayer
        );

        if (hits.Length > 0)
        {
            // Получаем позицию мыши в мировых координатах
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePosition.x, mousePosition.y);

            // Обрабатываем каждый найденный коллайдер
            foreach (Collider2D hit in hits)
            {
                // Проверяем, находится ли курсор мыши над коллайдером
                if (hit.OverlapPoint(mousePos2D))
                {
                    // Обработка собираемых предметов
                    ICollectible collectible = hit.GetComponent<ICollectible>();
                    if (collectible != null)
                    {
                        if (collectController != null)
                        {
                            collectible.Collect(collectController);
                        }
                        return;
                    }

                    // Обработка подсказок (HintItem)
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

                    // Обработка других взаимодействий
                    IInteractable interactable = hit.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        interactable.Interact(gameObject);
                        return;
                    }

                    // Обработка WobbleObject
                    WobbleObject wobbleObject = hit.GetComponent<WobbleObject>();
                    if (wobbleObject != null)
                    {
                        wobbleObject.Interact(gameObject);
                        return;
                    }

                    // Обработка Box
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
        // Логика обработки взаимодействия с UI (например, перетаскивание предметов)
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            DraggableUIItem draggableItem = result.gameObject.GetComponent<DraggableUIItem>();
            if (draggableItem != null)
            {
                // Логика взаимодействия с UI-предметом
                Debug.Log($"Взаимодействие с UI-предметом: {draggableItem.ItemName}");
                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация капсулы взаимодействия в окне сцены
        Gizmos.color = Color.yellow;

        Vector2 capsuleCenter = transform.position;
        Vector2 capsuleSize = new Vector2(interactionWidth, interactionHeight);
        float capsuleAngle = 0f;

        DrawWireCapsule(capsuleCenter, capsuleSize, capsuleAngle);
    }

    private void DrawWireCapsule(Vector2 center, Vector2 size, float angle)
    {
        // Сохраняем текущую матрицу Gizmos
        Matrix4x4 oldMatrix = Gizmos.matrix;

        // Применяем поворот и позицию
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0f, 0f, angle), Vector3.one);

        float radius = size.x / 2f;
        float height = size.y - (radius * 2f);

        // Рисуем центральный прямоугольник
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, height, 0f));

        // Рисуем верхнюю полусферу
        Gizmos.DrawWireSphere(new Vector3(0f, height / 2f, 0f), radius);

        // Рисуем нижнюю полусферу
        Gizmos.DrawWireSphere(new Vector3(0f, -height / 2f, 0f), radius);

        // Восстанавливаем матрицу Gizmos
        Gizmos.matrix = oldMatrix;
    }
}
