using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Players
{
    public class PlayerIntent : MonoBehaviour
    {
        public bool wantToMoveBack;
        public bool wantToMoveForward;
        public bool wantToMoveRight;
        public bool wantToMoveLeft;

        public bool wantToShoot;

        public bool wantToBlock;
    }
}