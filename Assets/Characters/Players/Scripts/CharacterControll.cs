using UnityEngine;
using UnityEngine.InputSystem;
public class CharacterControll : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform cameraTransform;

    [Header("Player Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float gravity = -20f;

    [Header("Animation")]
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private float walkAnimationValue = 0.3f;
    [SerializeField] private float runAnimationValue = 1f;
    [SerializeField] private float animationSmoothTime = 0.1f;

    // Player input action
    private InputAction moveAction;
    private InputAction runAction;

    private float verticalVelocity;
    private float currentAnimValue;
    private float animVelocity;
    // Hash of animations  
    private int speedHash;

    private void Awake()
    {
        // Assign Core Component
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        speedHash = Animator.StringToHash(speedParameter);

        moveAction = playerInput.actions["Move"];
        runAction = playerInput.actions["Run"];
    }

    private void Update()
    {
        HandleMovement();
    }
    // Handle movement Logic
    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        bool isRunning = runAction != null && runAction.IsPressed();

        Vector3 inputDirection = new Vector3(input.x, 0f, input.y);
        bool isMoving = inputDirection.sqrMagnitude > 0.01f;

        Vector3 moveDirection = Vector3.zero;

        if (isMoving)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            moveDirection = cameraForward * input.y + cameraRight * input.x;
            moveDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        float currentSpeed = isMoving ? (isRunning ? runSpeed : walkSpeed) : 0f;
        float targetAnim = isMoving ? (isRunning ? runAnimationValue : walkAnimationValue) : 0f;

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
