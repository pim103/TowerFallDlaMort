using System.Collections;
using System.Collections.Generic;
using Games;
using UnityEngine;

namespace Scripts.Players
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] 
        private ObjectsInScene ois;

        [SerializeField] 
        private GameObject projectilePrefab;

        private readonly List<Transform> players = new List<Transform>();
        /*
        private void Start()
        {
            ResetPlayerIntent(ois.player1Exposer.playerIntent);
            ResetPlayerIntent(ois.player2Exposer.playerIntent);
        }

        private void Update()
        {
            CheckPlayerIntent(ois.player1Exposer);
            CheckPlayerIntent(ois.player2Exposer);
        }
        */
/*
        private void ResetPlayerIntent(PlayerIntent playerIntent)
        {
            playerIntent.wantToMoveBack = false;
            playerIntent.wantToMoveForward = false;
            playerIntent.wantToMoveRight = false;
            playerIntent.wantToMoveLeft = false;

            playerIntent.wantToShootUp = false;
            playerIntent.wantToShootDown = false;
            playerIntent.wantToShootRight = false;
            playerIntent.wantToShootLeft = false;
         
            playerIntent.wantToBlock = false;
        }
        
        private void CheckPlayerIntent(PlayerExposer playerExposer)
        {
            PlayerIntent playerIntent = playerExposer.playerIntent;
            
            if (playerIntent.wantToMoveBack && playerExposer.playerTransform.position.z > -8)
            {
                playerExposer.playerTransform.Translate(0, 0, -1);
            }
            
            if (playerIntent.wantToMoveForward && playerExposer.playerTransform.position.z < 8)
            {
                playerExposer.playerTransform.Translate(0, 0, 1);
            }
            
            if (playerIntent.wantToMoveRight && playerExposer.playerTransform.position.x < 8)
            {
                playerExposer.playerTransform.Translate(1, 0, 0);
            }
            
            if (playerIntent.wantToMoveLeft && playerExposer.playerTransform.position.x > -8)
            {
                playerExposer.playerTransform.Translate(-1, 0, 0);
            }
        }*/
    }
}