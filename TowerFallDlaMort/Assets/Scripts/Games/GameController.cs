using Scripts.Menus;
using System.Collections;
using System.Collections.Generic;
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
            timer = StartCoroutine(CounterTimeLeft());
        }

        public void EndGame()
        {
            // TODO : Show menu with scores
            menuController.ReturnMenu();
        }
    }
}