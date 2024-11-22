using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform mainMenuPanel;
    [SerializeField] private RectTransform levelsPanel;
    [SerializeField] private RectTransform settingsPanel;
    [SerializeField] private RectTransform creditsPanel;

    [Header("Animation Settings")]
    [SerializeField] private float turnDuration = 0.75f;
    [SerializeField] private float targetRotationY = -90f;
    private RectTransform currentPanel;

    private void Start()
    {
        currentPanel = mainMenuPanel;
        SetupInitialState();
    }

    private void SetupInitialState()
    {
        // �������� ��� ������ ����� �������� ����
        levelsPanel.gameObject.SetActive(false);
        settingsPanel.gameObject.SetActive(false);
        creditsPanel.gameObject.SetActive(false);

        // ���������� ��� ��������
        mainMenuPanel.localRotation = Quaternion.identity;
        levelsPanel.localRotation = Quaternion.Euler(0, 90f, 0);
        settingsPanel.localRotation = Quaternion.Euler(0, 90f, 0);
        creditsPanel.localRotation = Quaternion.Euler(0, 90f, 0);
    }

    public void SwitchToPanel(RectTransform targetPanel)
    {
        if (currentPanel == targetPanel) return;

        // ���������� ������� ������
        targetPanel.gameObject.SetActive(true);

        // �������� ������� ������ (������� �����)
        Sequence sequence = DOTween.Sequence();

        sequence.Append(currentPanel.DOLocalRotate(new Vector3(0, targetRotationY, 0), turnDuration)
            .SetEase(Ease.InOutSine));

        sequence.Join(targetPanel.DOLocalRotate(Vector3.zero, turnDuration)
            .SetEase(Ease.InOutSine));

        sequence.OnComplete(() => {
            currentPanel.gameObject.SetActive(false);
            currentPanel = targetPanel;
        });
    }

    public void ReturnToMainMenu()
    {
        if (currentPanel == mainMenuPanel) return;

        // ���������� ������� ����
        mainMenuPanel.gameObject.SetActive(true);

        // �������� ��������
        Sequence sequence = DOTween.Sequence();

        sequence.Append(currentPanel.DOLocalRotate(new Vector3(0, 90f, 0), turnDuration)
            .SetEase(Ease.InOutSine));

        sequence.Join(mainMenuPanel.DOLocalRotate(Vector3.zero, turnDuration)
            .SetEase(Ease.InOutSine));

        sequence.OnComplete(() => {
            currentPanel.gameObject.SetActive(false);
            currentPanel = mainMenuPanel;
        });
    }

    // ������ ��� ������
    public void OpenLevels() => SwitchToPanel(levelsPanel);
    public void OpenSettings() => SwitchToPanel(settingsPanel);
    public void OpenCredits() => SwitchToPanel(creditsPanel);

    public void StartGame()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        DOTween.KillAll();
        Application.Quit();
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}