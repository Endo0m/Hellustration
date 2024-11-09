using System.Collections.Generic;
using UnityEngine;

public class ButcherEnemy : MonoBehaviour
{
    [SerializeField] private List<string> acceptedItemNames; // »мена предметов, которые уничтожают ћ€сника

    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemIdentifier itemIdentifier = other.GetComponent<ItemIdentifier>();
        if (itemIdentifier != null && acceptedItemNames.Contains(itemIdentifier.ItemName))
        {
            Debug.Log($"Item '{itemIdentifier.ItemName}' triggered the Butcher's death.");
            Destroy(gameObject); // ”ничтожение "ћ€сника"
        }
    }
}
