using System.Collections.Generic;
using UnityEngine;

public class ItemModifierZone : MonoBehaviour
{
    [System.Serializable]
    public class ItemModification
    {
        public string inputItemName; // ��� �������� ��������
        public GameObject modifiedPrefab; // ���������������� ������
    }

    [SerializeField] private List<ItemModification> itemModifications; // ������ ��������� �����������

    public bool TryModifyItem(GameObject targetItem, string itemName)
    {
        foreach (var modification in itemModifications)
        {
            if (modification.inputItemName == itemName)
            {
                // ���� ������ � ������ "TargetPointInstance" ��� ���������� ����������������� ��������
                GameObject targetPoint = GameObject.Find("TargetPointInstance");
                if (targetPoint == null)
                {
                    Debug.LogWarning("TargetPointInstance not found in the scene.");
                    return false;
                }

                Vector3 position = targetPoint.transform.position;
                Quaternion rotation = targetPoint.transform.rotation;

                // ������� ������ ������
                if (targetItem != null)
                {
                    Destroy(targetItem); // ������� ������� �������
                }

                // ������� ����� ������� � ������� ������� �����
                Instantiate(modification.modifiedPrefab, position, rotation);
                Debug.Log($"Item '{itemName}' was modified and created at TargetPointInstance position.");

                // ���������� ItemSpawner
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
