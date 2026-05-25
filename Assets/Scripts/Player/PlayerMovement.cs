using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float baseWalkSpeed = 2f;
    [SerializeField] private float baseSprintSpeed = 6f;
    [SerializeField] private float gravity = -9.81f;

    private PlayerPerks perks;
    private PlayerInputHandler inputHandler;
    private CharacterController characterController;
    private Rigidbody rigidbodyComponent;
    private Vector3 pendingMove;
    private float verticalVelocity;

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
    }

    private void Update()
    {
        var moveInput = inputHandler != null ? inputHandler.MoveInput : Vector2.zero;
        var move = new Vector3(moveInput.x, 0f, moveInput.y);
        var velocity = move * CurrentSpeed;

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
