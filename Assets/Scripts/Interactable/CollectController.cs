using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CollectController : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private Transform inventoryUIParent; // �������� ��� ������ � �������� ���������� (������������ Image)
    [SerializeField] private int totalItemsToCollect = 3; // ���������� ��������� ��� ��������� ��������������

    [Header("Hint Inventory Settings")]
    [SerializeField] private Transform hintInventoryUIParent; // �������� ��� ������ � �����������
    [SerializeField] private GameObject hintSlotPrefab; // ������ ����� ��� ���������
    [SerializeField] private GameObject hintPanel; // ������ ��� ����������� ���������
    [SerializeField] private TextMeshProUGUI hintTextComponent; // ��������� ��� ������ ���������

    private Dictionary<string, DraggableUIItem> inventoryItems = new Dictionary<string, DraggableUIItem>();
    private List<HintItem> hintInventory = new List<HintItem>();
    private int collectedItemCount = 0;

    private void Start()
    {
        // ������������� ������������ ������ � DraggableUIItem
        foreach (Transform child in inventoryUIParent)
        {
            DraggableUIItem draggableItem = child.GetComponent<DraggableUIItem>();
            if (draggableItem != null && !string.IsNullOrEmpty(draggableItem.ItemName))
            {
                inventoryItems[draggableItem.ItemName] = draggableItem;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // �������� ������� ���
        {
            // �������� �� �������������� � UI
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                // ������ �������� ��������
                ICollectible collectible = hit.collider.GetComponent<ICollectible>();
                if (collectible != null)
                {
                    collectible.Collect(this);
                    return;
                }

                // ������ ��������� (HintItem)
                HintItem hintItem = hit.collider.GetComponent<HintItem>();
                if (hintItem != null)
                {
                    AddHintToInventory(hintItem);
                    hintItem.gameObject.SetActive(false); // ��������� ������ ����� �������
                }
            }
        }
    }

    public void AddItemToInventory(CollectibleItem item)
    {
        string itemName = item.ItemName;

        if (inventoryItems.ContainsKey(itemName))
        {
            // ��������� ����������� � �����
            DraggableUIItem uiItem = inventoryItems[itemName];
            uiItem.UpdateSprite(item.ItemSprite);
            collectedItemCount++;

            // ���������, ����� �� ������������ ��������������
            if (collectedItemCount >= totalItemsToCollect)
            {
                EnableItemDragging();
            }

            // ������� ������������ ��������
            item.FadeOutAndDisable();
        }
        else
        {
            Debug.LogWarning($"���� ��� �������� � ������ {itemName} �� ������!");
        }
    }

    public void AddHintToInventory(HintItem hintItem)
    {
        hintInventory.Add(hintItem);
        DisplayHintInUI(hintItem);
    }

    private void DisplayHintInUI(HintItem hintItem)
    {
        // �������� ����� ��� ��������� � UI
        GameObject slot = Instantiate(hintSlotPrefab, hintInventoryUIParent);
        Button button = slot.AddComponent<Button>();
        button.onClick.AddListener(() => ShowHint(hintItem.HintText));

       
    }

    private void ShowHint(string hintText)
    {
        hintPanel.SetActive(true);
        hintTextComponent.text = hintText;
    }

    private void EnableItemDragging()
    {
        foreach (var item in inventoryItems.Values)
        {
            item.SetDraggable(true);
        }
    }
}
