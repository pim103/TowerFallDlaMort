using System;
using System.Collections.Generic;
using Games.GameState;
using Unity.Collections;


namespace Games.Agents
{
    public class AStarAgent : Agent
    {
        private Intent intent;
        private List<AStar.AStar>_aStars = new List<AStar.AStar>();

        public AStarAgent()
        {
            intent = new Intent();
        }
        
        public Intent Act(ref GameStateData gs, int id)
        {
            return intent;
        }
    }
}