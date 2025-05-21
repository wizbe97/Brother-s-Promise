using UnityEngine;
using Fusion;
using FusionUtilsEvents;

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
        _isOnline = GameManager.Instance.IsOnline;

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

            var prefabToSpawn = playerData.SelectedCharacter == 0
                ? brother1NetworkPrefab
                : brother2NetworkPrefab;

            var spawnPoint = GetSpawnPoint(index);

            runner.Spawn(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, player);
            index++;
        }
    }

    private void HandleOfflineSpawning()
    {
        Debug.Log("[GameplayManager] Handling offline spawning...");

        foreach (var player in PlayerDataOffline.players)
        {
            var prefab = player.SelectedCharacter == 0
                ? brother1OfflinePrefab
                : brother2OfflinePrefab;

            var spawnPos = GetSpawnPoint(player.SelectedCharacter).position;
            var go = Instantiate(prefab, spawnPos, Quaternion.identity);

            var controller = go.GetComponent<GameplayController>();
            controller.InitializeInput(player.Device, player.SelectedCharacter);
        }
    }

    private Transform GetSpawnPoint(int index)
    {
        return index == 0 ? spawnPoint1 : spawnPoint2;
    }
}
