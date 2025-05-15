using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class OfflinePlayerDataManager
{
    public static List<OfflinePlayer> players = new List<OfflinePlayer>();
    
}


public struct OfflinePlayer
{
    public InputDevice Device;
    public int SelectedCharacter;
    public string DisplayName;
}

