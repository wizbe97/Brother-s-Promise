using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using FusionUtilsEvents;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool IsOnline { get; private set; }


    [Header("Events")]
    public FusionEvent OnPlayerLeftEvent;
    public FusionEvent OnRunnerShutDownEvent;
    public FusionEvent OnDisconnectedEvent;

    public NetworkRunner runner;
    private Dictionary<PlayerRef, PlayerDataOnline> _playerDatas = new();

    public enum GameState
    {
        Lobby,
        Playing,
        Loading
    }

    public GameState State { get; private set; }

    [Header("Scene/Session")]
    public LevelManager LevelManager;
    [SerializeField] private GameObject exitCanvas;

    private Dictionary<PlayerRef, PlayerDataOnline> _playerData = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(transform.parent ? transform.parent.gameObject : gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(transform.parent ? transform.parent.gameObject : gameObject);
    }


    private void OnEnable()
    {
        OnPlayerLeftEvent?.RegisterResponse(PlayerDisconnected);
        OnRunnerShutDownEvent?.RegisterResponse(OnRunnerShutdown);
        OnDisconnectedEvent?.RegisterResponse(OnDisconnected);

    }

    private void OnDisable()
    {
        OnPlayerLeftEvent?.RemoveResponse(PlayerDisconnected);
        OnRunnerShutDownEvent?.RemoveResponse(OnRunnerShutdown);
        OnDisconnectedEvent?.RemoveResponse(OnDisconnected);

    }

    public void SetIsOnline(bool online)
    {
        IsOnline = online;
    }


    public void DisconnectAndReturnToOffline()
    {
        ExitSession();
    }

    public void SetGameState(GameState state)
    {
        State = state;
    }

    public void SetPlayerDataObject(PlayerRef player, PlayerDataOnline data)
    {
        if (!_playerData.ContainsKey(player))
        {
            _playerData.Add(player, data);
        }
    }

    public PlayerDataOnline GetPlayerData(PlayerRef playerRef)
    {
        _playerDatas.TryGetValue(playerRef, out var playerData);
        return playerData;
    }

    public void RegisterPlayerData(PlayerRef playerRef, PlayerDataOnline playerData)
    {
        if (!_playerDatas.ContainsKey(playerRef))
        {
            _playerDatas.Add(playerRef, playerData);
        }
    }


    public void PlayerDisconnected(PlayerRef player, NetworkRunner runner)
    {
        if (_playerData.TryGetValue(player, out var data))
        {
            if (runner.IsServer)
            {
                if (SceneManager.GetActiveScene().name == "2_LobbyOnline")
                {
                    foreach (var controller in FindObjectsOfType<LobbyPlayerControllerOnline>())
                    {
                        if (controller.Object != null && controller.Object.InputAuthority == player)
                        {
                            runner.Despawn(controller.Object);
                        }
                    }
                }
                else if (SceneManager.GetActiveScene().name == "4_Gameplay1")
                {
                    foreach (var controller in FindObjectsOfType<GameplayController>())
                    {
                        if (controller.Object != null && controller.Object.InputAuthority == player)
                        {
                            runner.Despawn(controller.Object);
                        }
                    }

                    ExitSession();
                }
            }

            if (data.Instance) runner.Despawn(data.Instance);
            if (data.Object) runner.Despawn(data.Object);
            _playerData.Remove(player);
        }
    }


    private void OnDisconnected(PlayerRef _, NetworkRunner runner)
    {
        ExitSession();
    }


    public void OnRunnerShutdown(PlayerRef _, NetworkRunner runner)
    {
        ExitSession();
    }


    public void LeaveRoom()
    {
        _ = LeaveRoomAsync();
    }

    private async Task LeaveRoomAsync()
    {
        await ShutdownRunner();
    }

    private async Task ShutdownRunner()
    {
        if (FusionHelper.LocalRunner != null)
        {
            await FusionHelper.LocalRunner.Shutdown();
        }

        SetGameState(GameState.Lobby);
        _playerData.Clear();
    }


    public void ExitSession()
    {
        LevelManager?.ResetLoadedScene();
        SceneManager.LoadScene(0);

        if (exitCanvas != null)
            exitCanvas.SetActive(false);

        if (runner != null)
            _ = ShutdownRunner();
    }


    public void ExitGame()
    {
        _ = ShutdownRunner();
        Application.Quit();
    }
}
