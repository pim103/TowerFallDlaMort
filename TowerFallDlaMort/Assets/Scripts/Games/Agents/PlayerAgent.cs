using System;
using System.Collections;
using System.Collections.Generic;
using Games;
using Games.Agents;
using Games.GameState;
using UnityEngine;

namespace Scripts.Players
{
    public class PlayerAgent : MonoBehaviour, Agent
    {
        [SerializeField] 
        private int PlayerId;

        private Intent intent;

        private void Start()
        {
            intent = new Intent();
        }

        public Intent Act(ref GameStateData gs, int id)
        {
            intent.moveIntent = ActionsAvailable.NONE;
            intent.actionIntent = ActionsAvailable.NONE;
            
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
                if (Input.GetKey(KeyCode.UpArrow))
                {
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
                    intent.moveIntent = ActionsAvailable.MOVE_RIGHT;
                }
            }
            
            return intent;
        }
    }
}