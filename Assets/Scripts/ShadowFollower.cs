using UnityEngine;
using DG.Tweening;

public class ShadowFollower : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(1f, 1f, 0f);
    [SerializeField] private float appearDuration = 1f;
    [SerializeField] private float stayDuration = 3f;
    [SerializeField] private float disappearDuration = 1f;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Animator animator;
    private SoundManager soundManager;
    private bool isAnimating = false;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        // ������������� ������� �����������
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }

        // ��������� ��������� ��� ������ ���������� �� timeScale
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        audioSource.priority = 80;
        audioSource.ignoreListenerPause = true;
        soundManager = FindObjectOfType<SoundManager>();

        // ���������� ���� ��������
        Color startColor = spriteRenderer.color;
        startColor.a = 0;
        spriteRenderer.color = startColor;
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            float deltaTime = Time.timeScale == 0 ? Time.unscaledDeltaTime : Time.deltaTime;
            Vector3 targetPosition = playerTransform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * deltaTime);
        }
    }

    public void AppearAndSpeak(string soundKey)
    {
        if (isAnimating) return;
        isAnimating = true;

        // ��������� �������� idle
        if (animator != null)
        {
            animator.SetBool("IsVisible", true);
        }

        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(true); // �������� ����� �������� ��� ������������� �������

        // ���������
        sequence.Append(spriteRenderer.DOFade(1f, appearDuration));

        // ��������������� �����
        sequence.AppendCallback(() =>
        {
            if (soundManager != null && audioSource != null)
            {
                soundManager.PlaySound(soundKey, audioSource);
            }
        });

        // ��������
        sequence.AppendInterval(stayDuration);

        // ������������
        sequence.Append(spriteRenderer.DOFade(0f, disappearDuration));

        // ���������� ��������
        sequence.OnComplete(() =>
        {
            isAnimating = false;
            if (animator != null)
            {
                animator.SetBool("IsVisible", false);
            }
        });
    }

    private void OnDestroy()
    {
        DOTween.Kill(spriteRenderer);
    }

    // �����������: ����� ��� ����������� ������� ����
    public void Hide()
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
        }
        if (animator != null)
        {
            animator.SetBool("IsVisible", false);
        }
        isAnimating = false;
    }
}