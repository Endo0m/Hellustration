using UnityEngine;
using DG.Tweening;

public class CollectibleItem : MonoBehaviour, ICollectible
{
    [SerializeField] private Sprite itemSprite; // Спрайт предмета
    [SerializeField] private string itemName; // Уникальное имя предмета (ID)
    [SerializeField] private float fadeDuration = 1f; // Длительность исчезновения

    public Sprite ItemSprite => itemSprite;
    public string ItemName => itemName;

    public void Collect(CollectController player)
    {
        Debug.Log($"Предмет {itemName} подобран!");
        player.AddItemToInventory(this);
    }

    public void FadeOutAndDisable()
    {
        // Плавное исчезновение с использованием DoTween
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.DOFade(0, fadeDuration).OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            Debug.LogWarning($"SpriteRenderer не найден на объекте {gameObject.name}");
            gameObject.SetActive(false); // Если компонент не найден, просто деактивируем
        }
    }
}
