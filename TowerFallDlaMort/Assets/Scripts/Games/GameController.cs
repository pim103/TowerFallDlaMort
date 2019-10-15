using System;
using Scripts.Menus;
using System.Collections;
using System.Collections.Generic;
using Games;
using Games.Agents;
using Games.GameState;
using Scripts.Players;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts.Games
{
    public class GameController : MonoBehaviour
    {
        // Time left before the end
        // Value in seconds
        private const int GAME_TIME = 500;

        [SerializeField]
        private MenuController menuController;

        [SerializeField] 
        private Button[] humanAgentBtn;
        [SerializeField] 
        private Button[] randomAgentBtn;
        [SerializeField] 
        private Button[] randomRolloutAgentBtn;
        [SerializeField] 
        private Button[] mctsAgentBtn;
        [SerializeField] 
        private Button[] aStarAgentBtn;
        [SerializeField] 
        private Button[] qLearningAgentBtn;

        [SerializeField]
        private ObjectsInScene ois;

        private Coroutine timer;

        public bool gameIsStart = false;

        private GameStateData gs;

        private List<Transform> players = new List<Transform>();

        private Agent agent1;
        private Agent agent2;

        private void Start()
        {
            humanAgentBtn[0].onClick.AddListener(delegate { agent1 = ois.player1Exposer.playerAgent; });
            humanAgentBtn[1].onClick.AddListener(delegate { agent2 = ois.player2Exposer.playerAgent; });

            for (int i = 0; i < 2; i++)
            {
                randomAgentBtn[i].onClick.AddListener(delegate
                {
                    switch (i)
                    {
                        case 0:
                            agent1 = new RandomAgent();
                            break;
                        case 1:
                            agent2 = new RandomAgent();
                            break;
                    }
                });
                randomRolloutAgentBtn[i].onClick.AddListener(delegate
                {
                    switch (i)
                    {
                        case 0:
                            agent1 = new RandomRolloutAgent();
                            break;
                        case 1:
                            agent2 = new RandomRolloutAgent();
                            break;
                    }
                });
            }
        }

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
            players.Add(ois.player1Exposer.playerTransform);
            players.Add(ois.player2Exposer.playerTransform);
            
            
            //agent1 = new RandomAgent();
            agent2 = new RandomRolloutAgent();
            
            GameStateRules.Init(ref gs);
        }

        public void EndGame()
        {
            // TODO : Show menu with scores
            gameIsStart = false;
            menuController.ReturnMenu();

            gs.players.Dispose();
        }

        public void Update()
        {
            if (!gameIsStart)
            {
                return;
            }

            //PlayerController.SyncPlayersView();
            SyncProjectilesView();
            SyncPlayersView();
            
            GameStateRules.Step(ref gs, agent1.Act(ref gs, 0), agent2.Act(ref gs, 1));
        }

        public void SyncProjectilesView()
        {
            
        }
        
        public void SyncPlayersView()
        {
            for(int i = 0; i < GameStateRules.MAX_PLAYERS; i++)
            {
                players[i].position = gs.players[i].playerPosition;
            }
        }
    }
}