using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using FusionUtilsEvents;

public class FusionHelper : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner LocalRunner;
    public NetworkPrefabRef PlayerDataNO;
    public NetworkPrefabRef LobbyPlayerControllerNO;
    private Brother1InputActions _brother1InputActions;
    private Brother2InputActions _brother2InputActions;

    public FusionEvent OnPlayerJoinedEvent;
    public FusionEvent OnPlayerLeftEvent;
    public FusionEvent OnRunnerShutDownEvent;
    public FusionEvent OnDisconnectedEvent;
    public FusionEvent OnSceneLoadedEvent;

    private FrameInput _cachedFrameInput;

    private void Update()
    {
        if (GameManager.Instance == null || LocalRunner == null || SceneManager.GetActiveScene().name != "4_Gameplay1")
            return;

        int selectedCharacter = GameManager.Instance.GetPlayerData(LocalRunner.LocalPlayer)?.SelectedCharacter ?? 0;

        if (selectedCharacter == 0)
        {
            var player = _brother1InputActions.Player;

            _cachedFrameInput.Move = player.Move.ReadValue<Vector2>();

            if (player.Jump.WasPressedThisFrame())
                _cachedFrameInput.JumpDown = true;

            _cachedFrameInput.JumpHeld = player.Jump.IsPressed();

            if (player.Dash.WasPressedThisFrame())
                _cachedFrameInput.DashDown = true;

            _cachedFrameInput.LadderHeld = player.LadderGrab.IsPressed();
        }
        else
        {
            var player = _brother2InputActions.Player;

            _cachedFrameInput.Move = player.Move.ReadValue<Vector2>();

            if (player.Jump.WasPressedThisFrame())
                _cachedFrameInput.JumpDown = true;

            _cachedFrameInput.JumpHeld = player.Jump.IsPressed();

            if (player.Dash.WasPressedThisFrame())
                _cachedFrameInput.DashDown = true;

            _cachedFrameInput.LadderHeld = player.LadderGrab.IsPressed();
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Debug.Log($"[FusionHelper] Spawning PlayerData and LobbyPlayerController for player {player}");

            // Spawn networked PlayerData
            runner.Spawn(PlayerDataNO, inputAuthority: player);

            // Spawn networked LobbyPlayerController
            runner.Spawn(LobbyPlayerControllerNO, inputAuthority: player);
        }

        if (runner.LocalPlayer == player)
        {
            LocalRunner = runner;
            Debug.Log("[FusionHelper] LocalRunner assigned");
        }

        OnPlayerJoinedEvent?.Raise(player, runner);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        OnPlayerLeftEvent?.Raise(player, runner);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) =>
        OnRunnerShutDownEvent?.Raise(default, runner);

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("Host Disconnected");
        OnDisconnectedEvent?.Raise(default, runner);
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("[FusionHelper] Scene Load Done");
        OnSceneLoadedEvent?.Raise(default, runner);

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "4_Gameplay1")
        {
            _brother1InputActions = new Brother1InputActions();
            _brother1InputActions.Enable();

            _brother2InputActions = new Brother2InputActions();
            _brother2InputActions.Enable();

            Debug.Log("[FusionHelper] Gameplay input actions enabled");
        }
    }


    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(_cachedFrameInput);
        _cachedFrameInput = default; // reset after sending
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
