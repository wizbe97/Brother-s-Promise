using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInput : MonoBehaviour
{
    private Brother1InputActions _brother1Actions;
    private Brother2InputActions _brother2Actions;

    private InputAction _move, _jump, _dash, _ladderGrab;

    private void Awake()
    {
        AssignInputActions();
        Debug.Log($"[PlayerInput] Assigned {(_brother1Actions != null ? "Brother1" : "Brother2")} input for {gameObject.name}");
    }

    private void OnEnable()
    {
        _brother1Actions?.Enable();
        _brother2Actions?.Enable();
    }

    private void OnDisable()
    {
        _brother1Actions?.Disable();
        _brother2Actions?.Disable();
    }

    public FrameInput Gather()
    {
        return new FrameInput
        {
            Move = _move?.ReadValue<Vector2>() ?? Vector2.zero,
            JumpDown = _jump?.WasPressedThisFrame() ?? false,
            JumpHeld = _jump?.IsPressed() ?? false,
            DashDown = _dash?.WasPressedThisFrame() ?? false,
            LadderHeld = _ladderGrab?.IsPressed() ?? false
        };
    }

    private void AssignInputActions()
    {
        var playerData = GetComponentInParent<PlayerData>();
        if (playerData == null || !playerData.Object.HasInputAuthority)
        {
            Debug.LogWarning("[PlayerInput] No input authority or PlayerData not found.");
            return;
        }

        if (playerData.SelectedCharacter == 0)
        {
            _brother1Actions = new Brother1InputActions();
            _move = _brother1Actions.Player.Move;
            _jump = _brother1Actions.Player.Jump;
            _dash = _brother1Actions.Player.Dash;
            _ladderGrab = _brother1Actions.Player.LadderGrab;
        }
        else if (playerData.SelectedCharacter == 1)
        {
            _brother2Actions = new Brother2InputActions();
            _move = _brother2Actions.Player.Move;
            _jump = _brother2Actions.Player.Jump;
            _dash = _brother2Actions.Player.Dash;
            _ladderGrab = _brother2Actions.Player.LadderGrab;
        }
        else
        {
            Debug.LogError($"[PlayerInput] Unknown character index {playerData.SelectedCharacter}");
        }
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
