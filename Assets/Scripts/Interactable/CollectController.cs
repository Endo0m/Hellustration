using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CollectController : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private Transform inventoryUIParent; // �������� ��� ������ � �������� ����������
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

    // ������� ����� Update(), ��� ��� �������������� ������ �������������� � InteractionController

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
