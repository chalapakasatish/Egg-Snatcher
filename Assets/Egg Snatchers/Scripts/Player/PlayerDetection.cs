using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerController))]
public class PlayerDetection : MonoBehaviour
{
    [Header("Components")]
    private PlayerController playerController;
    [Header("Elements")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask trampolineMask;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private CapsuleCollider capsuleCollider;
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }
    private void Update()
    {
        DetectTrampolines();
    }
    /*
    public bool CanGoThere(Vector3 targetPosition)
    {
        Vector3 center = targetPosition + boxCollider.center;
        return Physics.OverlapBox(center,boxCollider.bounds.extents/2,Quaternion.identity,groundMask).Length <= 0;
    }
    */
    public void DetectTrampolines()
    {
        if(!IsGrounded())
        {
            return;
        }
        if(Physics.OverlapBox(boxCollider.transform.position, boxCollider.size / 2, Quaternion.identity, trampolineMask).Length > 0)
        {
            playerController.Jump();
        }
    }
    public bool CanGoThere(Vector3 targetPosition)
    {
        Vector3 center = targetPosition + capsuleCollider.center;
        float halfHeight = (capsuleCollider.height/2) - capsuleCollider.radius;
        Vector3 offset = transform.up * halfHeight;
        Vector3 point0 = center + offset;
        Vector3 point1 = center - offset;
        return Physics.OverlapCapsule(point0,point1,capsuleCollider.radius,groundMask).Length <= 0;
    }
    public bool IsGrounded()
    {
        return Physics.OverlapBox(boxCollider.transform.position,boxCollider.size/2,Quaternion.identity,groundMask).Length > 0;
    }
}
