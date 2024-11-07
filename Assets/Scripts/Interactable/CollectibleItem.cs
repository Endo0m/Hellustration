using UnityEngine;

public class CollectibleItem : MonoBehaviour, ICollectible
{
    [SerializeField] private Sprite itemSprite; // Спрайт предмета
    [SerializeField] private string itemName; // Имя предмета (уникальный идентификатор)

    public Sprite ItemSprite => itemSprite;
    public string ItemName => itemName;

    public void Collect(CollectController player)
    {
        Debug.Log($"Предмет {itemName} подобран!");
        player.AddItemToInventory(this);
        gameObject.SetActive(false); // Деактивируем предмет после подбора
    }
}
