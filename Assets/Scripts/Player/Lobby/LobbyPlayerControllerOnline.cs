using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

public class LobbyPlayerControllerOnline : LobbyPlayerControllerBase
{
    private PlayerDataOnline playerData;
    private bool hasInputAuthority;

    public override void Spawned()
    {
        hasInputAuthority = Object.HasInputAuthority;

        var obj = Runner.GetPlayerObject(Object.InputAuthority);
        if (obj != null)
        {
            playerData = obj.GetComponent<PlayerDataOnline>();
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

            int playerIndex = 0;
            foreach (var player in Runner.ActivePlayers)
            {
                if (player == Object.InputAuthority)
                    break;
                playerIndex++;
            }

            float verticalSpacing = -200f;
            transform.localPosition = new Vector3(0f, playerIndex * verticalSpacing, 0f);

            UpdateDisplayName();
            UpdateVisualPosition();
            UpdateReadyVisual();
        }
    }

    protected override void OnMoveLeft()
    {
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

    protected override void OnStartGamePressed()
    {
        if (!hasInputAuthority || playerData == null) return;

        if (Runner.IsServer)
        {
            var lobbyManager = FindObjectOfType<LobbyOnline>();
            if (lobbyManager != null)
            {
                lobbyManager.StartGameButtonPressed();
            }
        }
    }


    protected override void OnEscapePressed()
    {
        if (!hasInputAuthority || playerData == null) return;

        if (playerData.IsReady)
            playerData.RPC_SetReady(false);
        else
            GameManager.Instance.ExitSession();
    }


    // --- New Implementations for Base ---
    public override string GetDisplayName()
    {
        return playerData != null ? playerData.DisplayName.ToString() : "Player";
    }

    public override int GetSelectedCharacter()
    {
        return playerData != null ? playerData.SelectedCharacter : 0;
    }

    public override bool IsReady()
    {
        return playerData != null && playerData.IsReady;
    }
}
