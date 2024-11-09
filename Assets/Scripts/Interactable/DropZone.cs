using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour
{
    [System.Serializable]
    public class ItemCombination
    {
        public string itemName; // Имя предмета, который используется
        public string targetItemName; // Имя целевого объекта
        public GameObject resultPrefab; // Префаб результата
    }

    [SerializeField] private List<ItemCombination> itemCombinations; // Список возможных комбинаций

    public bool TryCombineWithItem(string itemName, string targetName)
    {
        foreach (var combination in itemCombinations)
        {
            Debug.Log($"Checking combination: itemName = {itemName}, targetItemName = {combination.targetItemName}");
            if (combination.itemName == itemName && combination.targetItemName == targetName)
            {
                // Удаление текущего объекта с DropZone
                Destroy(gameObject);

                // Создание нового объекта на месте текущего
                Instantiate(combination.resultPrefab, transform.position, Quaternion.identity);
                Debug.Log($"Combined item '{itemName}' with '{combination.targetItemName}' to create '{combination.resultPrefab.name}'.");
                return true;
            }
        }

        Debug.LogWarning($"Failed to combine item '{itemName}' in DropZone.");
        return false;
    }
}
