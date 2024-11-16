using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadClick : MonoBehaviour
{
    [SerializeField] private Rigidbody2D head;
    [SerializeField] private ClickScreamTrigger clickScreamTrigger;

    private bool isClicked = false;

    private void OnMouseDown()
    {
        if (!isClicked)
        {
            clickScreamTrigger.PulseActive();
            head.gravityScale = 6;
            head.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
            isClicked = true;
        }
    }
}
