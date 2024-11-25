using UnityEngine;
using TMPro;

public class LocalizedTextComponent : MonoBehaviour
{
    [SerializeField] private string key;
    private TextMeshProUGUI textComponent;
    private bool isInitialized = false;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        InitializeLocalization();
    }

    private void InitializeLocalization()
    {
        if (LocalizationManager.Instance != null && !isInitialized)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
            UpdateText();
            isInitialized = true;
        }
        else
        {
            Invoke("InitializeLocalization", 0.1f);
        }
    }

    private void UpdateText()
    {
        if (textComponent != null && LocalizationManager.Instance != null)
        {
            textComponent.text = LocalizationManager.Instance.GetLocalizedValue(key);
        }
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }
}