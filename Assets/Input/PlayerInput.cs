using UnityEngine;
using Fusion;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInput : NetworkBehaviour
{
    private Brother1InputActions _brother1InputActions;
    private Brother2InputActions _brother2InputActions;
    private InputAction _move, _jump, _dash, _ladderGrab;
    private NetworkObject _networkObject;

    private int _selectedCharacter = 0;
    private bool _initialized = false;

    private void Awake()
    {
        _networkObject = GetComponent<NetworkObject>();

        _brother1InputActions = new Brother1InputActions();
        _brother2InputActions = new Brother2InputActions();
    }


    public override void Spawned()
    {
        if (_networkObject == null || !_networkObject.HasInputAuthority)
        {
            enabled = false;
            return;
        }

        TryInitialize();
    }

    private void Update()
    {
        if (!_initialized)
        {
            Debug.Log("Trying to initialize PlayerInput...");
            TryInitialize();
        }
    }

    private void TryInitialize()
    {
        var playerData = GameManager.Instance.GetPlayerData(FusionHelper.LocalRunner.LocalPlayer);
        if (playerData == null)
        {
            // PlayerData not ready yet, retry next frame
            return;
        }

        _selectedCharacter = playerData.SelectedCharacter;

        if (_selectedCharacter == 0)
        {
            _move = _brother1InputActions.Player.Move;
            _jump = _brother1InputActions.Player.Jump;
            _dash = _brother1InputActions.Player.Dash;
            _ladderGrab = _brother1InputActions.Player.LadderGrab;
            Debug.Log($"[PlayerInput] Assigned Brother1 input actions for player {FusionHelper.LocalRunner.LocalPlayer.PlayerId}");
        }
        else
        {
            _move = _brother2InputActions.Player.Move;
            _jump = _brother2InputActions.Player.Jump;
            _dash = _brother2InputActions.Player.Dash;
            _ladderGrab = _brother2InputActions.Player.LadderGrab;
            Debug.Log($"[PlayerInput] Assigned Brother2 input actions for player {FusionHelper.LocalRunner.LocalPlayer.PlayerId}");
        }

        _initialized = true;
    }

    private void OnEnable()
    {
        if (_selectedCharacter == 0)
            _brother1InputActions?.Enable();
        else
            _brother2InputActions?.Enable();
    }

    private void OnDisable()
    {
        if (_selectedCharacter == 0)
            _brother1InputActions?.Disable();
        else
            _brother2InputActions?.Disable();
    }

    public FrameInput Gather()
    {
        if (!_initialized)
            return default;

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
