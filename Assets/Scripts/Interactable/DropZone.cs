using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour
{
    [SerializeField] private List<string> requiredItemNames; // ����� ���������, ����������� ��� ������ �������
    [SerializeField] private GameObject trapPrefab; // ������ �������
    private List<string> collectedItems = new List<string>();

    // ����� ��� ���������� �������� � DropZone
    public void AddItem(string itemName)
    {
        if (!requiredItemNames.Contains(itemName))
        {
            Debug.LogWarning($"Item '{itemName}' is not required for this DropZone.");
            return;
        }

        collectedItems.Add(itemName);
        Debug.Log($"Item '{itemName}' added to DropZone.");

        if (collectedItems.Count == requiredItemNames.Count)
        {
            CreateTrap();
        }
    }

    // ����� �������� �������
    private void CreateTrap()
    {
        Instantiate(trapPrefab, transform.position, Quaternion.identity);
        collectedItems.Clear(); // ������� ������ ����� �������� �������
        Debug.Log("Trap created!");
    }
}
