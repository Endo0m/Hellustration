using System.Collections;
using UnityEngine;

public class ScreamerTrigger : MonoBehaviour
{
    [SerializeField] private GameObject screamerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float screamerLifetime = 3.0f;
    [SerializeField] private PulseController pulseController;
    [SerializeField] private string screamSound;

    private bool hasTriggered = false;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            SoundManager.Instance.PlaySound(screamSound, audioSource);
            pulseController.Panic(); // Заменили TriggerScare на Panic
            StartCoroutine(SpawnScreamer());
        }
    }

    private System.Collections.IEnumerator SpawnScreamer()
    {
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        GameObject screamer = Instantiate(screamerPrefab, spawnPosition, spawnRotation);
        yield return new WaitForSeconds(screamerLifetime);

        if (screamer != null)
        {
            Destroy(screamer);
        }
    }
}