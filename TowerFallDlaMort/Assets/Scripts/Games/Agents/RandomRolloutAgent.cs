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

            public NativeArray<ActionsAvailable> intent;

            public Intent getIntent;
            
            public void Execute()
            {
                var iterations = 5;

                int indexI = (int) ActionsAvailable.NONE;
                int indexJ = (int) ActionsAvailable.NONE;
                
                var gsCopy = GameStateRules.Clone(ref gs);
                int summedScore = gsCopy.players[id].PlayerScore;
                
                for (int i = 0; i <= (int) ActionsAvailable.NONE; i++)
                {
                    getIntent.moveIntent = (ActionsAvailable) i;
                    for (int j = (int) ActionsAvailable.NONE; j <= (int) ActionsAvailable.BLOCK; j++)
                    {
                        getIntent.actionIntent = (ActionsAvailable) j;
                        for (int k = 0; k < iterations; k++)
                        {
                            GameStateRules.CopyTo(ref gs, ref gsCopy);
                            
                            GameStateRules.Step(ref gsCopy, getIntent, id);

                            var nbLoop = 500;
                            while (nbLoop != 0)
                            {
                                GameStateRules.Step(ref gsCopy, rdmAgent.Act(ref gsCopy, id), id);
                                nbLoop--;
                            }

                            if (gsCopy.players[id].PlayerScore > summedScore)
                            {
                                summedScore = gsCopy.players[id].PlayerScore;
                                indexI = i;
                                indexJ = j;
                            }

                            //gsCopy.players.Dispose();
                            //gsCopy.projectiles.Dispose();
                        }
                    }
                }

                intent[0] = (ActionsAvailable) indexI;
                intent[1] = (ActionsAvailable) indexJ;
            }
        }

        public Intent Act(ref GameStateData gs, int id)
        {
            var job = new RandomRolloutJob
            {
                gs = gs,
                intent = new NativeArray<ActionsAvailable>(2, Allocator.TempJob),
                rdmAgent = new RandomAgent
                {
                    rdm = new Random((uint) Time.frameCount)
                },
                id = id
            };
            var handle = job.Schedule();
            handle.Complete();

            intent.moveIntent = job.intent[0];
            intent.actionIntent = job.intent[1];

            job.intent.Dispose();
            return intent;
        }
    }
}
