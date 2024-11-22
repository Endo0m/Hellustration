using UnityEngine;
using DG.Tweening;

public class CollectibleItem : MonoBehaviour, ICollectible
{
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private string itemName;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string collectSoundKey = "item_collect";

    private bool isCollected = false;

    public Sprite ItemSprite => itemSprite;
    public string ItemName => itemName;
    public string CollectSoundKey => collectSoundKey;

    public void Collect(CollectController player)
    {
        if (isCollected) return;

        isCollected = true;
        Debug.Log($"Предмет {itemName} подобран!");
        player.AddItemToInventory(this);
    }

    public void FadeOutAndDisable()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.DOFade(0, fadeDuration).OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            Debug.LogWarning($"SpriteRenderer не найден на объекте {gameObject.name}");
            gameObject.SetActive(false);
        }
    }

    // Для возможности сброса состояния
    public void Reset()
    {
        isCollected = false;
        if (TryGetComponent<SpriteRenderer>(out var renderer))
        {
            var color = renderer.color;
            color.a = 1f;
            renderer.color = color;
        }
        gameObject.SetActive(true);
    }
}