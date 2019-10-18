using System.Collections.Generic;
using Scripts.Games;
using UnityEngine;

namespace Games
{
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler sharedInstance;
        public GameController gc;

        void Awake() {
            sharedInstance = this;
        }
        
        public GameObject GetPooledObject() {
//1
            for (int i = 0; i < gc.amountOfProjectilesToPooled; i++) {
//2
                if (!gc.projectiles[i].activeInHierarchy) {
                    return gc.projectiles[i];
                }
            }
//3   
            return null;
        }

    }
}