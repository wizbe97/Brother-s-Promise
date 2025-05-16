using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyOffline : LobbyBase
{
    [Header("Offline Player Setup")]
    public GameObject lobbyPlayerPrefab;

    private HashSet<InputDevice> joinedDevices = new HashSet<InputDevice>();
    private Dictionary<int, LobbyPlayerControllerOffline> characterLocks = new();
    private bool waitingForExitConfirmation = false;

    protected override void Start()
    {
        base.Start();
        isOnline = false;
        GameManager.Instance.SetIsOnline(false);
        ShowLobbyPanel("Offline Co-Op");
    }

    private void Update()
    {
        if (joinedDevices.Count >= 2) return;

        foreach (var device in InputSystem.devices)
        {
            if (joinedDevices.Contains(device)) continue;

            if (DeviceJoinPressed(device))
                SpawnLocalPlayer(device);
        }

        if (joinedDevices.Count == 0 && Input.GetKeyDown(KeyCode.Escape) && waitingForExitConfirmation)
            GameManager.Instance?.ExitSession();
        else
            waitingForExitConfirmation = true;
    }

    private bool DeviceJoinPressed(InputDevice device)
    {
        if (device is Keyboard keyboard)
            return keyboard.spaceKey.wasPressedThisFrame;

        if (device is Gamepad gamepad)
            return gamepad.buttonSouth.wasPressedThisFrame;

        return false;
    }

    private void SpawnLocalPlayer(InputDevice device)
    {
        GameObject newPlayer = Instantiate(lobbyPlayerPrefab, imageBrother1);
        var lobbyPlayer = newPlayer.GetComponent<LobbyPlayerControllerOffline>();

        if (lobbyPlayer != null)
        {
            lobbyPlayer.assignedDevice = device;
            lobbyPlayer.imageBrother1 = imageBrother1;
            lobbyPlayer.imageBrother2 = imageBrother2;
            lobbyPlayer.lobbyManager = this;
            lobbyPlayer.Initialize(joinedDevices.Count);
        }

        joinedDevices.Add(device);
    }

    public void RemovePlayer(LobbyPlayerControllerOffline player)
    {
        if (player == null) return;

        if (joinedDevices.Contains(player.assignedDevice))
            joinedDevices.Remove(player.assignedDevice);

        foreach (var key in new List<int>(characterLocks.Keys))
        {
            if (characterLocks[key] == player)
                characterLocks.Remove(key);
        }

        Destroy(player.gameObject);
        CheckIfAllReady();
    }

    public void RegisterReady(LobbyPlayerControllerOffline player, int selectedCharacter)
    {
        if (!characterLocks.ContainsKey(selectedCharacter))
            characterLocks[selectedCharacter] = player;

        CheckIfAllReady();
    }

    public void UnregisterReady(LobbyPlayerControllerOffline player, int selectedCharacter)
    {
        if (characterLocks.TryGetValue(selectedCharacter, out var p) && p == player)
            characterLocks.Remove(selectedCharacter);

        CheckIfAllReady();
    }

    public int GetCharacterLockCount()
    {
        return characterLocks.Count;
    }

    protected override bool CanStartGameInternal()
    {
        if (characterLocks.Count != 2)
            return false;

        foreach (var player in characterLocks.Values)
        {
            if (!player.IsReady())
                return false;
        }

        return true;
    }

    public bool CanStartGame()
    {
        return startButton != null && startButton.gameObject.activeSelf;
    }

    public override void StartGameButtonPressed()
    {
        base.StartGameButtonPressed();

        PlayerDataOffline.players.Clear();

        foreach (var entry in characterLocks)
        {
            PlayerDataOffline.players.Add(new OfflinePlayer
            {
                SelectedCharacter = entry.Key,
                DisplayName = entry.Value.GetDisplayName(),
                Device = entry.Value.assignedDevice
            });
        }

        string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndexToLoad);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        SceneManager.LoadScene(sceneName);
    }
}
