using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadClick : MonoBehaviour
{
    [SerializeField] private Rigidbody2D head;
    [SerializeField] private ClickScreamTrigger clickScreamTrigger;
    [SerializeField] private float strange;
    private bool isClicked = false;

    private void OnMouseDown()
    {
        if (!isClicked)
        {
            clickScreamTrigger.PulseActive();
            head.gravityScale = 6;
            head.AddForce(Vector2.up * strange, ForceMode2D.Impulse);
            isClicked = true;
        }
    }

    
}
