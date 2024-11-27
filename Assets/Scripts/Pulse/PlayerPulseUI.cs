using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerPulseUI : MonoBehaviour
{
    [Header("Настройки UI")]
    [SerializeField] private Image pulseImage;
    [SerializeField] private AudioSource audioSource;

    [Header("Настройки нормального сердцебиения")]
    [SerializeField] private float normalScale = 1.5f;
    [SerializeField] private float normalDuration = 1.5f;

    [Header("Настройки испуга")]
    [SerializeField] private float panicScale = 2f;
    [SerializeField] private float panicDuration = 0.8f;

    [Header("Настройки звука")]
    [SerializeField] private string calmHeartbeat = "normal_heartbeat";
    [SerializeField] private string panicHeartbeat = "fast_heartbeat";

    private Tween currentPulseTween;
    private bool isPanicked = false;

    private void Start()
    {
        // Сразу запускаем нормальное сердцебиение при старте
        SetNormalState();
    }

    public void SetPanicState()
    {
        if (isPanicked) return;
        isPanicked = true;
        UpdatePulseAnimation();
        UpdateSound(true);
    }

    public void SetNormalState()
    {
        if (!isPanicked && currentPulseTween != null) return;
        isPanicked = false;
        UpdatePulseAnimation();
        UpdateSound(false);
    }

    private void UpdatePulseAnimation()
    {
        if (currentPulseTween != null && currentPulseTween.IsActive())
        {
            currentPulseTween.Kill(true);
        }

        float currentScale = isPanicked ? panicScale : normalScale;
        float currentDuration = isPanicked ? panicDuration : normalDuration;

        pulseImage.transform.localScale = Vector3.one;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(pulseImage.transform.DOScale(currentScale, currentDuration * 0.4f)
            .SetEase(Ease.OutQuad));
        sequence.Append(pulseImage.transform.DOScale(1f, currentDuration * 0.6f)
            .SetEase(Ease.InOutQuad));

        currentPulseTween = sequence.SetLoops(-1);
    }

    private void UpdateSound(bool panic)
    {
        if (!audioSource) return;

        string soundKey = panic ? panicHeartbeat : calmHeartbeat;
        AudioClip clip = SoundManager.Instance.GetAudioClip(soundKey);

        if (clip != null && (audioSource.clip != clip || !audioSource.isPlaying))
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        if (currentPulseTween != null && currentPulseTween.IsActive())
        {
            currentPulseTween.Kill();
        }
    }
}
