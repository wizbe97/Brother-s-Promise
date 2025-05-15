using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
#endif

public class OfflinePlayerInput : MonoBehaviour
{
    private IInputActionCollection2 _inputActions;
    private InputAction _move, _jump, _dash, _ladderGrab;
    private InputUser _inputUser;
    private InputDevice _assignedDevice;


    public void Initialize(InputDevice device, int selectedCharacter)
    {
        if (device == null)
        {
            Debug.LogError("[OfflinePlayerInput] No device passed to Initialize!");
            return;
        }

        _assignedDevice = device;

        if (selectedCharacter == 0)
            _inputActions = new Brother1InputActions();
        else
            _inputActions = new Brother2InputActions();

        _inputUser = InputUser.CreateUserWithoutPairedDevices();
        InputUser.PerformPairingWithDevice(_assignedDevice, _inputUser);
        _inputUser.AssociateActionsWithUser(_inputActions);
        _inputActions.Enable();

        if (_inputActions is Brother1InputActions brother1)
        {
            _move = brother1.Player.Move;
            _jump = brother1.Player.Jump;
            _dash = brother1.Player.Dash;
            _ladderGrab = brother1.Player.LadderGrab;
        }
        else if (_inputActions is Brother2InputActions brother2)
        {
            _move = brother2.Player.Move;
            _jump = brother2.Player.Jump;
            _dash = brother2.Player.Dash;
            _ladderGrab = brother2.Player.LadderGrab;
        }
        else
        {
            Debug.LogError("[OfflinePlayerInput] Unknown input actions type!");
            return;
        }
    }
    private void OnDisable() => _inputActions?.Disable();

    public FrameInputOffline GatherFrameInput()
    {
        return new FrameInputOffline
        {
            Move = _move.ReadValue<Vector2>(),
            JumpDown = _jump.WasPressedThisFrame(),
            JumpHeld = _jump.IsPressed(),
            DashDown = _dash.WasPressedThisFrame(),
            LadderHeld = _ladderGrab.IsPressed() 
        };
    }
}

public struct FrameInputOffline
{
    public Vector2 Move;
    public bool JumpDown;
    public bool JumpHeld;
    public bool DashDown;
    public bool LadderHeld;
}
