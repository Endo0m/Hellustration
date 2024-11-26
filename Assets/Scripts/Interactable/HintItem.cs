using UnityEngine;
using DG.Tweening;

public class HintItem : MonoBehaviour, ICollectible
{
    [Header("Sprites")]
    [SerializeField] private Sprite collectibleSprite;
    [Header("Localization Keys")]
    [SerializeField] private string hintTextKey;

    [Header("Sound Keys")]
    [SerializeField] private string collectSoundKey = "hint_collect";
    [SerializeField] private string shadowSoundKey = "shadow_hint";
    [SerializeField] private string shadowSoundKeyRussian = "shadow_hint_ru";

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;

    private bool isCollected = false;
    private bool wasReadFirstTime = false;
    public Sprite CollectibleSprite => collectibleSprite;
    public string HintText => LocalizationManager.Instance.GetLocalizedValue(hintTextKey);
    public string CollectSoundKey => collectSoundKey;
    public string CurrentShadowSoundKey =>
        LocalizationManager.Instance.CurrentLanguage == "ru_RU" ? shadowSoundKeyRussian : shadowSoundKey;
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
            var image = GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                image.DOFade(0, fadeDuration).OnComplete(() => gameObject.SetActive(false));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Reset()
    {
        isCollected = false;
        gameObject.SetActive(true);
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = 1f;
            renderer.color = color;
        }
        else
        {
            var image = GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                Color color = image.color;
                color.a = 1f;
                image.color = color;
            }
        }
    }
}