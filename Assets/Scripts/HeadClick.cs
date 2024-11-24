using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadClick : MonoBehaviour
{
    [SerializeField] private Rigidbody2D head;
   // [SerializeField] private ClickScreamTrigger clickScreamTrigger;
    [SerializeField] private float strange;
    private bool isClicked = false;
    private AudioSource audioSource;
    [SerializeField] private string fallSoundKey = "item_fall";

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnMouseDown()
    {
        if (!isClicked)
        {
            // clickScreamTrigger.PulseActive();
            SoundManager.Instance.PlaySound(fallSoundKey, audioSource);

            head.gravityScale = 6;
            head.AddForce(Vector2.up * strange, ForceMode2D.Impulse);
            isClicked = true;
        }
    }

    
}
