using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Fusion;
using System.Linq;

public class OnlineLobbyPlayerController : NetworkBehaviour
{
    [Header("References")]
    public TextMeshProUGUI playerNameText;
    public Image backgroundImage;

    [Header("Brother Images")]
    public Transform imageBrother1;
    public Transform imageBrother2;

    private PlayerData playerData;
    private LobbyInputActions inputActions;
    private bool hasInputAuthority;


    public override void Spawned()
    {
        hasInputAuthority = Object.HasInputAuthority;

        var obj = Runner.GetPlayerObject(Object.InputAuthority);
        if (obj != null)
        {
            playerData = obj.GetComponent<PlayerData>();
        }

        if (hasInputAuthority && playerData != null)
        {
            playerData.LobbyPlayerController = GetComponent<NetworkObject>();
        }

        SetInitialParent();
        playerNameText.text = playerData.DisplayName.ToString();
    }

    private void SetInitialParent()
    {
        if (imageBrother1 == null || imageBrother2 == null)
        {
            var lobbyCanvas = FindObjectOfType<LobbyCanvas>();
            if (lobbyCanvas != null)
            {
                imageBrother1 = lobbyCanvas.imageBrother1;
                imageBrother2 = lobbyCanvas.imageBrother2;
            }
        }

        if (imageBrother1 != null)
        {
            transform.SetParent(imageBrother1, false);

            // Find this player's real index
            int playerIndex = 0;
            foreach (var player in Runner.ActivePlayers)
            {
                if (player == Object.InputAuthority)
                    break;
                playerIndex++;
            }

            float verticalSpacing = -200f;
            transform.localPosition = new Vector3(0f, playerIndex * verticalSpacing, 0f);
        }
    }


    private void Awake()
    {
        inputActions = new LobbyInputActions();
    }

    private void OnEnable()
    {
        inputActions.Lobby.Enable();
        inputActions.Lobby.MoveLeft.performed += ctx => OnMoveLeft();
        inputActions.Lobby.MoveRight.performed += ctx => OnMoveRight();
        inputActions.Lobby.ReadyUp.performed += OnReadyUp;
    }

    private void OnDisable()
    {
        inputActions.Lobby.MoveLeft.performed -= ctx => OnMoveLeft();
        inputActions.Lobby.MoveRight.performed -= ctx => OnMoveRight();
        inputActions.Lobby.ReadyUp.performed -= OnReadyUp;
        inputActions.Lobby.Disable();
    }

    private void OnMoveLeft()
    {
        if (!hasInputAuthority || playerData == null || playerData.IsReady) return;
        playerData.RPC_SetCharacter(0);
        UpdateVisualPosition();
    }

    private void OnMoveRight()
    {
        if (!hasInputAuthority || playerData == null || playerData.IsReady) return;
        playerData.RPC_SetCharacter(1);
        UpdateVisualPosition();
    }


    private void OnReadyUp(InputAction.CallbackContext ctx)
    {
        if (!hasInputAuthority || playerData == null) return;

        bool nextReadyState = !playerData.IsReady;
        playerData.RPC_SetReady(nextReadyState);

        UpdateVisualPosition();
        UpdateReadyVisual();
    }

    private void UpdateVisualPosition()
    {
        if (playerData == null) return;

        Transform targetParent = playerData.SelectedCharacter == 0 ? imageBrother1 : imageBrother2;
        if (transform.parent != targetParent)
        {
            Vector3 currentLocalPosition = transform.localPosition;
            transform.SetParent(targetParent, false);
            transform.localPosition = new Vector3(0f, currentLocalPosition.y, 0f); 
        }
    }

    private void UpdateReadyVisual()
    {
        backgroundImage.color = playerData.IsReady ? Color.green : Color.white;
    }
}
