using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
public class PlayerInputHandler : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 1f;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool ShootPressed { get; private set; }
    public bool ReloadPressed { get; private set; }
    public bool InteractPressed { get; private set; }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        if (keyboard != null)
        {
            var x = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
            var y = (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f);
            MoveInput = new Vector2(x, y);
            SprintHeld = keyboard.leftShiftKey.isPressed;
            ReloadPressed = keyboard.rKey.wasPressedThisFrame;
            InteractPressed = keyboard.eKey.wasPressedThisFrame;
        }
        else
        {
            MoveInput = Vector2.zero;
            SprintHeld = false;
            ReloadPressed = false;
            InteractPressed = false;
        }

        if (mouse != null)
        {
            LookInput = mouse.delta.ReadValue() * mouseSensitivity;
            ShootPressed = mouse.leftButton.isPressed;
        }
        else
        {
            LookInput = Vector2.zero;
            ShootPressed = false;
        }
#else
        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        LookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
        SprintHeld = Input.GetKey(KeyCode.LeftShift);
        ShootPressed = Input.GetMouseButton(0);
        ReloadPressed = Input.GetKeyDown(KeyCode.R);
        InteractPressed = Input.GetKeyDown(KeyCode.E);
#endif
    }
}
