using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    [SerializeField] private VideoClip englishVideo;
    [SerializeField] private VideoClip russianVideo;
    [SerializeField] string nameScene;
    private bool isSkipped = false;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Set appropriate video based on current language
        videoPlayer.clip = LocalizationManager.Instance.CurrentLanguage == "ru_RU" ? russianVideo : englishVideo;

        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();

        // Subscribe to language changes
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }
    }

    private void OnLanguageChanged()
    {
        // Update video when language changes
        videoPlayer.clip = LocalizationManager.Instance.CurrentLanguage == "ru_RU" ? russianVideo : englishVideo;
        videoPlayer.Play();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SkipCutscene();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        LoadGameScene();
    }

    void SkipCutscene()
    {
        if (!isSkipped)
        {
            isSkipped = true;
            videoPlayer.Stop();
            LoadGameScene();
        }
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene(nameScene);
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }
}