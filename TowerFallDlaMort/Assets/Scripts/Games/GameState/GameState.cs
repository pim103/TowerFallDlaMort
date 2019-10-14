using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct GameState
{
    public NativeList<PlayerData> players;
    public NativeList<ProjectileData> projectiles;
    public const float projectileSpeed = 0.5f / 60f * 10f;
}
