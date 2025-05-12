using UnityEngine;
using Fusion;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInput : NetworkBehaviour
{
    private Brother1InputActions _brother1Actions;
    private Brother2InputActions _brother2Actions;

    private InputAction _move, _jump, _dash, _ladderGrab;

    public override void Spawned()
    {
        Debug.Log($"[PlayerInput] Spawned {gameObject.name} with InputAuthority={Object.InputAuthority}");

        if (!Object.HasInputAuthority)
        {
            Debug.Log($"[PlayerInput] No input authority on {gameObject.name}. Skipping input assignment.");
            return;
        }

        StartCoroutine(AssignInputWhenReady());
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

    private System.Collections.IEnumerator AssignInputWhenReady()
    {
        PlayerData playerData = null;

        while (playerData == null)
        {
            playerData = GameManager.Instance?.GetPlayerData(Object.InputAuthority);
            if (playerData == null)
            {
                yield return null; // Wait next frame
            }
        }

        Debug.Log($"[PlayerInput] Found PlayerData for {gameObject.name}. SelectedCharacter={playerData.SelectedCharacter}");

        AssignInputActions(playerData.SelectedCharacter);
    }

    private void AssignInputActions(int selectedCharacter)
    {
        if (selectedCharacter == 0)
        {
            _brother1Actions = new Brother1InputActions();
            _move = _brother1Actions.Player.Move;
            _jump = _brother1Actions.Player.Jump;
            _dash = _brother1Actions.Player.Dash;
            _ladderGrab = _brother1Actions.Player.LadderGrab;

            Debug.Log($"[PlayerInput] Assigned Brother1 input actions for {gameObject.name}");
        }
        else if (selectedCharacter == 1)
        {
            _brother2Actions = new Brother2InputActions();
            _move = _brother2Actions.Player.Move;
            _jump = _brother2Actions.Player.Jump;
            _dash = _brother2Actions.Player.Dash;
            _ladderGrab = _brother2Actions.Player.LadderGrab;

            Debug.Log($"[PlayerInput] Assigned Brother2 input actions for {gameObject.name}");
        }
        else
        {
            Debug.LogError($"[PlayerInput] Unknown SelectedCharacter={selectedCharacter} on {gameObject.name}");
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
