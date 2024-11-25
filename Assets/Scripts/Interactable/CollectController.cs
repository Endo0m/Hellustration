using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CollectController : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private Transform inventoryUIParent;
    [SerializeField] private int totalItemsToCollect = 3;

    [Header("Hint Inventory Settings")]
    [SerializeField] private Transform hintInventoryUIParent;
    [SerializeField] private GameObject hintSlotPrefab;
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintTextComponent;

    [Header("Hint Panel Settings")]
    [SerializeField] private Button closeHintButton;
    [SerializeField] private float hintPanelAnimationDuration = 0.3f;

    [Header("Shadow Settings")]
    [SerializeField] private ShadowFollower shadow;

    [Header("Sound Settings")]
    [SerializeField] private string hintOpenSoundKey = "hint_open";
    private AudioSource audioSource;
    private SoundManager soundManager;

    private Dictionary<string, DraggableUIItem> inventoryItems = new Dictionary<string, DraggableUIItem>();
    private List<HintItem> hintInventory = new List<HintItem>();
    private int collectedItemCount = 0;
    private bool isReadingHint = false;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        soundManager = FindObjectOfType<SoundManager>();

        foreach (Transform child in inventoryUIParent)
        {
            DraggableUIItem draggableItem = child.GetComponent<DraggableUIItem>();
            if (draggableItem != null && !string.IsNullOrEmpty(draggableItem.ItemName))
            {
                inventoryItems[draggableItem.ItemName] = draggableItem;
            }
        }

        if (closeHintButton != null)
        {
            closeHintButton.onClick.AddListener(CloseHint);
        }
    }

    private void Update()
    {
        if (isReadingHint && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseHint();
        }
    }

    public void AddItemToInventory(CollectibleItem item)
    {
        PlaySound(item.CollectSoundKey);

        string itemName = item.ItemName;
        if (inventoryItems.ContainsKey(itemName))
        {
            DraggableUIItem uiItem = inventoryItems[itemName];
            uiItem.UpdateSprite(item.ItemSprite);
            collectedItemCount++;

            if (collectedItemCount >= totalItemsToCollect)
            {
                EnableItemDragging();
            }
            item.FadeOutAndDisable();
        }
        else
        {
            Debug.LogWarning($"Слот для предмета с именем {itemName} не найден!");
        }
    }

    public void AddHintToInventory(HintItem hintItem)
    {
        PlaySound(hintItem.CollectSoundKey);
        hintInventory.Add(hintItem);
        DisplayHintInUI(hintItem);
    }

    private void ShowHint(string hintText, HintItem hintItem)
    {
        isReadingHint = true;
        PauseGame();

        PlaySound(hintOpenSoundKey);

        hintPanel.SetActive(true);
        hintTextComponent.text = hintText;

        hintPanel.transform.localScale = Vector3.zero;
        hintPanel.transform.DOScale(Vector3.one, hintPanelAnimationDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        if (!hintItem.WasReadFirstTime)
        {
            if (shadow != null)
            {
                // Используем CurrentShadowSoundKey вместо ShadowSoundKey
                shadow.AppearAndSpeak(hintItem.CurrentShadowSoundKey);
            }
            hintItem.SetReadFirstTime();
        }
    }

    private void CloseHint()
    {
        if (!isReadingHint) return;

        hintPanel.transform.DOScale(Vector3.zero, hintPanelAnimationDuration)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                hintPanel.SetActive(false);
                ResumeGame();
                isReadingHint = false;
            });
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void DisplayHintInUI(HintItem hintItem)
    {
        GameObject slot = Instantiate(hintSlotPrefab, hintInventoryUIParent);
        Button button = slot.GetComponent<Button>();
        if (button == null)
        {
            button = slot.AddComponent<Button>();
        }
        button.onClick.AddListener(() => ShowHint(hintItem.HintText, hintItem));
    }

    private void EnableItemDragging()
    {
        foreach (var item in inventoryItems.Values)
        {
            item.SetDraggable(true);
        }
    }

    private void PlaySound(string soundKey)
    {
        if (soundManager != null && audioSource != null && !string.IsNullOrEmpty(soundKey))
        {
            soundManager.PlaySound(soundKey, audioSource);
        }
    }
}