using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickScreamTrigger : MonoBehaviour
{
   [SerializeField] private PulseController pulseController;
   [SerializeField] private SpriteRenderer spriteRenderer;
   [SerializeField] private Collider2D colliderr2D;
   
    void OnMouseDown()
    {
        //StartCoroutine(pulseController.PulseCoroutine());
        pulseController.UpPulseScream();
        spriteRenderer.enabled = false;
        colliderr2D.enabled = false;
    }
}
