using UnityEngine;
using UnityEngine.InputSystem;
using Assets.UI.Scripts;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Asset")]
    [SerializeField] private InputActionAsset playerControls;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;
    private InputAction shootAction;
    private InputAction reloadAction;
    private InputAction interactAction;
    private InputAction switchWeaponAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool ShootPressed { get; private set; }
    public bool ReloadPressed { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool SwitchWeaponPressed { get; private set; }

    private void Awake()
    {
        if (playerControls == null)
        {
            return;
        }

        var playerMap = playerControls.FindActionMap("Player");
        if (playerMap != null)
        {
            moveAction = playerMap.FindAction("Move");
            lookAction = playerMap.FindAction("Look");
            sprintAction = playerMap.FindAction("Sprint");
            shootAction = playerMap.FindAction("Shoot");
            reloadAction = playerMap.FindAction("Reload");
            interactAction = playerMap.FindAction("Interact");
            switchWeaponAction = playerMap.FindAction("Switch Weapon");
        }
    }

    private void OnEnable()
    {
        playerControls?.Enable();
    }

    private void OnDisable()
    {
        playerControls?.Disable();
    }

    private void Update()
    {
        if (playerControls == null || moveAction == null || lookAction == null || sprintAction == null || 
            shootAction == null || reloadAction == null || interactAction == null || switchWeaponAction == null)
        {
            return;
        }

        MoveInput = moveAction.ReadValue<Vector2>();

        // Apply sensitivity from storage
        float sensitivity = PlayerStorageHandler.GetMouseSensitivity();
        LookInput = lookAction.ReadValue<Vector2>() * sensitivity;

        SprintHeld = sprintAction.IsPressed();
        ShootPressed = shootAction.IsPressed();

        ReloadPressed = reloadAction.WasPressedThisFrame();
        InteractPressed = interactAction.WasPressedThisFrame();
        SwitchWeaponPressed = switchWeaponAction != null && switchWeaponAction.WasPressedThisFrame();
    }
}
