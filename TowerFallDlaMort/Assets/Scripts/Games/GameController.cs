using System;
using Scripts.Menus;
using System.Collections;
using System.Collections.Generic;
using Scripts.Players;
using UnityEngine;

namespace Scripts.Games
{
    public class GameController : MonoBehaviour
    {
        // Time left before the end
        // Value in seconds
        private const int GAME_TIME = 500;

        [SerializeField]
        private MenuController menuController;

        private Coroutine timer;

        public bool gameIsStart = false;

        private IEnumerator CounterTimeLeft()
        {
            int timeLeft = GAME_TIME;

            while(timeLeft > 0)
            {
                yield return new WaitForSeconds(1);
                timeLeft--;
            }

            EndGame();
        }

        public void InitStartGame()
        {
            // TODO : reset position of players and to reset all values of intent
            gameIsStart = true;
            timer = StartCoroutine(CounterTimeLeft());
        }

        public void EndGame()
        {
            // TODO : Show menu with scores
            gameIsStart = false;
            menuController.ReturnMenu();
        }

        public void Update()
        {
            if (!gameIsStart)
            {
                return;
            }

            //PlayerController.SyncPlayersView();
            SyncProjectilesView();
        }

        public void SyncProjectilesView()
        {
            
        }
    }
}