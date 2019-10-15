using System;
using System.Collections;
using System.Collections.Generic;
using Games;
using Games.Agents;
using Games.GameState;
using Scripts.Games;
using UnityEngine;

namespace Scripts.Players
{
    public class PlayerAgent : MonoBehaviour, Agent
    {
        [SerializeField] 
        private int PlayerId;

        private Intent intent;
        public bool[] players;

        private bool hasTwoPlayer;

        private void Start()
        {
            intent = new Intent();
        }

        public Intent Act(ref GameStateData gs, int id)
        {
            intent.moveIntent = ActionsAvailable.NONE;
            intent.actionIntent = ActionsAvailable.NONE;
            hasTwoPlayer = HasTwoPlayer(players);


            if (!hasTwoPlayer)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    intent.actionIntent = ActionsAvailable.SHOT_FORWARD;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    intent.actionIntent = ActionsAvailable.SHOT_BACK;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    intent.actionIntent = ActionsAvailable.SHOT_LEFT;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    intent.actionIntent = ActionsAvailable.SHOT_RIGHT;
                }
                
                if (Input.GetKey(KeyCode.Z))
                {
                    if (Input.GetKey(KeyCode.D))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_FORWARD_RIGHT;
                    }
                    else if (Input.GetKey(KeyCode.Q))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_FORWARD_LEFT;
                    }
                    else
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_FORWARD;
                    }
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    if (Input.GetKey(KeyCode.D))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_BACK_RIGHT;
                    }
                    else if (Input.GetKey(KeyCode.Q))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_BACK_LEFT;
                    }
                    else
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_BACK;
                    }
                }
                else if (Input.GetKey(KeyCode.Q))
                {
                    intent.moveIntent = ActionsAvailable.MOVE_LEFT;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    intent.moveIntent = ActionsAvailable.MOVE_RIGHT;
                }
            }
            else
            {
                if (PlayerId == 0)
                {
                    if (Input.GetKey(KeyCode.Z))
                    {
                        if (Input.GetKey(KeyCode.D))
                        {
                            intent.moveIntent = ActionsAvailable.MOVE_FORWARD_RIGHT;
                        }
                        else if (Input.GetKey(KeyCode.Q))
                        {
                            intent.moveIntent = ActionsAvailable.MOVE_FORWARD_LEFT;
                        }
                        else
                        {
                            intent.moveIntent = ActionsAvailable.MOVE_FORWARD;
                        }
                    }
                    else if (Input.GetKey(KeyCode.S))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_FORWARD_LEFT;
                        if (Input.GetKey(KeyCode.D))
                        {
                            intent.moveIntent = ActionsAvailable.MOVE_BACK_RIGHT;
                        }
                        else if (Input.GetKey(KeyCode.Q))
                        {
                            intent.moveIntent = ActionsAvailable.MOVE_BACK_LEFT;
                        }
                        else
                        {
                            intent.moveIntent = ActionsAvailable.MOVE_BACK;
                        }
                    }
                    else if (Input.GetKey(KeyCode.Q))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_LEFT;
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_RIGHT;
                    }
                    if (Input.GetKeyDown(KeyCode.Y))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_FORWARD;
                    }
                    if (Input.GetKeyDown(KeyCode.H))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_BACK;
                    }
                    if (Input.GetKeyDown(KeyCode.G))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_LEFT;
                    }
                    if (Input.GetKeyDown(KeyCode.J))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_RIGHT;
                    }
                }
                else
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_BACK_RIGHT;
                    }
                    else if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_BACK_LEFT;
                        if (Input.GetKey(KeyCode.RightArrow))
                        {
                            intent.moveIntent = ActionsAvailable.MOVE_FORWARD_RIGHT;
                        }
                        else if (Input.GetKey(KeyCode.LeftArrow))
                        {
                            intent.moveIntent = ActionsAvailable.MOVE_FORWARD_LEFT;
                        }
                        else
                        {
                            intent.moveIntent = ActionsAvailable.MOVE_FORWARD;
                        }
                    }
                    else if (Input.GetKey(KeyCode.DownArrow))
                    {
                        if (Input.GetKey(KeyCode.RightArrow))
                        {
                            intent.moveIntent = ActionsAvailable.MOVE_BACK_RIGHT;
                        }
                        else if (Input.GetKey(KeyCode.LeftArrow))
                        {
                            intent.moveIntent = ActionsAvailable.MOVE_BACK_LEFT;
                        }
                        else
                        {
                            intent.moveIntent = ActionsAvailable.MOVE_BACK;
                        }
                    }
                    else if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_LEFT;
                    }
                    else if (Input.GetKey(KeyCode.RightArrow))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_BACK;
                    }
                    
                    else if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_LEFT;
                    }
                    else if (Input.GetKey(KeyCode.RightArrow))
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_RIGHT; 
                    }
                    if (Input.GetKeyDown(KeyCode.Keypad8))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_FORWARD;
                    }
                    if (Input.GetKeyDown(KeyCode.Keypad5))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_BACK;
                    }
                    if (Input.GetKeyDown(KeyCode.Keypad4))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_LEFT;
                    }
                    if (Input.GetKeyDown(KeyCode.Keypad6))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_RIGHT;
                    }
                }
            }
            Debug.Log("Intent = " + intent);
            return intent;
        }

        private bool HasTwoPlayer(bool[] players)
        {
            if (players[0] && players[1])
            {
                return true;
            }

            return false;
        }
    }
}