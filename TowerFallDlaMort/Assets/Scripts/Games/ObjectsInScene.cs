using Scripts.Items;
using Scripts.Players;
using UnityEngine;

namespace Games
{
    public class ObjectsInScene : MonoBehaviour
    {
        [SerializeField] 
        public PlayerExposer player1Exposer;
        
        [SerializeField] 
        public PlayerExposer player2Exposer;
        
        [SerializeField] 
        public ItemExposer[] itemExposer;
    }
}
