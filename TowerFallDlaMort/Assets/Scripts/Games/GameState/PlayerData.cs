﻿using System.Collections;
using System.Collections.Generic;
using Games.Agents;
using Unity.Collections;
using UnityEngine;

namespace Games.GameState
{
    public struct PlayerData {
        public Vector3 playerPosition;
        public float PlayerSpeed;
        public float PlayerRadius;
        public int PlayerScore;
        public Weapon weapon;
        public int PlayerLifeStock;
        public bool IsUntouchable;
    }  
}
