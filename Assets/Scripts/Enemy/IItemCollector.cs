using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IItemCollector
{
    void Initialize(List<string> requiredItems);
    void CollectItem(string itemName);
    bool HasAllRequiredItems();
}
