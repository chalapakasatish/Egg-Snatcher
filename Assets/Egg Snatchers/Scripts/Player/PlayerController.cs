using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum PlayerState
    {
        Grounded,
        Air
    }
    private PlayerState playerState;
    [Header("Components")]
    [SerializeField] private PlayerDetection playerDetection;

    [Header("Elements")]
    [SerializeField] private MobileJoystick joystick;

    [Header("Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float ySpeed;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private BoxCollider groundDetector;

    private void Start()
    {
        playerState = PlayerState.Grounded;
    }
    void Update()
    {
        MoveHorizontal();
        MoveVertical();
    }

    private void MoveVertical()
    {
        switch (playerState)
        {
            case PlayerState.Grounded:
                MoveVerticalGrounded();
                break;
            case PlayerState.Air:
                MoveVerticalAir();
                break;
        }
    }
    private void MoveVerticalGrounded()
    {
        if(!playerDetection.IsGrounded())
        {
            StartFalling();
            return;
        }
    }

    private void StartFalling()
    {
        playerState = PlayerState.Air;
    }
    private void Land()
    {
        playerState = PlayerState.Grounded;
        ySpeed = 0;
    }
    private void MoveVerticalAir()
    {
        float targetY = transform.position.y + ySpeed * Time.deltaTime;
        Vector3 targetPosition = transform.position.With(y: targetY);

        //we are falling
        if(!playerDetection.CanGoThere(targetPosition))
        {
            Physics.Raycast(groundDetector.transform.position, Vector3.down, out RaycastHit hit, groundMask);
            if(hit.collider != null)
            {
                targetPosition.y = hit.point.y;
            }
            transform.position = targetPosition;    
            Land();
            return;
        }
        transform.position = targetPosition;
        ySpeed += Physics.gravity.y * Time.deltaTime;
        if(playerDetection.IsGrounded())
        {
            Land();
        }
    }


    private void MoveHorizontal()
    {
        Vector2 moveVector = joystick.GetMoveVector();
        moveVector.x *= moveSpeed;
        float targetX = transform.position.x  + moveVector.x * Time.deltaTime;
        Vector2 targetPosition = transform.position.With(x: targetX);

        if(playerDetection.CanGoThere(targetPosition))
            transform.position = targetPosition;
    }
}
