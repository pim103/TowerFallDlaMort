using System.Collections;
using System.Collections.Generic;
using Games.Agents;
using Games.GameState;
using UnityEngine;

namespace Scripts.Players
{
    public class PlayerAgent : MonoBehaviour, Agent
    {
        [SerializeField] 
        private int PlayerId;
        
        private List<ActionsAvailable> actions = new List<ActionsAvailable>();
        
        public List<ActionsAvailable> Act(ref GameStateData gs)
        {
            actions.Clear();

            if (PlayerId == 0)
            {
                if (Input.GetKey(KeyCode.Z))
                {
                    if (Input.GetKey(KeyCode.D))
                    {
                        actions.Add(ActionsAvailable.MOVE_FORWARD_RIGHT);
                    }
                    else if (Input.GetKey(KeyCode.Q))
                    {
                        actions.Add(ActionsAvailable.MOVE_FORWARD_LEFT);
                    }
                    else
                    {
                        actions.Add(ActionsAvailable.MOVE_FORWARD);
                    }
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    if (Input.GetKey(KeyCode.D))
                    {
                        actions.Add(ActionsAvailable.MOVE_BACK_RIGHT);
                    }
                    else if (Input.GetKey(KeyCode.Q))
                    {
                        actions.Add(ActionsAvailable.MOVE_BACK_LEFT);
                    }
                    else
                    {
                        actions.Add(ActionsAvailable.MOVE_BACK);
                    }
                }
                else if (Input.GetKey(KeyCode.Q))
                {
                    actions.Add(ActionsAvailable.MOVE_LEFT);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    actions.Add(ActionsAvailable.MOVE_RIGHT);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        actions.Add(ActionsAvailable.MOVE_FORWARD_RIGHT);
                    }
                    else if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        actions.Add(ActionsAvailable.MOVE_FORWARD_LEFT);
                    }
                    else
                    {
                        actions.Add(ActionsAvailable.MOVE_FORWARD);
                    }
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        actions.Add(ActionsAvailable.MOVE_BACK_RIGHT);
                    }
                    else if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        actions.Add(ActionsAvailable.MOVE_BACK_LEFT);
                    }
                    else
                    {
                        actions.Add(ActionsAvailable.MOVE_BACK);
                    }
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    actions.Add(ActionsAvailable.MOVE_LEFT);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    actions.Add(ActionsAvailable.MOVE_RIGHT);
                }
            }
            
            return actions;
        }
    }
}