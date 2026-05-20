using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float baseWalkSpeed = 2f;
    [SerializeField] private float baseSprintSpeed = 6f;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    private PlayerPerks perks;

    // Movement speed reads perk modifiers directly for sprint bonuses.
    public float CurrentSpeed => IsSprinting
        ? baseSprintSpeed * (perks != null ? perks.SprintSpeedMultiplier : 1f)
        : baseWalkSpeed;

    private bool IsSprinting => Input.GetKey(sprintKey);

    private void Awake()
    {
        perks = GetComponent<PlayerPerks>();
    }

    private void Update()
    {
        var moveX = Input.GetAxis("Horizontal");
        var moveZ = Input.GetAxis("Vertical");
        var move = new Vector3(moveX, 0f, moveZ);
        transform.Translate(move * (CurrentSpeed * Time.deltaTime));
    }
}
