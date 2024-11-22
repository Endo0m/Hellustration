using UnityEngine;
using DG.Tweening;

public class HintItem : MonoBehaviour, ICollectible
{
    [SerializeField] private string hintText;
    [SerializeField] private GameObject hintUIPrefab;
    [SerializeField] private string collectSoundKey = "hint_collect";
    [SerializeField] private string shadowSoundKey = "shadow_hint"; // Ключ для звука тени
    [SerializeField] private float fadeDuration = 1f;

    private bool isCollected = false;
    private bool wasReadFirstTime = false;

    public string HintText => hintText;
    public string CollectSoundKey => collectSoundKey;
    public string ShadowSoundKey => shadowSoundKey;
    public bool WasReadFirstTime => wasReadFirstTime;

    public void SetReadFirstTime()
    {
        wasReadFirstTime = true;
    }

    public void Collect(CollectController player)
    {
        if (isCollected) return;

        isCollected = true;
        player.AddHintToInventory(this);
        FadeOutAndDisable();
    }

    private void FadeOutAndDisable()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.DOFade(0, fadeDuration).OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            UnityEngine.UI.Image image = GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                image.DOFade(0, fadeDuration).OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                Debug.LogWarning($"Не найден компонент для анимации исчезновения на объекте {gameObject.name}");
                gameObject.SetActive(false);
            }
        }
    }

    // Для возможности сброса состояния (например, при перезагрузке уровня)
    public void Reset()
    {
        isCollected = false;
    }
}