using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class GameplayManager : NetworkBehaviour
{
    [Header("Character Prefabs")]
    [SerializeField] private NetworkPrefabRef brother1Prefab;
    [SerializeField] private NetworkPrefabRef brother2Prefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    private Dictionary<PlayerRef, NetworkObject> spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

    private static GameplayManager _instance;
    public static GameplayManager Instance => _instance;

    private bool _playersSpawned = false;

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

    public override void Spawned()
    {
        if (!Runner.IsServer) return;
        // Don't immediately spawn here
    }

    private void Update()
    {
        if (!Runner || _playersSpawned || !Runner.IsServer)
            return;

        // Wait until players are fully active and their PlayerData objects exist
        bool allPlayersReady = true;

        foreach (var player in Runner.ActivePlayers)
        {
            var data = GameManager.Instance.GetPlayerData(player);
            if (data == null)
            {
                allPlayersReady = false;
                break;
            }
        }

        if (allPlayersReady)
        {
            SpawnAllPlayers();
            _playersSpawned = true;
        }
    }

    private void SpawnAllPlayers()
    {
        Debug.Log("[GameplayManager] Spawning all players...");

        int index = 0;
        foreach (var player in Runner.ActivePlayers)
        {
            var playerData = GameManager.Instance.GetPlayerData(player);
            if (playerData == null) continue;

            NetworkPrefabRef prefabToSpawn = playerData.SelectedCharacter == 0 ? brother1Prefab : brother2Prefab;
            Transform spawnPoint = index == 0 ? spawnPoint1 : spawnPoint2;

            var obj = Runner.Spawn(
                prefabToSpawn,
                spawnPoint.position,
                spawnPoint.rotation,
                player // Assign input authority
            );

            spawnedPlayers.Add(player, obj);
            index++;
        }
    }
}
