using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.GameState
{
    public struct ProjectileData
    {
        public Vector3 position;
        public Vector3 speed;
        public float radius;
        public int ownerId;
        public int timer;
    }
}
