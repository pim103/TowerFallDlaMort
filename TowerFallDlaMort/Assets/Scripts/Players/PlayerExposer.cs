using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace Scripts.Players
{
    public class PlayerExposer : MonoBehaviour
    {
        [SerializeField]
        public Transform playerTransform;

        [FormerlySerializedAs("playerIntent")] [SerializeField]
        public PlayerAgent playerAgent;
    }
}
