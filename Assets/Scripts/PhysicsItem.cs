using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PhysicsItem : MonoBehaviour
{
    [SerializeField] private string fallSoundKey = "item_fall";
    private bool isSoundPlayed = false;
    private AudioSource audioSource;
    private SoundManager soundManager;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ѕровер€ем что это первое столкновение с землей
        if (!isSoundPlayed && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isSoundPlayed = true;
            soundManager.PlaySound(fallSoundKey, audioSource);
        }
    }
}
