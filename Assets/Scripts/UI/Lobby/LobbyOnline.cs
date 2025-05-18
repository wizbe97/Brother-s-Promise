using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using FusionUtilsEvents;

public class LobbyOnline : LobbyBase
{
    public GameLauncher launcher;

    [Header("Network Prefabs")]
    public NetworkPrefabRef PlayerDataNO;
    public NetworkPrefabRef LobbyPlayerControllerNO;

    [Header("Fusion Events")]
    public FusionEvent OnPlayerJoinedEvent;
    public FusionEvent OnPlayerLeftEvent;
    public FusionEvent OnShutdownEvent;
    public FusionEvent OnPlayerDataSpawnedEvent;

    protected override void Start()
    {
        base.Start();

        launcher = FindObjectOfType<GameLauncher>();
        var levelManager = FindObjectOfType<LevelManager>();

        launcher.LaunchGame(GameMode.AutoHostOrClient, $"session_{saveSlot}", levelManager);
        GameManager.Instance.SetIsOnline(true);
    }

    private void OnEnable()
    {
        OnPlayerJoinedEvent.RegisterResponse(HandlePlayerJoined);
        OnPlayerLeftEvent.RegisterResponse(UpdateStartButtonOnline);
        OnShutdownEvent.RegisterResponse(HandleShutdown);
        OnPlayerDataSpawnedEvent.RegisterResponse(UpdateStartButtonOnline);
    }

    private void OnDisable()
    {
        OnPlayerJoinedEvent.RemoveResponse(HandlePlayerJoined);
        OnPlayerLeftEvent.RemoveResponse(UpdateStartButtonOnline);
        OnShutdownEvent.RemoveResponse(HandleShutdown);
        OnPlayerDataSpawnedEvent.RemoveResponse(UpdateStartButtonOnline);
    }

    private void HandlePlayerJoined(PlayerRef player, NetworkRunner runner)
    {
        if (runner.IsServer)
        {
            runner.Spawn(PlayerDataNO, inputAuthority: player);
            runner.Spawn(LobbyPlayerControllerNO, inputAuthority: player);
        }

        if (runner.LocalPlayer == player)
            FusionHelper.LocalRunner = runner;

        ShowLobbyPanel(runner.SessionInfo.IsValid ? runner.SessionInfo.Name : "Lobby");

        UpdateStartButtonOnline(player, runner);
    }

    private void HandleShutdown(PlayerRef player, NetworkRunner runner)
    {
        if (panelLobby != null)
            panelLobby.SetActive(false);
    }

    private void UpdateStartButtonOnline(PlayerRef _, NetworkRunner runner)
    {
        if (runner == null)
        {
            UpdateStartButton(false);
            return;
        }

        bool canStart = runner.IsServer && CanStartGameInternal();
        UpdateStartButton(canStart);
    }

    protected override bool CanStartGameInternal()
    {
        var runner = FusionHelper.LocalRunner;
        if (runner == null)
            return false;

        int playerCount = 0;
        foreach (var _ in runner.ActivePlayers)
            playerCount++;

        if (playerCount != 2) return false;

        var selected = new HashSet<int>();
        foreach (var player in runner.ActivePlayers)
        {
            var data = GameManager.Instance.GetPlayerData(player);
            if (data == null || !data.IsReady)
                return false;

            selected.Add(data.SelectedCharacter);
        }

        return selected.Count == 2;
    }

    public override void StartGameButtonPressed()
    {
        if (!FusionHelper.LocalRunner.IsServer)
            return;

        FusionHelper.LocalRunner.SessionInfo.IsOpen = false;
        FusionHelper.LocalRunner.SessionInfo.IsVisible = false;

        base.StartGameButtonPressed();

        string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndexToLoad);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        FusionHelper.LocalRunner.LoadScene(sceneName);
    }
}
