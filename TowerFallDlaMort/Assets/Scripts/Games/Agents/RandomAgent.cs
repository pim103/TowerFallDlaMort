using System.Collections.Generic;
using Games.GameState;
using UnityEngine;

namespace Games.Agents
{
    public class RandomAgent : Agent
    {
        private List<ActionsAvailable> actions = new List<ActionsAvailable>();
        
        public List<ActionsAvailable> Act(ref GameStateData gs, int id)
        {
            actions.Clear();

            actions.Add((ActionsAvailable)Random.Range(0, (int) ActionsAvailable.NONE) );
            actions.Add((ActionsAvailable)Random.Range((int) ActionsAvailable.NONE, (int) ActionsAvailable.BLOCK) );

            return actions;
        }
    }
}
