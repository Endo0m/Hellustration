using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButcherEnemy : EnemyBase
{
    [SerializeField] private List<string> requiredItems;
    private HashSet<string> collectedItems = new HashSet<string>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemIdentifier item = other.GetComponent<ItemIdentifier>();
        if (item != null && requiredItems.Contains(item.ItemName))
        {
            collectedItems.Add(item.ItemName);
            Debug.Log($"Мясник получил предмет: {item.ItemName}");
        }
    }

    protected override void CheckIfDefeated()
    {
        if (collectedItems.Count >= requiredItems.Count)
        {
            Debug.Log("Мясник побежден!");
            Destroy(gameObject);
        }
    }
}
