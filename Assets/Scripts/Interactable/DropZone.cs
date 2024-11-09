using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour
{
    [SerializeField] private List<string> requiredItemNames; // Имена предметов, необходимых для крафта ловушки
    [SerializeField] private GameObject trapPrefab; // Префаб ловушки
    private List<string> collectedItems = new List<string>();

    // Метод для добавления предмета в DropZone
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

    // Метод создания ловушки
    private void CreateTrap()
    {
        Instantiate(trapPrefab, transform.position, Quaternion.identity);
        collectedItems.Clear(); // Очистка списка после создания ловушки
        Debug.Log("Trap created!");
    }
}
