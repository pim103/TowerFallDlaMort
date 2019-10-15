using System.Collections.Generic;
using Games.GameState;
using Unity.Collections;
using UnityEngine;

namespace Games.Agents
{
    public class RandomRolloutAgent : Agent
    {
        private List<ActionsAvailable> actions = new List<ActionsAvailable>();

        public List<ActionsAvailable> Act(ref GameStateData gs)
        {
            var agent = new RandomAgent();
            var iterations = 5;

            int summedScore = 0;
            int indexI = 0;
            int indexJ = 0;

            for (int i = 0; i <= (int) ActionsAvailable.NONE; i++)
            {
                for (int j = (int) ActionsAvailable.NONE; j <= (int) ActionsAvailable.BLOCK; j++)
                {
                    for (int k = 0; k < iterations; k++)
                    {
                        var gsCopy = GameStateRules.Clone(gs);
                        //Debug.Log("ScorePlayer : " + gsCopy.players[0].PlayerScore);
                        actions.Clear();
                        
                        actions.Add((ActionsAvailable) i);
                        actions.Add((ActionsAvailable) j);

                        GameStateRules.Step(ref gsCopy, actions);

                        var nbLoop = 20;
                        while (nbLoop != 0)
                        {
                            GameStateRules.Step(ref gsCopy, agent.Act(ref gsCopy));
                            nbLoop--;
                        }

                        if (gsCopy.players[1].PlayerScore > summedScore)
                        {
                            summedScore = gsCopy.players[1].PlayerScore;
                            indexI = i;
                            indexJ = j;
                        }

                        gsCopy.players.Dispose();
                        gsCopy.projectiles.Dispose();
                    }
                }
            }
            
            actions.Clear();
            actions.Add((ActionsAvailable) indexI);
            actions.Add((ActionsAvailable) indexJ);
            
            return actions;
        }
    }
}
