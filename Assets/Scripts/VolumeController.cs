using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public static VolumeController Instance { get; private set; }
    private Slider currentVolumeSlider;
    private const string VOLUME_SLIDER_NAME = "VolumeSlider"; // ��� �������� ��� ������

    private void Awake()
    {
        // ���������� ���������
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // ������������� �� ������� �������� �����
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        // ������������ �� ������� ��� ����������� �������
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // ���� ������� �� ����� �����
        FindAndInitializeSlider();
    }

    private void FindAndInitializeSlider()
    {
        // ������� ������� �� �����
        GameObject sliderObject = GameObject.Find(VOLUME_SLIDER_NAME);
        if (sliderObject != null)
        {
            currentVolumeSlider = sliderObject.GetComponent<Slider>();
            if (currentVolumeSlider != null)
            {
                // ������������� ������� �������� ���������
                currentVolumeSlider.value = AudioListener.volume;
                // ������������� �� ��������� �������� ��������
                currentVolumeSlider.onValueChanged.RemoveAllListeners();
                currentVolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            }
        }
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    // ����� ��� ��������� ������� ��������� (����� �����������)
    public float GetCurrentVolume()
    {
        return AudioListener.volume;
    }
}