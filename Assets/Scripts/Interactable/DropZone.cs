using UnityEngine;

public class DropZone : MonoBehaviour
{
    [SerializeField] private string acceptedItemName; // Имя предмета, который может быть принят
    [SerializeField] private GameObject physicalItemPrefab; // Префаб для физического представления предмета

    public bool AcceptsItem(string itemName)
    {
        return itemName == acceptedItemName;
    }

    public void PlaceItem(string itemName)
    {
        if (physicalItemPrefab != null)
        {
            GameObject placedItem = Instantiate(physicalItemPrefab, transform.position, Quaternion.identity);
            placedItem.name = itemName;
            Debug.Log($"Placed item {itemName} in {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"No prefab assigned for physical item placement in {gameObject.name}");
        }
    }
}
