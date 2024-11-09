using UnityEngine;

public class ItemIdentifier : MonoBehaviour
{
    [SerializeField] private string itemName;

    public string ItemName => itemName;
}
