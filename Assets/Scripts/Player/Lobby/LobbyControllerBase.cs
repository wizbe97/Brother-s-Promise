using Fusion;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public abstract class LobbyPlayerControllerBase : NetworkBehaviour
{
    [Header("References")]
    public TextMeshProUGUI playerNameText;
    public Image backgroundImage;

    [Header("Brother Images")]
    public Transform imageBrother1;
    public Transform imageBrother2;

    protected LobbyInputActions inputActions;

    protected virtual void Awake()
    {
        inputActions = new LobbyInputActions();
    }

    protected virtual void OnEnable()
    {
        inputActions.Lobby.Enable();
        inputActions.Lobby.MoveLeft.performed += ctx => OnMoveLeft();
        inputActions.Lobby.MoveRight.performed += ctx => OnMoveRight();
        inputActions.Lobby.Ready.performed += ctx => OnReadyUp();
        inputActions.Lobby.StartGame.performed += ctx => OnStartGamePressed();
        inputActions.Lobby.Escape.performed += ctx => OnEscapePressed();
    }

    protected virtual void OnDisable()
    {
        inputActions.Lobby.MoveLeft.performed -= ctx => OnMoveLeft();
        inputActions.Lobby.MoveRight.performed -= ctx => OnMoveRight();
        inputActions.Lobby.Ready.performed -= ctx => OnReadyUp();
        inputActions.Lobby.StartGame.performed -= ctx => OnStartGamePressed();
        inputActions.Lobby.Escape.performed -= ctx => OnEscapePressed();
        inputActions.Lobby.Disable();
    }

    protected virtual void Start()
    {
        SetInitialParent();
    }

    protected virtual void Update()
    {
        UpdateDisplayName();
        UpdateVisualPosition();
        UpdateReadyVisual();
    }

    protected virtual void SetInitialParent()
    {
        if (imageBrother1 == null || imageBrother2 == null)
        {
            var lobbyCanvas = FindObjectOfType<LobbyBase>();
            if (lobbyCanvas != null)
            {
                imageBrother1 = lobbyCanvas.imageBrother1;
                imageBrother2 = lobbyCanvas.imageBrother2;
            }
        }

        if (imageBrother1 != null)
        {
            transform.SetParent(imageBrother1, false);
        }
    }


    protected abstract void OnMoveLeft();
    protected abstract void OnMoveRight();
    protected abstract void OnReadyUp();
    protected abstract void UpdateDisplayName();
    protected abstract void UpdateVisualPosition();
    protected abstract void UpdateReadyVisual();
    protected abstract void OnStartGamePressed();
    protected abstract void OnEscapePressed();

}
