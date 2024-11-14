using UnityEngine;

// ��������� ��� �������� ����� � �����
[System.Serializable]
public struct Sound
{
    public string name;  // ��� �����
    public AudioClip clip; // ������ �� ���������
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds; // ������ ������
    private AudioSource audioSource; // �������� ��� ��������������� ������

    // Singleton ��� �������� �������
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        // ��������, ��� � ��� ������ ���� ��������� SoundManager
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // ��������� ������ ����� �������
        }

        audioSource = GetComponent<AudioSource>();
    }

    // ����� ��� ������ ����� �� �����
    public void PlaySound(string soundName)
    {
        Sound sound = System.Array.Find(sounds, s => s.name == soundName);

        if (sound.clip != null)
        {
            audioSource.PlayOneShot(sound.clip); // ��������������� �����
        }
        else
        {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }
}
