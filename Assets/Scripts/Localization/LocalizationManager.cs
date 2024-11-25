using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    [System.Serializable]
    public class LocalizationData
    {
        public LocalizationItem[] items;
    }

    [System.Serializable]
    public class LocalizationItem
    {
        public string key;
        public string value;
    }

    private string currentLanguage;
    private Dictionary<string, string> localizedText;

    [SerializeField] private TMP_Dropdown languageDropdown;

    public static bool isReady = false;
    public delegate void ChangeLangText();
    public event ChangeLangText OnLanguageChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManager();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public string CurrentLanguage => currentLanguage;

    private void InitializeManager()
    {
        if (!PlayerPrefs.HasKey("Language"))
        {
            string defaultLanguage = Application.systemLanguage == SystemLanguage.Russian ? "ru_RU" : "en_US";
            PlayerPrefs.SetString("Language", defaultLanguage);
        }

        currentLanguage = PlayerPrefs.GetString("Language");
        LoadLocalizedText(currentLanguage);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeDropdown();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeDropdown();
    }

    private void InitializeDropdown()
    {
        if (languageDropdown == null)
        {
            languageDropdown = FindObjectOfType<TMP_Dropdown>();
        }

        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(new List<string> { "English", "Русский" });
            languageDropdown.value = GetLanguageIndex(currentLanguage);
            languageDropdown.onValueChanged.RemoveAllListeners();
            languageDropdown.onValueChanged.AddListener(OnLanguageSelected);
        }
    }

    private int GetLanguageIndex(string langCode)
    {
        return langCode == "ru_RU" ? 1 : 0;
    }

    private string GetLanguageCode(int index)
    {
        return index == 1 ? "ru_RU" : "en_US";
    }

    private void OnLanguageSelected(int index)
    {
        string newLanguage = GetLanguageCode(index);
        ChangeLanguage(newLanguage);
    }

    public void ChangeLanguage(string langCode)
    {
        if (langCode != currentLanguage)
        {
            LoadLocalizedText(langCode);
            if (languageDropdown != null)
            {
                languageDropdown.value = GetLanguageIndex(langCode);
            }
            PlayerPrefs.SetString("Language", langCode);
            OnLanguageChanged?.Invoke();
        }
    }

    private void LoadLocalizedText(string langName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Languages", langName + ".json");

        if (File.Exists(path))
        {
            string dataAsJson = File.ReadAllText(path);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            localizedText = new Dictionary<string, string>();
            foreach (var item in loadedData.items)
            {
                localizedText[item.key] = item.value;
            }

            currentLanguage = langName;
            isReady = true;
            OnLanguageChanged?.Invoke();
        }
        else
        {
            Debug.LogError($"Localization file not found at path: {path}");
        }
    }

    public string GetLocalizedValue(string key)
    {
        if (localizedText != null && localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }
        return key;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}