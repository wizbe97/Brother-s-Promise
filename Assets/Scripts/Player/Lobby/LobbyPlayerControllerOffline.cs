using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class LobbyPlayerControllerOffline : LobbyPlayerControllerBase
{
    private InputUser inputUser;
    public InputDevice assignedDevice;

    private int selectedCharacter = 0;
    private string displayName;
    private bool isReady = false;
    private int playerIndex;

    [HideInInspector] public LobbyOffline lobbyManager;

    public void Initialize(int index)
    {
        playerIndex = index;
        playerNameText.text = $"Player {playerIndex + 1}";
        displayName = $"Player {playerIndex + 1}";
        selectedCharacter = 0;
        UpdateVisualPosition();

        RectTransform rect = GetComponent<RectTransform>();
        if (playerIndex == 1 && rect != null)
        {
            rect.anchoredPosition -= new Vector2(0, 200f);
        }

        inputUser = InputUser.CreateUserWithoutPairedDevices();
        InputUser.PerformPairingWithDevice(assignedDevice, user: inputUser);
        inputUser.AssociateActionsWithUser(inputActions);
    }

    protected override void OnMoveLeft()
    {
        if (isReady) return;
        selectedCharacter = 0;
        UpdateVisualPosition();
    }

    protected override void OnMoveRight()
    {
        if (isReady) return;
        selectedCharacter = 1;
        UpdateVisualPosition();
    }

    protected override void OnReadyUp()
    {
        if (isReady)
        {
            isReady = false;
            UpdateReadyVisual();
            lobbyManager.UnregisterReady(this, selectedCharacter);
        }
        else
        {
            isReady = true;
            UpdateReadyVisual();
            lobbyManager.RegisterReady(this, selectedCharacter);
        }
    }

    protected override void OnStartGamePressed()
    {
        if (playerIndex == 0 && lobbyManager != null && lobbyManager.CanStartGame())
            lobbyManager.StartGameButtonPressed();
    }


    protected override void OnEscapePressed()
    {
        if (isReady)
        {
            isReady = false;
            UpdateReadyVisual();
            lobbyManager.UnregisterReady(this, selectedCharacter);
        }
        else
            lobbyManager.RemovePlayer(this);
    }

    // --- New Implementations for Base ---
    public override string GetDisplayName()
    {
        return displayName;
    }

    public override int GetSelectedCharacter()
    {
        return selectedCharacter;
    }

    public override bool IsReady()
    {
        return isReady;
    }
}
