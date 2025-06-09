using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [Space]
    [SerializeField] private CameraSpring cameraSpring;
    [SerializeField] private CameraLean cameraLean;

    private PlayerInputActions _inputActions;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());

        cameraSpring.Initialize();
        cameraLean.Initialize();
    }

    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    void Update()
    {
        var input = _inputActions.Gameplay;
        var deltaTime = Time.deltaTime;

        var cameraInput = new CameraInput
        {
            Look = input.Look.ReadValue<Vector2>()
        };
        playerCamera.UpdateRotation(cameraInput);

        var characterInput = new CharacterInput
        {
            Rotation = playerCamera.transform.rotation,
            Move = input.Move.ReadValue<Vector2>(),
            Jump = input.Jump.WasPressedThisFrame(),
            JumpSustain = input.Jump.IsPressed(),
            Crouch = input.Crouch.WasPressedThisFrame()
                ? CrouchInput.Toggle
                : CrouchInput.None
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);

        // --- DASH ON SHIFT ---
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            playerCharacter.TryDash();
        }

        // --- GRAPPLE ON RIGHT MOUSE BUTTON ---
        if (Input.GetMouseButtonDown(1))
        {
            // Raycast from camera center
            var cam = Camera.main;
            var origin = cam.transform.position;
            var direction = cam.transform.forward;
            playerCharacter.TryStartGrapple(origin, direction);
        }

        // --- CANCEL GRAPPLE ON RIGHT MOUSE BUTTON UP ---
        if (Input.GetMouseButtonUp(1))
        {
            playerCharacter.CancelGrapple();
        }
    }

    void LateUpdate()
    {
        var deltaTime = Time.deltaTime;
        var cameraTarget = playerCharacter.GetCameraTarget();
        var state = playerCharacter.GetState();

        playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
        cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
        cameraLean.UpdateLean(deltaTime, state.Stance is Stance.Slide, state.Acceleration, cameraTarget.up);
        playerCamera.UpdateFov(playerCharacter.GetTargetFov());
    }
}
