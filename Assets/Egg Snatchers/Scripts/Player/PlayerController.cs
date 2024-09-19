using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
public class PlayerController : NetworkBehaviour
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
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float ySpeed;
    [SerializeField] private float xSpeed;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private BoxCollider groundDetector;

    public float XSpeed { get => xSpeed; set => xSpeed = value; }

    [Header("Actions")]
    public Action onJumpStarted;
    public Action onFallStarted;
    public Action onLandStarted;

    private void Start()
    {
        playerState = PlayerState.Grounded;
    }
    void Update()
    {
        if(!IsOwner) 
            return;
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

    private void MoveVerticalAir()
    {
        float targetY = transform.position.y + ySpeed * Time.deltaTime;
        Vector3 targetPosition = transform.position.With(y: targetY);

        //we are falling
        if(!playerDetection.CanGoThere(targetPosition,out Collider firstCollider))
        {
            float minY = firstCollider.ClosestPoint(transform.position).y;
            Physics.Raycast(groundDetector.transform.position, Vector3.down, out RaycastHit hit,1, groundMask);
            if(hit.collider != null)
            {
                targetPosition.y = minY;
            }
            else
            {
                float maxY = firstCollider.ClosestPoint(transform.position).y;


                //Physics.Raycast(transform.position, Vector3.up, out hit, 3f, groundMask);
                //float maxY = hit.point.y;
                targetPosition.y = maxY - 2.4f;
                ySpeed = 0;
            }
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }
        transform.position = targetPosition;
        if(playerDetection.IsGrounded())
        {
            Land();
        }
    }


    private void MoveHorizontal()
    {
        Vector2 moveVector = InputManager.instance.GetMovevector();

        XSpeed = Mathf.Abs(moveVector.x);

        ManageFacing(moveVector.x);

        moveVector.x *= moveSpeed;

        float targetX = transform.position.x  + moveVector.x * Time.deltaTime;
        Vector2 targetPosition = transform.position.With(x: targetX);

        if(playerDetection.CanGoThere(targetPosition, out Collider firstCollider))
            transform.position = targetPosition;
    }
    public void ManageFacing(float xSpeed)
    {
        float facing = XSpeed != 0 ? Mathf.Sign(xSpeed) : transform.localScale.x;
        transform.localScale = transform.localScale.With(x : facing);
    }
    private void StartFalling()
    {
        playerState = PlayerState.Air;
        onFallStarted?.Invoke();
    }
    private void Land()
    {
        playerState = PlayerState.Grounded;
        ySpeed = 0;
        onLandStarted?.Invoke();
    }
    public void Jump()
    {
        playerState = PlayerState.Air;
        ySpeed = jumpSpeed;
        onJumpStarted?.Invoke();
    }
}
