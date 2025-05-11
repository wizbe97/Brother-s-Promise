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

    public FusionEvent OnPlayerJoinedEvent;
    public FusionEvent OnPlayerLeftEvent;
    public FusionEvent OnRunnerShutDownEvent;
    public FusionEvent OnDisconnectedEvent;


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
        Debug.Log("[FusionHelper] Shutdown detected");
        OnPlayerLeftEvent?.Raise(player, runner);
    }



    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) =>
        OnRunnerShutDownEvent?.Raise(default, runner);

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("[FusionHelper] Disconnected from server detected");

        if (runner != null)
        {
            Debug.Log("[FusionHelper] Shutting down after disconnection...");
            _ = runner.Shutdown();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ExitSession();
        }
    }


    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
