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
        [SerializeField]
        private ControlsAndroid controls;

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
            //#if UNITY_ANDROID
            if (!hasTwoPlayer)
            {
                //Debug.Log(controls.shootFoward[id]);
                if (controls.shootFoward[id])
                {
                    intent.actionIntent = ActionsAvailable.SHOT_FORWARD;
                }
                else if (controls.shootBack[id])
                {
                    intent.actionIntent = ActionsAvailable.SHOT_BACK;
                }
                else if (controls.shootLeft[id])
                {
                    intent.actionIntent = ActionsAvailable.SHOT_LEFT;
                }
                else if (controls.shootRight[id])
                {
                    intent.actionIntent = ActionsAvailable.SHOT_RIGHT;
                }
                
                if (controls.moveFoward[id])
                {
                    if (controls.moveLeft[id])
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_FORWARD_RIGHT;
                    }
                    else if (controls.moveRight[id])
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_FORWARD_LEFT;
                    }
                    else
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_FORWARD;
                    }
                }
                else if (controls.moveBack[id])
                {
                    if (controls.moveRight[id])
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_BACK_RIGHT;
                    }
                    else if (controls.moveLeft[id])
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_BACK_LEFT;
                    }
                    else
                    {
                        intent.moveIntent = ActionsAvailable.MOVE_BACK;
                    }
                }
                else if (controls.moveLeft[id])
                {
                    intent.moveIntent = ActionsAvailable.MOVE_LEFT;
                }
                else if (controls.moveRight[id])
                {
                    intent.moveIntent = ActionsAvailable.MOVE_RIGHT;
                }
            }
            
            if (!hasTwoPlayer)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    intent.actionIntent = ActionsAvailable.SHOT_FORWARD;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    intent.actionIntent = ActionsAvailable.SHOT_BACK;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    intent.actionIntent = ActionsAvailable.SHOT_LEFT;
                }
                else if (Input.GetKey(KeyCode.RightArrow))
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

                    if (Input.GetKey(KeyCode.Y))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_FORWARD;
                    }
                    if (Input.GetKey(KeyCode.H))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_BACK;
                    }
                    if (Input.GetKey(KeyCode.G))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_LEFT;
                    }
                    if (Input.GetKey(KeyCode.J))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_RIGHT;
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

                    if (Input.GetKey(KeyCode.Keypad8))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_FORWARD;
                    }
                    if (Input.GetKey(KeyCode.Keypad5))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_BACK;
                    }
                    if (Input.GetKey(KeyCode.Keypad4))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_LEFT;
                    }
                    if (Input.GetKey(KeyCode.Keypad6))
                    {
                        intent.actionIntent = ActionsAvailable.SHOT_RIGHT;
                    }
                }
            }
            //#endif
            

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