using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowObject : MonoBehaviour, IShadow
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject textObject;

    public void ShadowOn()
    {
        animator.SetBool("Appear", true);
        StartCoroutine(TextAppear());
    }

    public void ShadowOff()
    {
        animator.SetBool("Appear", false);
        StartCoroutine(TextDisappear());
    }

    private IEnumerator TextAppear()
    {
        yield return new WaitForSeconds(1.5f);
        textObject.SetActive(true);
    }

    private IEnumerator TextDisappear()
    {
        yield return new WaitForSeconds(0.5f);
        textObject.SetActive(false);
    }
}
