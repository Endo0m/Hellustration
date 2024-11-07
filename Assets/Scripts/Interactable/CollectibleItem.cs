using UnityEngine;

public class CollectibleItem : MonoBehaviour, ICollectible
{
    [SerializeField] private Sprite itemSprite; // ������ ��������
    [SerializeField] private string itemName; // ��� �������� (���������� �������������)

    public Sprite ItemSprite => itemSprite;
    public string ItemName => itemName;

    public void Collect(CollectController player)
    {
        Debug.Log($"������� {itemName} ��������!");
        player.AddItemToInventory(this);
        gameObject.SetActive(false); // ������������ ������� ����� �������
    }
}
