using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectController : MonoBehaviour
{
    [SerializeField] private Transform inventoryUIParent; // Родитель для UI элементов инвентаря
    [SerializeField] private GameObject inventorySlotPrefab; // Префаб для отображения предметов в UI

    private List<CollectibleItem> inventory = new List<CollectibleItem>();

    public void AddItemToInventory(CollectibleItem item)
    {
        inventory.Add(item);
        DisplayItemInUI(item);
    }

    private void DisplayItemInUI(CollectibleItem item)
    {
        GameObject slot = Instantiate(inventorySlotPrefab, inventoryUIParent);
        Image itemImage = slot.GetComponentInChildren<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = item.ItemSprite;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ЛКМ
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                ICollectible collectible = hit.collider.GetComponent<ICollectible>();
                if (collectible != null)
                {
                    collectible.Collect(this);
                }
            }
        }
    }
}
