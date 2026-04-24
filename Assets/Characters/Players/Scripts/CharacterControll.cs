using UnityEngine;
using UnityEngine.InputSystem;
public class CharacterControll : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInput playerInput;
    // Rerence to mainCamera
    private Transform mainCamera;
    [Header("Player Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Animation")]
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private float walkAnimationValue = 0.3f;
    [SerializeField] private float runAnimationValue = 1f;
    [SerializeField] private float animationSmoothTime = 0.1f;

    private InputAction moveAction;
    private InputAction runAction;

    private float verticalVelocity;
    private float currentAnimValue;
    private float animVelocity;
    private int speedHash;

    private void Awake()
    {
        // Assign Key Component (Character control,   Player input, Animator  )
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        // Hash of Animation parameters 
        speedHash = Animator.StringToHash(speedParameter);
        // Set the player input
        moveAction = playerInput.actions["Move"];
        runAction = playerInput.actions["Run"];
    }

    private void Update()
    {
        HandleMovement();
    }
    // Handle movement logic 
    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        // Checke if the player running 
        bool isRunning = runAction.IsPressed();

        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);
        bool isMoving = moveDirection.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            moveDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        float currentSpeed = 0f;
        float targetAnim = 0f;

        if (isMoving)
        {
            currentSpeed = isRunning ? runSpeed : walkSpeed;
            targetAnim = isRunning ? runAnimationValue : walkAnimationValue;
        }

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalMove = moveDirection * currentSpeed;
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);

        currentAnimValue = Mathf.SmoothDamp(
            currentAnimValue,
            targetAnim,
            ref animVelocity,
            animationSmoothTime
        );

        animator.SetFloat(speedHash, currentAnimValue);
    }
}
