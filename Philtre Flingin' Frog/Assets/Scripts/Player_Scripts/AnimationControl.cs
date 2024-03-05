using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    public Animator animator;
    public PlayerMovement playerMovement;

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", playerMovement.getSpeed());
    }
}
