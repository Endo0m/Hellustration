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
    [SerializeField] private Button closeHintButton; // Кнопка закрытия подсказки
    [SerializeField] private float hintPanelAnimationDuration = 0.3f;

    [Header("Shadow Settings")]
    [SerializeField] private ShadowFollower shadow;

    private bool isReadingHint = false;
    [Header("Sound Settings")]
    [SerializeField] private string hintOpenSoundKey = "hint_open"; // Звук открытия подсказки
    private AudioSource audioSource;
    private SoundManager soundManager;

    private Dictionary<string, DraggableUIItem> inventoryItems = new Dictionary<string, DraggableUIItem>();
    private List<HintItem> hintInventory = new List<HintItem>();
    private int collectedItemCount = 0;

    private void Start()
    {
        // Инициализация звуковой системы
        audioSource = gameObject.AddComponent<AudioSource>();
        soundManager = FindObjectOfType<SoundManager>();

        // Инициализация существующих слотов
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
        // Опционально: добавляем возможность закрытия по клавише Escape
        if (isReadingHint && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseHint();
        }
    }

    public void AddItemToInventory(CollectibleItem item)
    {
        // Воспроизводим звук подбора предмета
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
        // Воспроизводим звук подбора подсказки
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

        // Анимация появления панели
        hintPanel.transform.localScale = Vector3.zero;
        hintPanel.transform.DOScale(Vector3.one, hintPanelAnimationDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // Важно: анимация будет работать при остановленном времени

        // Проверяем, первое ли это прочтение подсказки
        if (!hintItem.WasReadFirstTime)
        {
            if (shadow != null)
            {
                shadow.AppearAndSpeak(hintItem.ShadowSoundKey);
            }
            hintItem.SetReadFirstTime();
        }
    }

    private void CloseHint()
    {
        if (!isReadingHint) return;

        // Анимация исчезновения панели
        hintPanel.transform.DOScale(Vector3.zero, hintPanelAnimationDuration)
            .SetEase(Ease.InBack)
            .SetUpdate(true) // Анимация работает при остановленном времени
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
        // Обеспечиваем восстановление времени при уничтожении объекта
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