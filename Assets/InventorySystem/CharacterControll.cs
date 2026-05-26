using System.Collections.Specialized;
using UnityEngine;
public class CharacterControll : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip walkFootstepClip;
    [SerializeField] private AudioClip runFootstepClip;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.3f;

    private float verticalVelocity;
    private float currentAnimValue;
    private float animVelocity;
    private float footstepTimer;
    private Door curentDoor;
    private int speedHash;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        speedHash = Animator.StringToHash(speedParameter);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Old Input System
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);
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

            moveDirection = cameraForward * vertical + cameraRight * horizontal;
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
        HandleFootsteps(isMoving, isRunning);
    }

    private void HandleFootsteps(bool isMoving, bool isRunning)
    {
        if (!isMoving || !controller.isGrounded)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            AudioClip clip = isRunning ? runFootstepClip : walkFootstepClip;
            float interval = isRunning ? runStepInterval : walkStepInterval;
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
            }
            footstepTimer = interval;
        }
    }
}
