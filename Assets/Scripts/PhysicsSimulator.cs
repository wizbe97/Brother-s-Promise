using System.Collections.Generic;
using UnityEngine;
public class PhysicsSimulator : MonoBehaviour
{
    public static PhysicsSimulator Instance { get; private set; }

    private readonly HashSet<IPhysicsObject2> _platforms = new();
    private readonly HashSet<IPhysicsObject2> _players = new();

    private void Awake() => Instance = this;

    public void AddPlatform(IPhysicsObject2 platform) => _platforms.Add(platform);
    public void AddPlayer(IPhysicsObject2 player) => _players.Add(player);
    public void RemovePlatform(IPhysicsObject2 platform) => _platforms.Remove(platform);
    public void RemovePlayer(IPhysicsObject2 player) => _players.Remove(player);

    private float _time;

    private void Update()
    {
        var delta = Time.deltaTime;
        _time += delta;
        foreach (var platform in _platforms)
        {
            platform.TickUpdate(delta, _time);
        }

        foreach (var player in _players)
        {
            player.TickUpdate(delta, _time);
        }
    }

    private void FixedUpdate()
    {
        var delta = Time.deltaTime;
        foreach (var platform in _platforms)
        {
            platform.TickFixedUpdate(delta);
        }

        foreach (var player in _players)
        {
            player.TickFixedUpdate(delta);
        }
    }
}

public interface IPhysicsObject2
{
    public void TickFixedUpdate(float delta);
    public void TickUpdate(float delta, float time);
}

public interface IPhysicsMover2
{
    public bool UsesBounding { get; }

    [Tooltip("The player requires grounding on the platform at least once before the grounding effector takes effect")]
    public bool RequireGrounding { get; }

    public Vector2 FramePositionDelta { get; }
    public Vector2 FramePosition { get; }
    public Vector2 Velocity { get; }
    public Vector2 TakeOffVelocity { get; }
}

public static class SimulatorBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        var simulator = new GameObject("Simulator");
        simulator.AddComponent<PhysicsSimulator>();
        Object.DontDestroyOnLoad(simulator);
    }
}
