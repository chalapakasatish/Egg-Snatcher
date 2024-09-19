using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("Components")]
    private PlayerController playerController;
    [Header("Elements")]
    [SerializeField] private Animator animator;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerController.onJumpStarted += Jump;
        playerController.onFallStarted += Fall;
        playerController.onLandStarted += Land;
    }
    private void OnDestroy()
    {
        playerController.onJumpStarted -= Jump;
        playerController.onFallStarted -= Fall;
        playerController.onLandStarted -= Land;
    }
    private void Update()
    {
        UpdateBlendTree();
    }

    private void UpdateBlendTree()
    {
        animator.SetFloat("xSpeed", playerController.XSpeed);
    }
    public void Jump()
    {
        animator.Play("Jump");
    } 
    public void Fall()
    {
        animator.Play("Fall");
    } 
    public void Land()
    {
        animator.Play("Land");
    }
}
