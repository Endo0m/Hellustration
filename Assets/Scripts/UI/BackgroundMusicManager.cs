using System.Collections;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("����������� �����")]
    public AudioClip[] musicTracks; // ������ � ������������ �������

    [Header("��������� ���������������")]
    public bool playRandomly = false; // ����� ������� ���������������: �������� ��� �� �������

    private AudioSource audioSource; // �������� �����
    private int currentTrackIndex = 0; // ������ �������� �����

    void Start()
    {
        // �������� ��������� AudioSource �� �������
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // ���� AudioSource �� ��������, ��������� ���
        }

        // ��������, ��� ������ ������ �� ������
        if (musicTracks.Length > 0)
        {
            PlayNextTrack(); // �������� ��������������� ������� �����
        }
        else
        {
            Debug.LogWarning("��� ��������� ����������� ������ ��� ���������������.");
        }
    }

    void Update()
    {
        // ��������, ����������� �� ��������������� �������� �����
        if (!audioSource.isPlaying)
        {
            PlayNextTrack(); // ���� ������� ���� ����������, �������� ��������������� ����������
        }
    }

    // ����� ��� ������ ���������� �����
    void PlayNextTrack()
    {
        if (musicTracks.Length == 0) return;

        if (playRandomly)
        {
            // ��������������� ���������� �����
            currentTrackIndex = Random.Range(0, musicTracks.Length);
        }
        else
        {
            // ��������������� ���������� ����� �� �������
            currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        }

        // ��������������� �����
        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.Play();
    }
}
