﻿using Unity.Collections;

namespace Games.GameState
{
    public struct GameStateData
    {
        public NativeList<PlayerData> players;
        public NativeList<ProjectileData> projectiles;
        public NativeList<ItemsData> items;
        public const float projectileSpeed = 0.5f / 60f * 10f;

        //public int gameTime;
        public float lastDistance;
    }
}
