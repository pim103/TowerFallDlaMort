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

        public bool wantToShootUp;
        public bool wantToShootDown;
        public bool wantToShootLeft;
        public bool wantToShootRight;

        public bool wantToBlock;
    }
}