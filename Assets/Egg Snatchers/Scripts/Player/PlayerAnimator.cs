using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : NetworkBehaviour
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
    private new void OnDestroy()
    {
        playerController.onJumpStarted -= Jump;
        playerController.onFallStarted -= Fall;
        playerController.onLandStarted -= Land;
    }
    private void Update()
    {
        UpdateBlendTreeRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateBlendTreeRpc()
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
