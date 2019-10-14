using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Players
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerIntent player1Intent;
        public PlayerIntent player2Intent;
        public PlayerExposer player1Exposer;
        public PlayerExposer player2Exposer;

        private void Start()
        {
            resetPlayerIntent(player1Intent);
            resetPlayerIntent(player2Intent);
        }

        private void Update()
        {
            checkPlayerIntent(player1Intent, player1Exposer);
            checkPlayerIntent(player2Intent, player2Exposer);
        }

        private void resetPlayerIntent(PlayerIntent playerIntent)
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
        
        private void checkPlayerIntent(PlayerIntent playerIntent, PlayerExposer playerExposer)
        {
            if (playerIntent.wantToMoveBack == true && playerExposer.playerTransform.position.z > -8)
            {
                playerExposer.playerTransform.Translate(0, 0, -1);
            }
            
            if (playerIntent.wantToMoveForward == true && playerExposer.playerTransform.position.z < 8)
            {
                playerExposer.playerTransform.Translate(0, 0, 1);
            }
            
            if (playerIntent.wantToMoveRight == true && playerExposer.playerTransform.position.x > -8)
            {
                playerExposer.playerTransform.Translate(-1, 0, 0);
            }
            
            if (playerIntent.wantToMoveLeft == true && playerExposer.playerTransform.position.x < 8)
            {
                playerExposer.playerTransform.Translate(1, 0, 0);
            }
        }
    }
}