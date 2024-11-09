using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowController : MonoBehaviour
{
    private ShadowObject shadowObject;

    private void Awake() 
    {
        shadowObject = GetComponent<ShadowObject>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        shadowObject.ShadowOn();
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        shadowObject.ShadowOff();
    }
}
