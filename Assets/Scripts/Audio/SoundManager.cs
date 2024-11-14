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
    }

    // ����� ��� ������ ����� �� �����
    public void PlaySound(string soundName)
    {
        Sound sound = System.Array.Find(sounds, s => s.name == soundName);

        if (sound.clip != null)
        {
            // �������� ������ GameObject ��� ����� ���������
            GameObject soundGameObject = new GameObject("AudioSource_" + soundName);
            AudioSource newAudioSource = soundGameObject.AddComponent<AudioSource>();

            // ������������� ��������� ��������� � ����������� ����
            newAudioSource.clip = sound.clip;
            newAudioSource.Play();

            // ������� ������ ����� ��������� ��������������� �����
            Destroy(soundGameObject, sound.clip.length);
        }
        else
        {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }
}
