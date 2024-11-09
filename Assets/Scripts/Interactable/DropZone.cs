using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour
{
    [System.Serializable]
    public class ItemCombination
    {
        public string itemName; // ��� ��������, ������� ������������
        public string targetItemName; // ��� �������� �������
        public GameObject resultPrefab; // ������ ����������
    }

    [SerializeField] private List<ItemCombination> itemCombinations; // ������ ��������� ����������

    public bool TryCombineWithItem(string itemName, string targetName)
    {
        foreach (var combination in itemCombinations)
        {
            Debug.Log($"Checking combination: itemName = {itemName}, targetItemName = {combination.targetItemName}");
            if (combination.itemName == itemName && combination.targetItemName == targetName)
            {
                // �������� �������� ������� � DropZone
                Destroy(gameObject);

                // �������� ������ ������� �� ����� ��������
                Instantiate(combination.resultPrefab, transform.position, Quaternion.identity);
                Debug.Log($"Combined item '{itemName}' with '{combination.targetItemName}' to create '{combination.resultPrefab.name}'.");
                return true;
            }
        }

        Debug.LogWarning($"Failed to combine item '{itemName}' in DropZone.");
        return false;
    }
}
