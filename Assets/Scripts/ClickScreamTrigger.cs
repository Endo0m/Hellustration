using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickScreamTrigger : MonoBehaviour
{
   [SerializeField] private PlayerPulseUI pulseUI;
   [SerializeField] private SpriteRenderer spriteRenderer;
   [SerializeField] private Collider2D colliderr2D;
   
    void OnMouseDown()
    {
        StartCoroutine(pulseUI.PulseCoroutine());
        spriteRenderer.enabled = false;
        colliderr2D.enabled = false;
    }
}
