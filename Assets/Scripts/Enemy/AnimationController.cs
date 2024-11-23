using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour, IAnimatable
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayAnimation(string animationTrigger, bool value)
    {
        animator.SetBool(animationTrigger, value);
    }

    public void UpdateFacing(float horizontalVelocity)
    {
        if (horizontalVelocity != 0)
        {
            spriteRenderer.flipX = horizontalVelocity < 0;
        }
    }
}