using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float baseWalkSpeed = 2f;
    [SerializeField] private float baseSprintSpeed = 6f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform moveReference;

    private PlayerPerks perks;
    private PlayerInputHandler inputHandler;
    private CharacterController characterController;
    private Rigidbody rigidbodyComponent;
    private Vector3 pendingMove;
    private float verticalVelocity;
    private Animator animator;

    // Movement speed reads perk modifiers directly for sprint bonuses.
    public float CurrentSpeed => IsSprinting
        ? baseSprintSpeed * (perks != null ? perks.SprintSpeedMultiplier : 1f)
        : baseWalkSpeed;

    private bool IsSprinting => inputHandler != null && inputHandler.SprintHeld;

    private void Awake()
    {
        perks = GetComponent<PlayerPerks>();
        inputHandler = GetComponent<PlayerInputHandler>();
        characterController = GetComponent<CharacterController>();
        rigidbodyComponent = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        if (moveReference == null)
        {
            moveReference = transform;
        }
    }

    private void Update()
    {
        var moveInput = inputHandler != null ? inputHandler.MoveInput : Vector2.zero;
        var forward = moveReference.forward;
        forward.y = 0f;
        forward.Normalize();
        var right = moveReference.right;
        right.y = 0f;
        right.Normalize();
        var move = forward * moveInput.y + right * moveInput.x;
        var velocity = move * CurrentSpeed;

        if (animator != null)
        {
            float targetSpeed = move.magnitude;
            if (IsSprinting && targetSpeed > 0) targetSpeed = 1f;
            else if (targetSpeed > 0) targetSpeed = 0.5f;
            animator.SetFloat("Speed", targetSpeed, 0.1f, Time.deltaTime);
        }

        if (characterController != null)
        {
            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            verticalVelocity += gravity * Time.deltaTime;
            var motion = (velocity + Vector3.up * verticalVelocity) * Time.deltaTime;
            characterController.Move(motion);
            return;
        }

        if (rigidbodyComponent != null && !rigidbodyComponent.isKinematic)
        {
            pendingMove = velocity;
            return;
        }

        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    private void FixedUpdate()
    {
        if (rigidbodyComponent == null || rigidbodyComponent.isKinematic)
        {
            return;
        }

        var delta = pendingMove * Time.fixedDeltaTime;
        if (delta.sqrMagnitude <= 0f)
        {
            return;
        }

        rigidbodyComponent.MovePosition(rigidbodyComponent.position + delta);
    }
}
