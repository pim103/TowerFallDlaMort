using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Games.GameState;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = Unity.Mathematics.Random;

namespace Games.Agents
{
    public class RandomRolloutAgent : Agent
    {
        private Intent intent;

        public RandomRolloutAgent()
        {
            intent = new Intent();
        }
        
        [BurstCompile]
        struct RandomRolloutJob : IJob
        {
            public GameStateData gs;
            
            public RandomAgent rdmAgent;

            [WriteOnly] 
            public int id;

            public Intent intent;
            
            public void Execute()
            {
                var iterations = 5;

                int summedScore = 0;
                int indexI = 0;
                int indexJ = 0;
                
                var gsCopy = GameStateRules.Clone(ref gs);

                intent = new Intent();
                
                for (int i = 0; i <= (int) ActionsAvailable.NONE; i++)
                {
                    intent.moveIntent = (ActionsAvailable) i;
                    for (int j = (int) ActionsAvailable.NONE; j <= (int) ActionsAvailable.BLOCK; j++)
                    {
                        intent.moveIntent = (ActionsAvailable) j;
                        for (int k = 0; k < iterations; k++)
                        {
                            GameStateRules.CopyTo(ref gs, ref gsCopy);

                            GameStateRules.Step(ref gsCopy, intent, id);

                            var nbLoop = 500;
                            while (nbLoop != 0)
                            {
                                GameStateRules.Step(ref gsCopy, rdmAgent.Act(ref gsCopy, id), id);
                                nbLoop--;
                            }

                            if (gsCopy.players[id].PlayerScore > summedScore)
                            {
                                //Debug.Log("i : " + i);
                                //Debug.Log("j : " + j);
                                //Debug.Log("PlayerScore : " + gsCopy.players[id].PlayerScore);
                                //Debug.Log("sum : " + j);
                                summedScore = gsCopy.players[id].PlayerScore;
                                indexI = i;
                                indexJ = j;
                            }

                            //gsCopy.players.Dispose();
                            //gsCopy.projectiles.Dispose();
                        }
                    }
                }

                intent.moveIntent = (ActionsAvailable) indexI;
                intent.actionIntent = (ActionsAvailable) indexJ;
            }
        }

        public Intent Act(ref GameStateData gs, int id)
        {
            var job = new RandomRolloutJob
            {
                gs = gs,
                rdmAgent = new RandomAgent
                {
                    rdm = new Random((uint) Time.frameCount)
                }
            };
            var handle = job.Schedule();
            handle.Complete();

            Intent intent = job.intent;

            return intent;
        }
    }
}
