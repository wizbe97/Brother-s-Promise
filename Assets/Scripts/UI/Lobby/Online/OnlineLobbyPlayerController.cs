using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Fusion;

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

        StartCoroutine(SetInitialParent());
    }

    private IEnumerator SetInitialParent()
    {
        yield return null; // wait one frame so LobbyCanvas assigns the images

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

            // Always offset based on PlayerId
            float verticalSpacing = -200f;
            int playerIndex = Object.InputAuthority.PlayerId - 1; // PlayerId starts at 1, so subtract 1
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
        inputActions.Lobby.SwapCharacter.performed += OnSwapCharacter;
        inputActions.Lobby.ReadyUp.performed += OnReadyUp;
    }

    private void OnDisable()
    {
        inputActions.Lobby.SwapCharacter.performed -= OnSwapCharacter;
        inputActions.Lobby.ReadyUp.performed -= OnReadyUp;
        inputActions.Lobby.Disable();
    }

    private void Update()
    {
        if (playerData == null) return;

        playerNameText.text = playerData.DisplayName.ToString();
        UpdateVisualPosition();
        UpdateReadyVisual();
    }

    private void OnSwapCharacter(InputAction.CallbackContext ctx)
    {
        if (!hasInputAuthority || playerData == null || playerData.IsReady) return;

        Vector2 input = ctx.ReadValue<Vector2>();
        if (Mathf.Abs(input.x) > 0.5f)
        {
            int newCharacter = playerData.SelectedCharacter == 0 ? 1 : 0;
            playerData.RPC_SetCharacter(newCharacter);
        }
    }

    private void OnReadyUp(InputAction.CallbackContext ctx)
    {
        if (!hasInputAuthority || playerData == null) return;

        bool nextReadyState = !playerData.IsReady;
        playerData.RPC_SetReady(nextReadyState);
    }

    private void UpdateVisualPosition()
    {
        if (playerData == null) return;

        Transform targetParent = playerData.SelectedCharacter == 0 ? imageBrother1 : imageBrother2;
        if (transform.parent != targetParent)
        {
            Vector3 currentLocalPosition = transform.localPosition;
            transform.SetParent(targetParent, false);
            transform.localPosition = new Vector3(0f, currentLocalPosition.y, 0f); // Keep same Y offset after swap
        }
    }


    private void UpdateReadyVisual()
    {
        backgroundImage.color = playerData.IsReady ? Color.green : Color.white;
    }
}
