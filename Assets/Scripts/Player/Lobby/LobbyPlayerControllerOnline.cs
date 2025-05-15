using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Fusion;

public class LobbyPlayerControllerOnline : LobbyPlayerControllerBase
{
    private PlayerData playerData;
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
    }

    protected override void SetInitialParent()
    {
        base.SetInitialParent();
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
            playerNameText.text = playerData.DisplayName.ToString();
            UpdateVisualPosition();
            UpdateReadyVisual();
        }
    }
    protected override void OnMoveLeft()
    {
        Debug.Log("Move Left attempted. HasInputAuthority: " + hasInputAuthority + "PlayerData: " + playerData + " IsReady: " + (playerData != null ? playerData.IsReady.ToString() : "null"));
        if (!hasInputAuthority || playerData == null || playerData.IsReady) return;
        playerData.RPC_SetCharacter(0);
    }

    protected override void OnMoveRight()
    {
        if (!hasInputAuthority || playerData == null || playerData.IsReady) return;
        playerData.RPC_SetCharacter(1);
    }

    protected override void OnReadyUp()
    {
        if (!hasInputAuthority || playerData == null) return;

        bool nextReadyState = !playerData.IsReady;
        playerData.RPC_SetReady(nextReadyState);
    }

    protected override void UpdateDisplayName()
    {
        if (playerData != null)
            playerNameText.text = playerData.DisplayName.ToString();
    }

    protected override void UpdateVisualPosition()
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

    protected override void UpdateReadyVisual()
    {
        backgroundImage.color = playerData.IsReady ? Color.green : Color.white;
    }

    protected override void OnStartGamePressed()
    {
        if (!hasInputAuthority || playerData == null) return;

        // Only the host should actually start the game
        if (Runner.IsServer)
        {
            // Here you would normally trigger your host-only start logic
            // For now just log it
            Debug.Log("[LobbyPlayerControllerOnline] Host requested to start the game");

            // Example: you could call a LobbyManagerOnline.StartGameButtonPressed() here
            // LobbyManagerOnline.Instance.StartGameButtonPressed();
        }
    }

    protected override void OnEscapePressed()
    {
        if (!hasInputAuthority || playerData == null) return;

        // Player wants to leave the lobby
        Debug.Log("[LobbyPlayerControllerOnline] Player pressed Escape to leave lobby");

        // In online mode: you don't manually destroy their player.
        // Instead you can tell Fusion to disconnect or de-spawn properly.

        // Safest: Disconnect the player from the session
        Runner.Disconnect(Object.InputAuthority);
    }

}
