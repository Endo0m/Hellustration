using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private string itemName; // ��� ��������
    private Image itemImage;
    private bool isDraggable = false;
    private Transform originalParent;
    private Canvas parentCanvas;

    // ����� �������� ��� ��������������
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
        CombinableData = combinableData; // ������������� ������ ��� ��������������
        SetDraggable(false); // ��������� �������������� �� ���������
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
        GetComponent<CanvasGroup>().blocksRaycasts = false; // ��������� ����������� �������������� � ������� UI
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
            // �������� �������������� � ItemModifierZone
            ItemModifierZone modifierZone = hit.collider.GetComponent<ItemModifierZone>();
            if (modifierZone != null)
            {
                if (modifierZone.TryModifyItem(gameObject, itemName)) // �������� ������� ������ ��� ������
                {
                    Destroy(gameObject); // ������� �������� ������ ����� �������� �����������
                    return;
                }
            }

            // �������� �������������� � DropZone (��������� ��� ���������)
            DropZone dropZone = hit.collider.GetComponent<DropZone>();
            if (dropZone != null)
            {
                if (dropZone.TryCombineWithItem(itemName, dropZone.name))
                {
                    Destroy(gameObject); // ������� �������� ������ ����� ��������� ��������������
                    return;
                }
            }
        }

        // ������� �� ����� � ������ �������
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        Debug.Log($"Item '{itemName}' was returned to its original position.");
    }



}
