using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CollectController : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private Transform inventoryUIParent;
    [SerializeField] private int totalItemsToCollect = 3;

    [Header("Hint Inventory Settings")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintTextComponent;
    [SerializeField] private Image[] existingScrollSlots; // Существующие слоты для свитков в UI

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
    private HintItem currentHintItem;
    private Pause pauseController;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        soundManager = FindObjectOfType<SoundManager>();
        pauseController = FindObjectOfType<Pause>();

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
            uiItem.SetDraggable(true);
            /* if (collectedItemCount >= totalItemsToCollect)
             {
                 EnableItemDragging();
             }*/
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

        // Находим соответствующий слот по имени свитка (например, "scroll_1" ищет existingScrollSlots[0])
        int scrollIndex = GetScrollIndex(hintItem.name) - 1;
        if (scrollIndex >= 0 && scrollIndex < existingScrollSlots.Length)
        {
            Image scrollSlot = existingScrollSlots[scrollIndex];
            // Обновляем спрайт в UI
            scrollSlot.sprite = hintItem.CollectibleSprite;

            Button button = scrollSlot.GetComponent<Button>();
            if (button == null)
            {
                button = scrollSlot.gameObject.AddComponent<Button>();
            }
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ShowHint(hintItem.HintText, hintItem));
        }
    }

    private int GetScrollIndex(string scrollName)
    {
        // Извлекаем номер из имени свитка (scroll_1 -> 1)
        string numberPart = scrollName.Replace("scroll_", "").Replace("(Clone)", "");
        int index;
        return int.TryParse(numberPart, out index) ? index : -1;
    }

    private void ShowHint(string hintText, HintItem hintItem)
    {
        if (isReadingHint) return;

        currentHintItem = hintItem;
        isReadingHint = true;
        if (pauseController != null)
        {
            pauseController.SetHintOpen(true);
        }

        PauseGame();
        PlaySound(hintOpenSoundKey);

        hintPanel.SetActive(true);
        hintTextComponent.text = hintText;

        hintPanel.transform.localScale = Vector3.zero;
        hintPanel.transform.DOScale(Vector3.one, hintPanelAnimationDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
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

                if (pauseController != null)
                {
                    pauseController.SetHintOpen(false);
                }

                // Показываем тень после закрытия записки
                if (!currentHintItem.WasReadFirstTime)
                {
                    if (shadow != null)
                    {
                        shadow.AppearAndSpeak(currentHintItem.CurrentShadowSoundKey);
                    }
                    currentHintItem.SetReadFirstTime();
                }
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