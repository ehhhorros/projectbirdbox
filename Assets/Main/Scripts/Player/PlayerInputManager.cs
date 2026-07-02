using UnityEngine;
using UnityEngine.InputSystem;
// HUOM LISÄÄ NAMESPACE JOTA EN OSAA

/// <summary>
/// Manages player input using Unity's Input System.
/// Reads input actions from an Input Action Asset and exposes
/// the current input values through public properties.
/// </summary>
public class PlayerInputManager : MonoBehaviour
{

    #region Inspector Variables
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("ActionMap Name Reference")]
    [SerializeField] private string actionMapName = "Player";
    #endregion

    #region Private Variables
    private InputAction movementAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintTriggered { get; private set; }
    private InputActionMap mapReference;
    #endregion

    /// <summary>
    /// Finds the required Action Map and input actions,
    /// then subscribes to their input events.
    /// </summary>
    private void Awake()
    {
        mapReference = playerControls.FindActionMap(actionMapName);

        if (mapReference == null)
        {
            Debug.LogError("Action Map not found!");
            return;
        }

        movementAction = mapReference.FindAction("Move");
        lookAction = mapReference.FindAction("Look");
        jumpAction = mapReference.FindAction("Jump");
        sprintAction = mapReference.FindAction("Sprint");

        SubscribeActionValuesToInputEvents();
    }

    /// <summary>
    /// Subscribes input events to update the public input properties.
    /// </summary>
    private void SubscribeActionValuesToInputEvents()
    {
        // Movement
        movementAction.performed +=
            inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled +=
            inputInfo => MovementInput = Vector2.zero;

        // Camera look
        lookAction.performed +=
            inputInfo => LookInput = inputInfo.ReadValue<Vector2>();
        lookAction.canceled +=
            inputInfo => LookInput = Vector2.zero;

        // Jump
        jumpAction.performed +=
            inputInfo => JumpTriggered = true;
        jumpAction.canceled +=
            inputInfo => JumpTriggered = false;

        // Sprint
        sprintAction.performed +=
            inputInfo => SprintTriggered = true;
        sprintAction.canceled +=
            inputInfo => SprintTriggered = false;
    }

    /// <summary>
    /// Enables the player Action Map when this component is enabled.
    /// </summary>
    private void OnEnable()
    {
        mapReference?.Enable();
    }

    /// <summary>
    /// Disables the player Action Map when this component is disabled.
    /// </summary>
    private void OnDisable()
    {
        mapReference?.Disable();
    }
}
