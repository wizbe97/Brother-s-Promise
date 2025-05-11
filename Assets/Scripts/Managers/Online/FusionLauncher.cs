using UnityEngine;
using Fusion;


public class FusionLauncher : MonoBehaviour
{
    private NetworkRunner _runner;
    private ConnectionStatus _status;


    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Failed,
        Connected,
        Loading,
        Loaded
    }

    public async void Launch(GameMode mode, string room,
        INetworkSceneManager sceneLoader)
    {
        Debug.Log($"[FusionLauncher] Launching Fusion - Room: {room}, Mode: {mode}");

        SetConnectionStatus(ConnectionStatus.Connecting, "");

        // Prevent the object from being destroyed on scene load
        DontDestroyOnLoad(gameObject);

        // If there was a previous runner, shutdown and destroy it
        if (_runner != null)
        {
            await _runner.Shutdown();
            Destroy(_runner.gameObject);  // Make sure to destroy the old runner
            _runner = null;  // Clear reference to the old runner
        }

        // Create a new runner instance for each launch
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.name = name;
        _runner.ProvideInput = mode != GameMode.Server;

        // Start the game with the new runner
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = room,
            SceneManager = sceneLoader
        });

        Debug.Log($"[FusionLauncher] Fusion started - Room: {room}, Mode: {mode}");
    }

    public void SetConnectionStatus(ConnectionStatus status, string message)
    {
        _status = status;
    }
}
