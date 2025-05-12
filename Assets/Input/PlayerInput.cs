using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInput : MonoBehaviour
{
    private Brother1InputActions _brother1InputActions;
    private Brother2InputActions _brother2InputActions;
    private InputAction _move, _jump, _dash, _ladderGrab;

    private bool _inputSetupDone = false;
    private int _selectedCharacter = 0;

    private void Awake()
    {
        _brother1InputActions = new Brother1InputActions();
        _brother2InputActions = new Brother2InputActions();
    }

    private void Update()
    {
        if (_inputSetupDone) return;

        var playerData = GameManager.Instance.GetPlayerData(FusionHelper.LocalRunner.LocalPlayer);

        if (playerData != null)
        {
            _inputSetupDone = true;
            _selectedCharacter = playerData.SelectedCharacter;

            if (_selectedCharacter == 0)
            {
                _move = _brother1InputActions.Player.Move;
                _jump = _brother1InputActions.Player.Jump;
                _dash = _brother1InputActions.Player.Dash;
                _ladderGrab = _brother1InputActions.Player.LadderGrab;
                _brother1InputActions.Enable();
            }
            else
            {
                _move = _brother2InputActions.Player.Move;
                _jump = _brother2InputActions.Player.Jump;
                _dash = _brother2InputActions.Player.Dash;
                _ladderGrab = _brother2InputActions.Player.LadderGrab;
                _brother2InputActions.Enable();
            }
        }
    }

    private void OnDisable()
    {
        if (!_inputSetupDone) return;

        if (_selectedCharacter == 0)
            _brother1InputActions?.Disable();
        else
            _brother2InputActions?.Disable();
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
}

public struct FrameInput
{
    public Vector2 Move;
    public bool JumpDown;
    public bool JumpHeld;
    public bool DashDown;
    public bool LadderHeld;
}
