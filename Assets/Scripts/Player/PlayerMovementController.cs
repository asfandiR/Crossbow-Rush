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
    [SerializeField] private float acceleration = 10.0f;
    [SerializeField] private Joystick joystick; 
    
    private CharacterController characterController;
    private PlayerAttackController playerAttackController;
    private Camera mainCamera;
    private Vector3 moveDirection;
    private float currentSpeed;

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
        Vector3 input = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
        float inputMagnitude = Mathf.Abs (Mathf.Clamp01(input.magnitude));
        
        if (input.sqrMagnitude >= 0.01f)
        {
            moveDirection = input.normalized; // Нормализуем, чтобы убрать рывки от дрожания стика
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed * inputMagnitude, acceleration * Time.deltaTime); // Lerp для плавности
            
            if(playerAttackController.hasTarget==false)
                RotateTowards(moveDirection);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, acceleration * Time.deltaTime); // Плавное торможение
        }
        
        if (currentSpeed > 0.01f)
            MovePlayer(moveDirection); // Двигаемся по последнему известному направлению
        else
            currentSpeed = 0f;
            
    }
 private void RotateTowards(Vector3 direction)
    {
       if (direction != Vector3.zero)
           transform.rotation = Quaternion.LookRotation(direction);
       }
    private void MovePlayer(Vector3 targetDirection)
    {
         Vector3 desiredMove = targetDirection * currentSpeed * Time.deltaTime;
                characterController.Move(desiredMove);
            
    }
    
    public bool HasActiveInput()
    {
        return new Vector3(joystick.Horizontal, 0f, joystick.Vertical).sqrMagnitude >= 0.01f;
    }
}