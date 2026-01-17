// PlayerMovementController.cs
using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovementController : MonoBehaviour
{
    // ... (Поля и Awake остаются без изменений)
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;    
    [SerializeField] private Joystick joystick; 
    
    private CharacterController characterController;
    private PlayerAttackController playerAttackController;
    private Camera mainCamera;
    private Vector3 moveDirection;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerAttackController = GetComponent<PlayerAttackController>();
        mainCamera = Camera.main; 
         if (joystick == null || mainCamera == null)
        {
            Debug.LogError("Ты пидорасс");
            return;
        }
    }

    private void Update()
    {
        moveDirection =  new Vector3(joystick.Horizontal, 0f, joystick.Vertical) * -1f;
        if (HasActiveInput())
        {
        MovePlayer(moveDirection);
        if(playerAttackController.hasTarget)
        RotateTowards(moveDirection);
        }
        ApplyGravity();

    }
 private void RotateTowards(Vector3 target)
    {
       transform.LookAt(target);
       }
    private void MovePlayer(Vector3 targetDirection)
    {
         Vector3 desiredMove = targetDirection * moveSpeed * Time.deltaTime*-1f;
                characterController.Move(desiredMove);
            
    }
    
   
    private void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            characterController.Move(Vector3.down * 9.8f * Time.deltaTime); 
        }
    }
    
    public bool HasActiveInput()
    {
        return moveDirection.magnitude >= 0.01f;
    }
}