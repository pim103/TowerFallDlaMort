using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scripts.Players
{
    public class PlayerExposer : MonoBehaviour
    {
        [SerializeField]
        public Transform playerTransform;

        [SerializeField]
        public PlayerIntent playerIntent;
    }
}
