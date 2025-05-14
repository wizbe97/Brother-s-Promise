using UnityEngine;
using Fusion;
using FusionUtilsEvents;
using System.Linq;

public class GameplayManager : NetworkBehaviour
{
    [Header("Character Prefabs")]
    [SerializeField] private NetworkPrefabRef brother1Prefab;
    [SerializeField] private NetworkPrefabRef brother2Prefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    [Header("Fusion Events")]
    public FusionEvent OnSceneLoaded;
    public FusionEvent OnPlayerLeft;

    private static GameplayManager _instance;
    public static GameplayManager Instance => _instance;

    private NetworkRunner _cachedRunner;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _cachedRunner = FusionHelper.LocalRunner;
        if (_cachedRunner == null)
        {
            Debug.LogError("[GameplayManager] Failed to find LocalRunner in Awake. Spawns might fail!");
        }
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
        Debug.Log("[GameplayManager] SceneLoaded event received.");

        if (runner == null)
        {
            Debug.LogError("[GameplayManager] Runner is NULL. Cannot spawn players.");
            return;
        }

        if (!runner.IsServer)
        {
            Debug.Log("[GameplayManager] Not the server. Skipping player spawn.");
            return;
        }

        Debug.Log("[GameplayManager] Server is spawning players...");

        int index = 0;
        foreach (var player in runner.ActivePlayers)
        {
            var playerData = GameManager.Instance.GetPlayerData(player);
            if (playerData == null)
            {
                Debug.LogWarning($"[GameplayManager] No PlayerData found for {player}. Skipping.");
                continue;
            }

            NetworkPrefabRef prefabToSpawn = playerData.SelectedCharacter == 0
                ? brother1Prefab
                : brother2Prefab;

            Transform spawnPoint = GetSpawnPoint(index);

            var spawnedObject = runner.Spawn(
                prefabToSpawn,
                spawnPoint.position,
                spawnPoint.rotation,
                player 
            );

            if (spawnedObject != null)
            {
                Debug.Log($"[GameplayManager] Spawned {prefabToSpawn} for {player} at {spawnPoint.position}");
            }
            else
            {
                Debug.LogError($"[GameplayManager] Failed to spawn prefab for {player}");
            }

            index++;
        }
    }

    private Transform GetSpawnPoint(int index)
    {
        return index == 0 ? spawnPoint1 : spawnPoint2;
    }
}
