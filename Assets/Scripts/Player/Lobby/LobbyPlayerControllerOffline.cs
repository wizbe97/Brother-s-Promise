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
    [HideInInspector] public LobbyOffline lobbyManager;


    public void Initialize(int playerIndex)
    {
        playerNameText.text = $"Player {playerIndex + 1}";
        displayName = $"Player {playerIndex + 1}";


        selectedCharacter = 0;
        UpdateVisualPosition();

        RectTransform rect = GetComponent<RectTransform>();
        if (playerIndex == 1 && rect != null)
        {
            rect.anchoredPosition -= new Vector2(0, 200f);
        }

        // Device pairing
        inputUser = InputUser.CreateUserWithoutPairedDevices();
        InputUser.PerformPairingWithDevice(assignedDevice, user: inputUser);
        inputUser.AssociateActionsWithUser(inputActions);
    }

    protected override void OnMoveLeft()
    {
        if (isReady) return;

        selectedCharacter = 0; // Brother 1
        UpdateVisualPosition();
    }

    protected override void OnMoveRight()
    {
        if (isReady) return;

        selectedCharacter = 1; // Brother 2
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


    protected override void UpdateDisplayName()
    {
        if (playerNameText != null)
            playerNameText.text = displayName;
    }

    protected override void UpdateVisualPosition()
    {
        Transform targetParent = selectedCharacter == 0 ? imageBrother1 : imageBrother2;
        if (transform.parent != targetParent)
        {
            Vector3 currentLocalPosition = transform.localPosition;
            transform.SetParent(targetParent, false);
            transform.localPosition = new Vector3(0f, currentLocalPosition.y, 0f);
        }
    }

    protected override void UpdateReadyVisual()
    {
        if (backgroundImage != null)
            backgroundImage.color = isReady ? Color.green : Color.white;
    }

    public int GetSelectedCharacter()
    {
        return selectedCharacter;
    }

    public bool IsReady => isReady;

    protected override void OnStartGamePressed()
    {
        if (lobbyManager != null && lobbyManager.CanStartGame())
        {
            lobbyManager.StartGameButtonPressed();
        }
    }


    protected override void OnEscapePressed()
    {
        if (isReady)
        {
            // If already ready, unready first
            isReady = false;
            UpdateReadyVisual();
            lobbyManager.UnregisterReady(this, selectedCharacter);
            Debug.Log("[LobbyPlayerControllerOffline] Player un-readied via Escape");
        }
        else
        {
            // Otherwise, fully remove this player
            Debug.Log("[LobbyPlayerControllerOffline] Player left the lobby via Escape");

            lobbyManager.RemovePlayer(this);
        }
    }

}
