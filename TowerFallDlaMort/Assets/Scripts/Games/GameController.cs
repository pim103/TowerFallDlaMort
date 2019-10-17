using System;
using Scripts.Menus;
using System.Collections;
using System.Collections.Generic;
using Games;
using Games.Agents;
using Games.GameState;
using Scripts.Items;
using Scripts.Players;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        private GameObject projectilePrefab;
        [SerializeField]
        private GameObject obstaclePrefab;

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
        private Text timerText;

        [SerializeField]
        private ObjectsInScene ois;

        private Coroutine timer;

        public bool gameIsStart = false;

        private GameStateData gs;
    
        private List<Transform> players = new List<Transform>();
        
        private List<GameObject> items = new List<GameObject>();
        private List<GameObject> projectiles = new List<GameObject>();
        private List<GameObject> obstacles = new List<GameObject>();
        
        private Agent agent1;
        private Agent agent2;
        private int nbProjectile = 0;

        public int winnerId;

        //private bool _hasTwoPlayer = false;

        private void Start()
        {
            humanAgentBtn[0].onClick.AddListener(delegate
            {
                agent1 = ois.player1Exposer.playerAgent;
                ois.player1Exposer.playerAgent.players[0] = true;
                ois.player2Exposer.playerAgent.players[0] = true;
                ButtonSelector(0, 0);
            });
            humanAgentBtn[1].onClick.AddListener(delegate
            {
                agent2 = ois.player2Exposer.playerAgent; 
                ois.player1Exposer.playerAgent.players[1] = true;
                ois.player2Exposer.playerAgent.players[1] = true;
                ButtonSelector(0, 1);
            });
            randomAgentBtn[0].onClick.AddListener(delegate { agent1 = new RandomAgent
                {
                    rdm = new Unity.Mathematics.Random((uint) Time.frameCount)
                };
                ois.player1Exposer.playerAgent.players[0] = false;
                ois.player2Exposer.playerAgent.players[0] = false;
                ButtonSelector(1, 0);
            });
            randomAgentBtn[1].onClick.AddListener(delegate { agent2 = new RandomAgent
                {
                    rdm = new Unity.Mathematics.Random((uint) Time.time)
                };
                ois.player1Exposer.playerAgent.players[1] = false;
                ois.player2Exposer.playerAgent.players[1] = false;
                ButtonSelector(1, 1);
            });
            randomRolloutAgentBtn[0].onClick.AddListener(delegate
            {
                agent1 = new RandomRolloutAgent();
                ois.player1Exposer.playerAgent.players[0] = false;
                ois.player2Exposer.playerAgent.players[0] = false;
                ButtonSelector(2, 0);
            });
            randomRolloutAgentBtn[1].onClick.AddListener(delegate
            {
                agent2 = new RandomRolloutAgent();
                ois.player1Exposer.playerAgent.players[1] = false;
                ois.player2Exposer.playerAgent.players[1] = false;
                ButtonSelector(2, 1);
            });
            mctsAgentBtn[0].onClick.AddListener(delegate
            {
                agent1 = new MCTSAgent();
                ois.player1Exposer.playerAgent.players[0] = false;
                ois.player2Exposer.playerAgent.players[0] = false;
                ButtonSelector(3, 0);
            });
            mctsAgentBtn[1].onClick.AddListener(delegate
            {
                agent2 = new MCTSAgent();
                ois.player1Exposer.playerAgent.players[1] = false;
                ois.player2Exposer.playerAgent.players[1] = false;
                ButtonSelector(3, 1);
            });
            aStarAgentBtn[0].onClick.AddListener(delegate
            {
                agent1 = new AStarAgent();
                ois.player1Exposer.playerAgent.players[0] = false;
                ois.player2Exposer.playerAgent.players[0] = false;
                ButtonSelector(4, 0);
            });
            aStarAgentBtn[1].onClick.AddListener(delegate
            {
                agent2 = new AStarAgent();
                ois.player1Exposer.playerAgent.players[1] = false;
                ois.player2Exposer.playerAgent.players[1] = false;
                ButtonSelector(4, 1);
            });
            qLearningAgentBtn[0].interactable = false;
            qLearningAgentBtn[1].interactable = false;
            
            InitDefaultSelection();
        }

        private void InitDefaultSelection()
        {
            agent1 = ois.player1Exposer.playerAgent;
            ois.player1Exposer.playerAgent.players[0] = true;
            ois.player2Exposer.playerAgent.players[0] = true;
            ButtonSelector(0, 0);
            
            agent2 = ois.player2Exposer.playerAgent; 
            ois.player1Exposer.playerAgent.players[1] = true;
            ois.player2Exposer.playerAgent.players[1] = true;
            ButtonSelector(0, 1);
        }

        private void ButtonSelector(int id, int idList)
        {
            humanAgentBtn[idList].image.color = Color.white;
            randomAgentBtn[idList].image.color = Color.white;
            randomRolloutAgentBtn[idList].image.color = Color.white;
            mctsAgentBtn[idList].image.color = Color.white;
            aStarAgentBtn[idList].image.color = Color.white;
            qLearningAgentBtn[idList].image.color = Color.white;

            switch (id)
            {
                case 0:
                    humanAgentBtn[idList].image.color = Color.red;
                    break;
                case 1:
                    randomAgentBtn[idList].image.color = Color.red;
                    break;
                case 2:
                    randomRolloutAgentBtn[idList].image.color = Color.red;
                    break;
                case 3:
                    mctsAgentBtn[idList].image.color = Color.red;
                    break;
                case 4:
                    aStarAgentBtn[idList].image.color = Color.red;
                    break;
                case 5:
                    qLearningAgentBtn[idList].image.color = Color.red;
                    break;
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

            winnerId = -1;

            for (int i = 0; i < GameStateRules.MAX_ITEMS; i++)
            {
                items.Add(ois.itemExposer[i].ItemGameObject);
            }
                
            GameStateRules.Init(ref gs);
            SetItemsPosition();
            InitItemsType();
            PopObstacle();
        }

        public void PopObstacle()
        {
            for (int i = 0; i < GameStateRules.MAX_OBSTACLE * 4; i++)
            {
                obstacles.Add(Instantiate(obstaclePrefab, gs.obstacles[i].position, Quaternion.identity));
            }
            
            //Debug.Log("GameController, GS OBS Length " + gs.obstacles.Length);
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

            if (gs.EndOfGame)
            {
                if (gs.players[0].PlayerLifeStock > gs.players[1].PlayerLifeStock)
                {
                    winnerId = 0;
                }
                else if (gs.players[0].PlayerLifeStock < gs.players[1].PlayerLifeStock)
                {
                    winnerId = 1;
                }
                else
                {
                    winnerId = -1;
                }
                
                gameIsStart = false;
                menuController.ActiveMenu(Menus.Menus.END_GAME_MENU);

                return;
            }

            SyncNumbersOfProjectiles();
            SyncProjectilesView();
            SyncPlayersView();
            SyncItemsView();
            SyncTimerView();
            
            GameStateRules.Step(ref gs, agent1.Act(ref gs, 0), agent2.Act(ref gs, 1));
            GameStateRules.UpdateItems(ref gs);
        }

        public void SyncTimerView()
        {
            var time = gs.timer;
            timerText.text = (int)time + " secondes";
        }

        public void SyncNumbersOfProjectiles()
        {   
            if (projectiles.Count < gs.projectiles.Length)
            {
                for (int i = projectiles.Count; i < gs.projectiles.Length; i++)
                {
                    AddNewProjectile(gs.projectiles[i].ownerId, i);
                }
            }

            for (int i = 0; i < projectiles.Count - gs.projectiles.Length; i++)
            {
                Destroy(projectiles[projectiles.Count - 1]);
                projectiles.RemoveAt(projectiles.Count - 1);
            }
        }

        public void AddNewProjectile(int idPlayer, int idProj)
        {
            projectiles.Add(idPlayer == 2
                ? Instantiate(projectilePrefab,
                    new Vector3(gs.projectiles[idProj].position.x, 1, gs.projectiles[idProj].position.z),
                    Quaternion.identity)
                : Instantiate(projectilePrefab,
                    new Vector3(gs.players[idPlayer].playerPosition.x, gs.players[idPlayer].playerPosition.y,
                        gs.players[idPlayer].playerPosition.z), Quaternion.identity));
        }

        public void SyncProjectilesView()
        {
            for (int i = 0; i < gs.projectiles.Length; i++)
            {
                projectiles[i].transform.position = gs.projectiles[i].position;

                if (gs.projectiles[i].ownerId == 0)
                {
                    projectiles[i].GetComponent<Renderer>().material.color = Color.blue;
                }
                else if(gs.projectiles[i].ownerId == 1)
                {
                    projectiles[i].GetComponent<Renderer>().material.color = Color.red;
                }
                else
                {
                    projectiles[i].GetComponent<Renderer>().material.color = Color.yellow;
                }
            }
        }
        
        public void SyncPlayersView()
        {
            for(int i = 0; i < GameStateRules.MAX_PLAYERS; i++)
            {
                players[i].position = gs.players[i].playerPosition;
            }
        }

        public void SyncItemsView()
        {
            for (int i = 0; i < GameStateRules.MAX_ITEMS; i++)
            {
                items[i].transform.rotation = Quaternion.Euler(gs.items[i].rotation);
                if (gs.items[i].active == false)
                {
                    items[i].gameObject.SetActive(false);
                }
            }
        }

        public void SetItemsPosition()
        {
            for (int i = 0; i < GameStateRules.MAX_ITEMS; i++)
            {
                Vector3 currentPosition = items[i].transform.position;
                currentPosition.x = gs.items[i].position.x;
                currentPosition.z = gs.items[i].position.y;
                items[i].transform.position = currentPosition;
            }
        }

        public void InitItemsType()
        {
            for (int i = 0; i < GameStateRules.MAX_ITEMS; i++)
            {
                ItemExposer ie = items[i].GetComponent<ItemExposer>();
                var typeNumber = ie.ItemTypeNumber;
                var item = gs.items[i];
                item.type = typeNumber;
                gs.items[i] = item;
                Debug.Log("type d'item :" + gs.items[i].type);
            } 
        }
    }
}