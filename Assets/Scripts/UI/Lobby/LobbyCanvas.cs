using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using FusionUtilsEvents;

public class LobbyCanvas : MonoBehaviour
{
    public GameLauncher launcher;

    [Header("Scene Setup")]
    public int sceneIndexToLoad;
    public NetworkPrefabRef PlayerDataNO;
    public NetworkPrefabRef LobbyPlayerControllerNO;

    [Header("UI References")]
    [SerializeField] private GameObject panelLobby;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private Button startButton;
    public Transform imageBrother1;
    public Transform imageBrother2;

    [Header("Fusion Events")]
    public FusionEvent OnPlayerJoinedEvent;
    public FusionEvent OnPlayerLeftEvent;
    public FusionEvent OnShutdownEvent;
    public FusionEvent OnPlayerDataSpawnedEvent;

    private void Start()
    {
        int saveSlot = PlayerPrefs.GetInt("SaveSlot", 0);
        bool isOnline = PlayerPrefs.GetInt("PlayOnline", 0) == 1;
        Debug.Log($"[LobbyCanvas] SaveSlot={saveSlot}, PlayOnline={isOnline}");

        if (isOnline)
        {
            launcher = FindObjectOfType<GameLauncher>();
            var levelManager = FindObjectOfType<LevelManager>();
            launcher.LaunchGame(GameMode.AutoHostOrClient, $"session_{saveSlot}", levelManager);
        }

        if (startButton != null)
            startButton.gameObject.SetActive(false);

        if (panelLobby != null)
            panelLobby.SetActive(false);
    }

    private void OnEnable()
    {
        OnPlayerJoinedEvent.RegisterResponse(HandlePlayerJoined);
        OnPlayerLeftEvent.RegisterResponse(UpdateStartButton);
        OnShutdownEvent.RegisterResponse(HandleShutdown);
        OnPlayerDataSpawnedEvent.RegisterResponse(UpdateStartButton);
    }

    private void OnDisable()
    {
        OnPlayerJoinedEvent.RemoveResponse(HandlePlayerJoined);
        OnPlayerLeftEvent.RemoveResponse(UpdateStartButton);
        OnShutdownEvent.RemoveResponse(HandleShutdown);
        OnPlayerDataSpawnedEvent.RemoveResponse(UpdateStartButton);
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
        if (panelLobby != null)
            panelLobby.SetActive(true);

        if (roomNameText != null && runner.SessionInfo.IsValid)
            roomNameText.text = $"Room: {runner.SessionInfo.Name}";

        UpdateStartButton(player, runner);
    }


    private void HandleShutdown(PlayerRef player, NetworkRunner runner)
    {
        if (panelLobby != null)
            panelLobby.SetActive(false);
    }

    private void UpdateStartButton(PlayerRef _, NetworkRunner runner)
    {
        if (startButton == null || runner == null)
            return;

        bool canStart = runner.IsServer && CanStartGame(runner);
        startButton.gameObject.SetActive(canStart);
    }

    private bool CanStartGame(NetworkRunner runner)
    {
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

    public void StartGameButtonPressed()
    {
        if (!FusionHelper.LocalRunner.IsServer)
            return;

        if (sceneIndexToLoad <= 0)
        {
            Debug.LogError("[LobbyCanvas] Invalid scene index to load.");
            return;
        }

        FusionHelper.LocalRunner.SessionInfo.IsOpen = false;
        FusionHelper.LocalRunner.SessionInfo.IsVisible = false;

        // Get the scene name from build index
        string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndexToLoad);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

        // Now call LoadScene with sceneName (string)
        FusionHelper.LocalRunner.LoadScene(sceneName);
    }


}
