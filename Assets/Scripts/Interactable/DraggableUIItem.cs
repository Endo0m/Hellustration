using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class DraggableUIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private string itemName; // ��� �������� (ID)
    private Image itemImage;
    private bool isDraggable = false;
    private Transform originalParent;
    private Canvas parentCanvas;

    public string ItemName => itemName;

    private void Awake()
    {
        itemImage = GetComponent<Image>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(Sprite sprite, string name, CollectController controller)
    {
        itemName = name;
        itemImage.sprite = sprite;
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

        // ���������� Raycast ��� ����������� DropZone
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            DropZone dropZone = hit.collider.GetComponent<DropZone>();
            if (dropZone != null && dropZone.AcceptsItem(itemName))
            {
                Debug.Log($"Item {itemName} placed in {dropZone.gameObject.name}");

                // ������� ������� �� UI � ������� ��� � DropZone
                dropZone.PlaceItem(itemName);
                Destroy(gameObject); // ������� ������� �� UI
                return;
            }
        }

        // ���� ���������� �� �������, ���������� ������� �� �����
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        Debug.Log($"Item {itemName} could not be placed and was returned to inventory.");
    }
}