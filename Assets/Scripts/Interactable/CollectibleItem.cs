using UnityEngine;
using DG.Tweening;

public class CollectibleItem : MonoBehaviour, ICollectible
{
    [SerializeField] private Sprite itemSprite; // ������ ��������
    [SerializeField] private string itemName; // ���������� ��� �������� (ID)
    [SerializeField] private float fadeDuration = 1f; // ������������ ������������

    public Sprite ItemSprite => itemSprite;
    public string ItemName => itemName;

    public void Collect(CollectController player)
    {
        Debug.Log($"������� {itemName} ��������!");
        player.AddItemToInventory(this);
    }

    public void FadeOutAndDisable()
    {
        // ������� ������������ � �������������� DoTween
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.DOFade(0, fadeDuration).OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            Debug.LogWarning($"SpriteRenderer �� ������ �� ������� {gameObject.name}");
            gameObject.SetActive(false); // ���� ��������� �� ������, ������ ������������
        }
    }
}
