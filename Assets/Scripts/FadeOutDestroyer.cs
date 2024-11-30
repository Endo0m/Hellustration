using UnityEngine;
using DG.Tweening;

public class FadeOutDestroyer : MonoBehaviour
{
    [SerializeField] private float delayBeforeFade = 2f; // �������� ����� ������� ������������
    [SerializeField] private float fadeDuration = 1f; // ������������ �������� ������������
    [SerializeField] private SpriteRenderer spriteRenderer; // ��� 2D ��������

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // ������������������ ��������
        Sequence sequence = DOTween.Sequence();

        // ���� �������� �����
        sequence.AppendInterval(delayBeforeFade);

        // ������ ������ ������������ �� 0
        sequence.Append(spriteRenderer.DOFade(0f, fadeDuration));

        // ���������� ������ ����� ���������� ��������
        sequence.OnComplete(() => Destroy(gameObject));
    }

    private void OnDestroy()
    {
        // ������� �������� ��� ����������� �������
        DOTween.Kill(spriteRenderer);
    }
}