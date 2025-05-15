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


    protected override void Start()
    {
        base.Start();
        isOnline = false;

        if (!isOnline)
        {
            ShowLobbyPanel("Offline Co-Op");
        }

        // TEMP LOG FOR EASE
        GameManager.Instance.SetIsOnline(false);

    }

    private void Update()
    {
        if (joinedDevices.Count >= 2) return;

        foreach (var device in InputSystem.devices)
        {
            if (joinedDevices.Contains(device))
                continue;

            if (DeviceJoinPressed(device))
            {
                SpawnLocalPlayer(device);
            }
        }
    }

    private bool DeviceJoinPressed(InputDevice device)
    {
        if (device is Keyboard keyboard)
        {
            return keyboard.spaceKey.wasPressedThisFrame;
        }

        if (device is Gamepad gamepad)
        {
            return gamepad.buttonSouth.wasPressedThisFrame;
        }

        return false;
    }

    private void SpawnLocalPlayer(InputDevice device)
    {
        Debug.Log($"[LobbyOfflineManager] Spawning local player with device: {device.displayName}");

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

        // Remove device from joined list
        if (joinedDevices.Contains(player.assignedDevice))
        {
            joinedDevices.Remove(player.assignedDevice);
        }

        // Remove from characterLocks if ready
        if (characterLocks.ContainsValue(player))
        {
            foreach (var key in new List<int>(characterLocks.Keys))
            {
                if (characterLocks[key] == player)
                {
                    characterLocks.Remove(key);
                }
            }
        }

        // Destroy the player GameObject
        Destroy(player.gameObject);

        // Recheck if start button should still be active
        CheckIfAllReady();
    }


    public void RegisterReady(LobbyPlayerControllerOffline player, int selectedCharacter)
    {
        if (!characterLocks.ContainsKey(selectedCharacter))
        {
            characterLocks[selectedCharacter] = player;
            Debug.Log($"[LobbyOffline] Registered player {player.name} as ready for character {selectedCharacter}");
        }
        else
        {
            Debug.LogWarning($"[LobbyOffline] Character {selectedCharacter} already locked by another player.");
        }

        CheckIfAllReady();
    }


    public void UnregisterReady(LobbyPlayerControllerOffline player, int selectedCharacter)
    {
        if (characterLocks.TryGetValue(selectedCharacter, out var p) && p == player)
            characterLocks.Remove(selectedCharacter);

        CheckIfAllReady();
    }

    public bool CanStartGame()
    {
        return startButton != null && startButton.gameObject.activeSelf;
    }

    protected override bool CanStartGameInternal()
    {
        if (characterLocks.Count != 2)
            return false;

        foreach (var player in characterLocks.Values)
        {
            if (!player.IsReady)
                return false;
        }

        return true;
    }


    public int GetCharacterLockCount()
    {
        return characterLocks.Count;
    }

    public override void StartGameButtonPressed()
    {
        base.StartGameButtonPressed();
        OfflinePlayerDataManager.players.Clear();

        foreach (var entry in characterLocks)
        {
            Debug.Log($"[StartGame] Saving device for {entry.Value.playerNameText.text}: {entry.Value.assignedDevice?.displayName}");

            OfflinePlayerDataManager.players.Add(new OfflinePlayer
            {
                SelectedCharacter = entry.Key,
                DisplayName = entry.Value.playerNameText.text,
                Device = entry.Value.assignedDevice
            });
        }

        string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndexToLoad);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        SceneManager.LoadScene(sceneName);
    }
}
