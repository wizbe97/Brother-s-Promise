using UnityEngine;
using Fusion;

public struct FrameInput : INetworkInput
{
    public Vector2 Move;
    public NetworkBool JumpDown;
    public NetworkBool JumpHeld;
    public NetworkBool DashDown;
    public NetworkBool LadderHeld;
    public float JumpPressedTime;
}
