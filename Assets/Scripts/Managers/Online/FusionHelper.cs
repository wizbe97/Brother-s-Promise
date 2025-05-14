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
    public FusionEvent OnPlayerJoinedEvent;
    public FusionEvent OnPlayerLeftEvent;
    public FusionEvent OnRunnerShutDownEvent;
    public FusionEvent OnDisconnectedEvent;
    public FusionEvent OnSceneLoadedEvent;
    public FusionEvent OnInputCollected;



    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        OnPlayerJoinedEvent?.Raise(player, runner);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        OnPlayerLeftEvent?.Raise(player, runner);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) =>
        OnRunnerShutDownEvent?.Raise(default, runner);

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) // Host Disconnected
    {
        OnDisconnectedEvent?.Raise(default, runner);
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        OnSceneLoadedEvent?.Raise(default, runner);
    }


    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(GameplayInputCollector.CachedInput);
        GameplayInputCollector.CachedInput = default;
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
