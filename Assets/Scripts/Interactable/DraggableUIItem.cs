using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private string itemName; // Имя предмета
    private Image itemImage;
    private bool isDraggable = false;
    private Transform originalParent;
    private Canvas parentCanvas;

    // Новое свойство для комбинирования
    public ICombinable CombinableData { get; private set; }

    public string ItemName => itemName;

    private void Awake()
    {
        itemImage = GetComponent<Image>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(Sprite sprite, string name, ICombinable combinableData = null)
    {
        itemName = name;
        itemImage.sprite = sprite;
        CombinableData = combinableData; // Инициализация данных для комбинирования
        SetDraggable(false); // Отключаем перетаскивание по умолчанию
    }

    public void UpdateSprite(Sprite newSprite)
    {
        if (itemImage != null)
        {
            itemImage.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning($"Image component missing in {gameObject.name}");
        }
    }

    public void SetDraggable(bool canDrag)
    {
        isDraggable = canDrag;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        originalParent = transform.parent;
        transform.SetParent(parentCanvas.transform);
        GetComponent<CanvasGroup>().blocksRaycasts = false; // Отключаем возможность взаимодействия с другими UI
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, eventData.position, parentCanvas.worldCamera, out Vector2 position);
        transform.localPosition = position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            DropZone dropZone = hit.collider.GetComponent<DropZone>();
            if (dropZone != null)
            {
                // Передача имени текущего предмета и имени DropZone
                if (dropZone.TryCombineWithItem(itemName, dropZone.name))
                {
                    Destroy(gameObject); // Удаляем исходный объект после успешного комбинирования
                    return;
                }

                Debug.LogWarning($"Item '{itemName}' could not be combined in {dropZone.gameObject.name}.");
            }
        }

        // Возврат на место в случае неудачи
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        Debug.Log($"Item '{itemName}' was returned to its original position.");
    }

}
