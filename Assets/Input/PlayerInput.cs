using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInput : MonoBehaviour
{
    private Brother1InputActions _actions;
    private InputAction _move, _jump, _dash, _ladderGrab;

    private void Awake()
    {
        _actions = new Brother1InputActions(); // This was setup for singleplayer. There is Brother1InputActions and Brother2InputActions. You can see in the prefabs folder the 2 different prefabs for the 2 brothers. It needs to assign the correct InputActions based on the SelectedCharacter in PlayerData.
        _move = _actions.Player.Move;
        _jump = _actions.Player.Jump;
        _dash = _actions.Player.Dash;
        _ladderGrab = _actions.Player.LadderGrab;
    }

    private void OnEnable() => _actions.Enable();
    private void OnDisable() => _actions.Disable();

    public FrameInput Gather()
    {
        return new FrameInput
        {
            Move = _move.ReadValue<Vector2>(),
            JumpDown = _jump.WasPressedThisFrame(),
            JumpHeld = _jump.IsPressed(),
            DashDown = _dash.WasPressedThisFrame(),
            LadderHeld = _ladderGrab.IsPressed()
        };
    }
}

public struct FrameInput
{
    public Vector2 Move;
    public bool JumpDown;
    public bool JumpHeld;
    public bool DashDown;
    public bool LadderHeld;
}
