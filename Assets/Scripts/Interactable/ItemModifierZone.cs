using System.Collections.Generic;
using UnityEngine;

public class ItemModifierZone : MonoBehaviour
{
    [System.Serializable]
    public class ItemModification
    {
        public string inputItemName; // Имя входного предмета
        public GameObject modifiedPrefab; // Модифицированный префаб
    }

    [SerializeField] private List<ItemModification> itemModifications; // Список возможных модификаций

    public bool TryModifyItem(GameObject targetItem, string itemName)
    {
        foreach (var modification in itemModifications)
        {
            if (modification.inputItemName == itemName)
            {
                // Ищем объект с именем "TargetPointInstance" для размещения модифицированного предмета
                GameObject targetPoint = GameObject.Find("TargetPointInstance");
                if (targetPoint == null)
                {
                    Debug.LogWarning("TargetPointInstance not found in the scene.");
                    return false;
                }

                Vector3 position = targetPoint.transform.position;
                Quaternion rotation = targetPoint.transform.rotation;

                // Удаляем старый объект
                if (targetItem != null)
                {
                    Destroy(targetItem); // Удаляем текущий предмет
                }

                // Создаем новый предмет в позиции целевой точки
                Instantiate(modification.modifiedPrefab, position, rotation);
                Debug.Log($"Item '{itemName}' was modified and created at TargetPointInstance position.");

                // Уведомляем ItemSpawner
                ItemSpawner itemSpawner = FindObjectOfType<ItemSpawner>();
                if (itemSpawner != null)
                {
                    itemSpawner.SetModifiedItem(modification.modifiedPrefab);
                }

                return true;
            }
        }

        Debug.LogWarning($"No modification found for item '{itemName}' in ItemModifierZone.");
        return false;
    }
}
