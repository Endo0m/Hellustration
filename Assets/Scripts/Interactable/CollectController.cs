using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CollectController : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private Transform inventoryUIParent; // Родитель для слотов с обычными предметами (существующие Image)
    [SerializeField] private int totalItemsToCollect = 3; // Количество предметов для активации перетаскивания

    [Header("Hint Inventory Settings")]
    [SerializeField] private Transform hintInventoryUIParent; // Родитель для слотов с подсказками
    [SerializeField] private GameObject hintSlotPrefab; // Префаб слота для подсказок
    [SerializeField] private GameObject hintPanel; // Панель для отображения подсказок
    [SerializeField] private TextMeshProUGUI hintTextComponent; // Компонент для текста подсказки

    private Dictionary<string, DraggableUIItem> inventoryItems = new Dictionary<string, DraggableUIItem>();
    private List<HintItem> hintInventory = new List<HintItem>();
    private int collectedItemCount = 0;

    private void Start()
    {
        // Инициализация существующих слотов с DraggableUIItem
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
        if (Input.GetMouseButtonDown(0)) // Проверка нажатия ЛКМ
        {
            // Проверка на взаимодействие с UI
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                // Подбор обычного предмета
                ICollectible collectible = hit.collider.GetComponent<ICollectible>();
                if (collectible != null)
                {
                    collectible.Collect(this);
                    return;
                }

                // Подбор подсказки (HintItem)
                HintItem hintItem = hit.collider.GetComponent<HintItem>();
                if (hintItem != null)
                {
                    AddHintToInventory(hintItem);
                    hintItem.gameObject.SetActive(false); // Отключаем объект после подбора
                }
            }
        }
    }

    public void AddItemToInventory(CollectibleItem item)
    {
        string itemName = item.ItemName;

        if (inventoryItems.ContainsKey(itemName))
        {
            // Обновляем изображение в слоте
            DraggableUIItem uiItem = inventoryItems[itemName];
            uiItem.UpdateSprite(item.ItemSprite);
            collectedItemCount++;

            // Проверяем, можно ли активировать перетаскивание
            if (collectedItemCount >= totalItemsToCollect)
            {
                EnableItemDragging();
            }

            // Плавное исчезновение предмета
            item.FadeOutAndDisable();
        }
        else
        {
            Debug.LogWarning($"Слот для предмета с именем {itemName} не найден!");
        }
    }

    public void AddHintToInventory(HintItem hintItem)
    {
        hintInventory.Add(hintItem);
        DisplayHintInUI(hintItem);
    }

    private void DisplayHintInUI(HintItem hintItem)
    {
        // Создание слота для подсказки в UI
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
