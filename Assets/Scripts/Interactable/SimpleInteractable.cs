using UnityEngine;

public class SimpleInteractable : MonoBehaviour
{
    [SerializeField] private Animator animator; // �������� ��� ���������� ���������
    [SerializeField] private string animationTrigger = "Wobble"; // �������� �������� ��� ��������
    [SerializeField] private AudioClip[] audioClips; // ������ ������
    [SerializeField] private AudioSource audioSource; // �������� ����� ��� ��������������� �����

    private int currentAudioIndex = 0; // ������� ������ ����� ��� ������������

    private void OnMouseDown()
    {
        // ��������� ������� �������� � ��������� �
        if (animator != null)
        {
            animator.SetTrigger(animationTrigger);
        }

        // ����������� ���� �� ������� �� �������
        PlayAudio();
    }

    private void PlayAudio()
    {
        if (audioClips != null && audioClips.Length > 0 && audioSource != null)
        {
            // ����������� ������� ����
            audioSource.clip = audioClips[currentAudioIndex];
            audioSource.Play();

            // ��������� � ���������� �����
            currentAudioIndex = (currentAudioIndex + 1) % audioClips.Length; // ����������� ������������ �������
        }
    }
}
