using UnityEngine;
// HUOM LISÄÄ NAMESPACE JOTA EN OSAA

/// <summary>
/// Controls a first-person player using a CharacterController.
/// Handles movement, sprinting, jumping, and mouse look.
/// </summary>
public class FirstPersonController : MonoBehaviour
{
    #region Inspector Variables

    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityMultiplier = 1f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputManager playerInputManager;
    
    #endregion

    #region Private Variables
    private Vector3 currentMovement;
    private float verticalRotation;
    private float currentSpeed => 
        walkSpeed * (playerInputManager.SprintTriggered ? sprintMultiplier : 1f);
   
    #endregion

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    /// <summary>
    /// Converts movement input from local space to world space.
    /// This allows movement relative to where the player is facing.
    /// </summary>
    /// <returns>Normalized world-space movement direction.</returns>
    private Vector3 CalculateWorldDirection()
    {
        Vector3 inputDirection = new(playerInputManager.MovementInput.x, 0f, playerInputManager.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }

    /// <summary>
    /// Handles jumping and gravity.
    /// Keeps the player grounded and applies gravity while airborne.
    /// </summary>
    private void HandleJump()
    {
        if(characterController.isGrounded)
        {
            currentMovement.y = -0.5f;
            if(playerInputManager.JumpTriggered)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    /// <summary>
    /// Calculates movement direction and moves the player.
    /// </summary>    
    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * currentSpeed;
        currentMovement.z = worldDirection.z * currentSpeed;

        HandleJump();
        characterController.Move(currentMovement * Time.deltaTime);
    }

    /// <summary>
    /// Rotates the player horizontally.
    /// </summary>
    /// <param name="rotationAmount">
    /// Horizontal rotation amount in degrees.
    /// </param>
    private void AddHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    /// <summary>
    /// Rotates the camera vertically while clamping the angle.
    /// </summary>
    /// <param name="rotationAmount">
    /// Vertical rotation amount in degrees.
    /// </param>
    private void AddVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    /// <summary>
    /// Handles mouse look.
    /// </summary>
    private void HandleRotation()
    {
        float mouseXRotation = playerInputManager.LookInput.x * mouseSensitivity;
        float mouseYRotation = playerInputManager.LookInput.y * mouseSensitivity;

        AddHorizontalRotation(mouseXRotation);
        AddVerticalRotation(mouseYRotation);

    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }
}
