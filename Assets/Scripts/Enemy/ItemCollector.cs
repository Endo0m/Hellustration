using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemCollector : MonoBehaviour, IItemCollector
{
    private List<string> requiredItems;
    private HashSet<string> collectedItems = new HashSet<string>();
    private bool hasAllItems = false;

    public void Initialize(List<string> items)
    {
        requiredItems = new List<string>(items);
    }

    public void CollectItem(string itemName)
    {
        if (requiredItems.Contains(itemName))
        {
            collectedItems.Add(itemName);
            CheckIfAllItemsCollected();
            Debug.Log($"Collected item: {itemName}. Total: {collectedItems.Count}/{requiredItems.Count}");
        }
    }

    public bool HasAllRequiredItems()
    {
        return hasAllItems;
    }

    private void CheckIfAllItemsCollected()
    {
        hasAllItems = collectedItems.Count >= requiredItems.Count;
    }
}