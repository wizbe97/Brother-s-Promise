using UnityEngine;

public interface IPhysicsMover
{
    bool UsesBounding { get; }

    // The player must ground at least once before bounding effector applies
    bool RequireGrounding { get; }

    Vector2 FramePositionDelta { get; }
    Vector2 FramePosition { get; }
    Vector2 Velocity { get; }
    Vector2 TakeOffVelocity { get; }
}
