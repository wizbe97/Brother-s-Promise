using UnityEngine;
using Fusion;
using FusionUtilsEvents;
using Cinemachine;
using System.Linq;
using UnityEngine.InputSystem;

public class GameplayManager : NetworkBehaviour
{
    [Header("Character Prefabs (Online)")]
    [SerializeField] private NetworkPrefabRef brother1NetworkPrefab;
    [SerializeField] private NetworkPrefabRef brother2NetworkPrefab;

    [Header("Character Prefabs (Offline)")]
    [SerializeField] private GameObject brother1OfflinePrefab;
    [SerializeField] private GameObject brother2OfflinePrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    [Header("Fusion Events")]
    public FusionEvent OnSceneLoaded;

    private static GameplayManager _instance;
    public static GameplayManager Instance => _instance;

    private bool _isOnline;
    private CinemachineTargetGroup _targetGroup;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (GameManager.Instance != null)
            _isOnline = GameManager.Instance.IsOnline;

        _targetGroup = FindObjectOfType<CinemachineTargetGroup>();

#if UNITY_EDITOR
        if (GameManager.Instance == null)
        {
            DebugSpawnPlayers();
            return;
        }
#endif

        if (!_isOnline)
            HandleOfflineSpawning();
    }


    private void OnEnable()
    {
        OnSceneLoaded?.RegisterResponse(SceneLoaded);
    }

    private void OnDisable()
    {
        OnSceneLoaded?.RemoveResponse(SceneLoaded);
    }

    private void SceneLoaded(PlayerRef _, NetworkRunner runner)
    {
        if (_isOnline)
            HandleOnlineSpawning(runner);
        else
            HandleOfflineSpawning();
    }

    private void HandleOnlineSpawning(NetworkRunner runner)
    {
        Debug.Log("[GameplayManager] Handling online spawning...");
        if (runner == null || !runner.IsServer)
            return;

        int index = 0;
        foreach (var player in runner.ActivePlayers)
        {
            var playerData = GameManager.Instance.GetPlayerData(player);

            NetworkPrefabRef prefabToSpawn = playerData.SelectedCharacter == 0
                ? brother1NetworkPrefab
                : brother2NetworkPrefab;

            Transform spawnPoint = GetSpawnPoint(index);

            var spawnedObject = runner.Spawn(
                prefabToSpawn,
                spawnPoint.position,
                spawnPoint.rotation,
                player
            );
            // Add to Cinemachine Target Group
            if (_targetGroup != null)
            {
                _targetGroup.AddMember(spawnedObject.transform, 1f, 2f);
            }
            index++;
        }

    }

    private void HandleOfflineSpawning()
    {
        Debug.Log("[GameplayManager] Handling offline spawning...");

        foreach (var player in PlayerDataOffline.players)
        {
            var prefab = (player.SelectedCharacter == 0) ? brother1OfflinePrefab : brother2OfflinePrefab;
            var spawnPos = GetSpawnPoint(player.SelectedCharacter).position;

            var go = Instantiate(prefab, spawnPos, Quaternion.identity);
            var controller = go.GetComponent<GameplayController>();
            controller.InitializeInput(player.Device, player.SelectedCharacter);

            // Add to Cinemachine Target Group
            if (_targetGroup != null)
            {
                _targetGroup.AddMember(go.transform, 1f, 2f);
            }
        }
    }

    private void DebugSpawnPlayers()
    {
#if UNITY_EDITOR
        var keyboard = Keyboard.current;
        var gamepad = Gamepad.all.FirstOrDefault();

        if (keyboard == null || gamepad == null)
            return;

        // Manually create offline players (skips the LobbyOffline scene)
        PlayerDataOffline.players.Clear();

        PlayerDataOffline.players.Add(new OfflinePlayer
        {
            Device = gamepad,
            SelectedCharacter = 0,
            DisplayName = "Gamepad Debug"
        });

        PlayerDataOffline.players.Add(new OfflinePlayer
        {
            Device = keyboard,
            SelectedCharacter = 1,
            DisplayName = "Keyboard Debug"
        });

        // Manually spawn and initialize the players
        foreach (var player in PlayerDataOffline.players)
        {
            GameObject prefab = player.SelectedCharacter == 0 ? brother1OfflinePrefab : brother2OfflinePrefab;
            Vector3 spawnPos = GetSpawnPoint(player.SelectedCharacter).position;

            GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);

            var controller = go.GetComponent<GameplayController>();
            if (controller != null)
                controller.InitializeInput(player.Device, player.SelectedCharacter);

            if (_targetGroup != null)
                _targetGroup.AddMember(go.transform, 1f, 2f);
        }
#endif
    }



    private Transform GetSpawnPoint(int index)
    {
        return index == 0 ? spawnPoint1 : spawnPoint2;
    }
}
