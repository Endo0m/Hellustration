using UnityEngine;

public class Hideout : MonoBehaviour, IInteractable
{
    [SerializeField] private bool faceRightWhenHidden = true;
    [SerializeField] private string hideSoundKey = "hide_sound";

    private SoundManager soundManager;
    private AudioSource audioSource;

    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Interact(GameObject interactor)
    {
        PlayerController player = interactor.GetComponent<PlayerController>();
        if (player != null)
        {

            if (soundManager != null && !string.IsNullOrEmpty(hideSoundKey))
            {
                soundManager.PlaySound(hideSoundKey, audioSource);
            }

            player.HideInObject(transform, faceRightWhenHidden);
        }
    }
}