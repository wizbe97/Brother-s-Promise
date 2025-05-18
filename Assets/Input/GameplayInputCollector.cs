using UnityEngine;
using Fusion;

public class GameplayInputCollector : MonoBehaviour
{
    public static FrameInput CachedInput;

    private Brother1InputActions _brother1InputActions;
    private Brother2InputActions _brother2InputActions;

    private void Awake()
    {
        _brother1InputActions = new Brother1InputActions();
        _brother2InputActions = new Brother2InputActions();
    }

    private void OnEnable()
    {
        _brother1InputActions.Enable();
        _brother2InputActions.Enable();
    }

    private void OnDisable()
    {
        _brother1InputActions.Disable();
        _brother2InputActions.Disable();
    }

    private void Update()
    {
        if (GameManager.Instance == null || FusionHelper.LocalRunner == null)
            return;

        int selectedCharacter = GameManager.Instance.GetPlayerData(FusionHelper.LocalRunner.LocalPlayer)?.SelectedCharacter ?? 0;

        FrameInput frameInput = default;

        if (selectedCharacter == 0)
        {
            var player = _brother1InputActions.Player;
            frameInput.Move = player.Move.ReadValue<Vector2>();
            frameInput.JumpDown |= player.Jump.WasPressedThisFrame();
            frameInput.JumpHeld = player.Jump.IsPressed();
            frameInput.DashDown |= player.Dash.WasPressedThisFrame();
            frameInput.LadderHeld = player.LadderGrab.IsPressed();
        }
        else
        {
            var player = _brother2InputActions.Player;
            frameInput.Move = player.Move.ReadValue<Vector2>();
            frameInput.JumpDown |= player.Jump.WasPressedThisFrame();
            frameInput.JumpHeld = player.Jump.IsPressed();
            frameInput.DashDown |= player.Dash.WasPressedThisFrame();
            frameInput.LadderHeld = player.LadderGrab.IsPressed();
        }

        CachedInput.Move = frameInput.Move;
        CachedInput.JumpDown |= frameInput.JumpDown;
        CachedInput.JumpHeld = frameInput.JumpHeld;
        CachedInput.DashDown |= frameInput.DashDown;
        CachedInput.LadderHeld = frameInput.LadderHeld;
    }
}
