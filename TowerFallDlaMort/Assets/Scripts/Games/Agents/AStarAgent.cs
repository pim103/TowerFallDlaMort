using System;
using System.Collections.Generic;
using Games.GameState;
using Unity.Collections;


namespace Games.Agents
{
    public class AStarAgent : Agent
    {
        private List<ActionsAvailable> actions = new List<ActionsAvailable>();
        private List<AStar.AStar> _aStars = new List<AStar.AStar>();
        public List<ActionsAvailable> Act(ref GameStateData gs, int id)
        {
            actions.Clear();

            return actions;
        }
    }
}