using System.Collections.Generic;
using UnityEngine;
using Games.GameState;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Games.Agents
{
    public struct Node
    {
        public int moveIntent;
        public int actionIntent;
        public int nc;
        public long rc;
        public int npc;
    }

    public struct SelectedNodeInfo
    {
        public long hash;
        public int nodeIndex;
    }
    
    public class MCTSAgent : Agent
    {
        [BurstCompile]
        struct MCTSAgentJob : IJob
        {
            public GameStateData gs;

            public RandomAgent rdmAgent;

            [WriteOnly]
            public NativeArray<int> sumScore;

            public int id;
            public void Execute()
            {
                //Debug.Log("Init");
                var iterations = 100;
                var agent = rdmAgent;
                var gsCopy = GameStateRules.Clone(ref gs);
                var rootHash = GameStateRules.GetHashCode(ref gsCopy);
                //Debug.Log("EndInit");
                var memory = new NativeHashMap<long, NativeList<Node>>(10000000, Allocator.Temp);
                
                //Debug.Log("EndInit2");
                memory.TryAdd(rootHash, new NativeList<Node>(54, Allocator.Temp));
                
                
                for (var i = 0; i < (int)ActionsAvailable.NONE+1; i++)
                {
                    for (var j = (int) ActionsAvailable.NONE; j < (int) ActionsAvailable.BLOCK + 1; j++)
                    {
                        memory[rootHash]
                            .Add(new Node
                            {
                                moveIntent = i,
                                actionIntent = j,
                                nc = 0,
                                npc = 0,
                                rc = 0
                            });
                    }
                }
                for (int i = 0; i < memory[rootHash].Length; i++)
                {
                    sumScore[i] = 0;
                }
                
                for (var n = 0; n < iterations; n++)
                {
                    GameStateRules.CopyTo(ref gs, ref gsCopy);
                    var currentHash = rootHash;
                    var selectedNodes = new NativeList<SelectedNodeInfo>(Allocator.Temp);

                    var currentTime = gsCopy.currentGameStep;
                    //var countSelect = 0;
                    //SELECT
                    while (!gsCopy.EndOfGame && gsCopy.currentGameStep - 100 <= currentTime)
                    {
                        //countSelect++;
                        var hasUnexploredNodes = false;
                        
                        for (var i = 0; i < memory[currentHash].Length; i++)
                        {
                            if (memory[currentHash][i].nc == 0)
                            {
                                hasUnexploredNodes = true;
                                break;
                            }
                        }
                        
                        if (hasUnexploredNodes)
                        {
                            break;
                        }
                        //sumScore[1] = gsCopy.currentGameStep;
                        //sumScore[2] = currentTime;
                        
                        var bestNodeIndex = -1;
                        var bestNodeScore = float.MinValue;

                        for (var i = 0; i < memory[currentHash].Length; i++)
                        {
                            var list = memory[currentHash];
                            var node = list[i];
                            node.npc += 1;
                            list[i] = node;
                            memory[currentHash] = list;
                            

                            var score = (float) memory[currentHash][i].rc / memory[currentHash][i].nc
                                        + math.sqrt(2 * math.log(memory[currentHash][i].npc) / memory[currentHash][i].nc);

                            if (score >= bestNodeScore)
                            {
                                bestNodeIndex = i;
                                bestNodeScore = score;
                            }
                        }
                        
                        selectedNodes.Add(new SelectedNodeInfo
                        {
                            hash = currentHash,
                            nodeIndex = bestNodeIndex
                        });
                        Intent currentIntent = new Intent();
                        currentIntent.moveIntent = (ActionsAvailable)memory[currentHash][bestNodeIndex].moveIntent;
                        currentIntent.actionIntent = (ActionsAvailable)memory[currentHash][bestNodeIndex].actionIntent;
                        GameStateRules.Step(ref gsCopy,currentIntent,id);
                        currentHash = GameStateRules.GetHashCode(ref gsCopy);
                        
                        if (!memory.ContainsKey(currentHash))
                        {
                            memory.TryAdd(currentHash, new NativeList<Node>(54, Allocator.Temp));
                
                            for (var i = 0; i < (int)ActionsAvailable.NONE+1; i++)
                            {
                                for (var j = (int) ActionsAvailable.NONE; j < (int) ActionsAvailable.BLOCK + 1; j++)
                                {
                                    memory[currentHash]
                                        .Add(new Node
                                        {
                                            moveIntent = i,
                                            actionIntent = j,
                                            nc = 0,
                                            npc = 0,
                                            rc = 0
                                        });
                                }
                            }
                        }
                    }

                    //sumScore[0] = countSelect;
                    //EXPAND
                    if (!gsCopy.EndOfGame)
                    {
                        var unexploredActions = new NativeList<int>(Allocator.Temp);

                        for (var i = 0; i < memory[currentHash].Length; i++)
                        {
                            if (memory[currentHash][i].nc == 0)
                            {
                                unexploredActions.Add(i);
                                //sumScore[i] = i;
                            }
                        }
                        
                        
                        var chosenNodeIndex = agent.rdm.NextInt(0, unexploredActions.Length);
                        
                        selectedNodes.Add(new SelectedNodeInfo
                        {
                            hash = currentHash,
                            nodeIndex = unexploredActions[chosenNodeIndex]
                        });
                        
                        Intent currentIntent = new Intent();
                        currentIntent.moveIntent = (ActionsAvailable)memory[currentHash][unexploredActions[chosenNodeIndex]].moveIntent;
                        currentIntent.actionIntent = (ActionsAvailable)memory[currentHash][unexploredActions[chosenNodeIndex]].actionIntent;
                        /*sumScore[0] = (int)currentIntent.moveIntent;
                        sumScore[1] = (int)currentIntent.actionIntent;*/
                        GameStateRules.Step(ref gsCopy, currentIntent, id);
                        
                        currentHash = GameStateRules.GetHashCode(ref gsCopy);
                        
                        if (!memory.ContainsKey(currentHash))
                        {
                            memory.TryAdd(currentHash, new NativeList<Node>(54, Allocator.Temp));
                
                            for (var i = 0; i < (int)ActionsAvailable.NONE+1; i++)
                            {
                                for (var j = (int) ActionsAvailable.NONE; j < (int) ActionsAvailable.BLOCK + 1; j++)
                                {
                                    memory[currentHash]
                                        .Add(new Node
                                        {
                                            moveIntent = i,
                                            actionIntent = j,
                                            nc = 0,
                                            npc = 0,
                                            rc = 0
                                        });
                                }
                            }
                        }
                    }

                    
                    //SIMULATE
                    currentTime = gsCopy.currentGameStep;
                    Intent currentIntent1 = new Intent();
                    currentIntent1.moveIntent = ActionsAvailable.NONE;
                    currentIntent1.actionIntent = ActionsAvailable.NONE;
                    while (!gsCopy.EndOfGame && gsCopy.currentGameStep - 100 <= currentTime)
                    {
                        var chosenActionIndex = agent.rdm.NextInt(0, 9);
                        var chosenActionIndex1 = agent.rdm.NextInt(9, 13);
                        

                        //sumScore[0] = chosenActionIndex;

                        currentIntent1.moveIntent =
                                (ActionsAvailable) chosenActionIndex;
                        currentIntent1.actionIntent =
                                (ActionsAvailable) chosenActionIndex1;

                        GameStateRules.Step(ref gsCopy, currentIntent1, id);
                    }
                    
                    //BACKPROPAGATE
                    //sumScore[0] = selectedNodes.Length;
                    for (var i = 0; i < selectedNodes.Length; i++)
                    {
                        var list = memory[selectedNodes[i].hash];
                        var node = list[selectedNodes[i].nodeIndex];
                        
                        node.rc += gsCopy.players[id].PlayerScore;
                        node.nc += 1;
                        
                        list[selectedNodes[i].nodeIndex] = node;

                        memory[selectedNodes[i].hash] = list;
                    }
                }
                //sumScore[0] = memory[rootHash].Length;
                
                for (var i = 0; i < memory[rootHash].Length; i++)
                {
                    sumScore[i] = memory[rootHash][i].nc;
                }
            }
        }

        public Intent Act(ref GameStateData gs, int id)
        {
            var job = new MCTSAgentJob
            {
                gs = gs,
                sumScore = new NativeArray<int>(54, Allocator.TempJob),
                rdmAgent = new RandomAgent {rdm = new Random((uint) Time.frameCount)},
                id = id
            };
            var handle = job.Schedule();
            handle.Complete();

            var bestActionIndex = -1;
            var bestScore = long.MinValue;
            for (var i = 0; i < job.sumScore.Length; i++)
            {
                if (bestScore > job.sumScore[i])
                {
                    continue;
                }

                bestScore = job.sumScore[i];
                bestActionIndex = i;
            }
            //var chosenAction = availableActions[bestActionIndex];
            /*for (int i = 0; i < job.sumScore.Length; i++)
            {
                Debug.Log(i+" "+job.sumScore[i]);
            }*/
            //Debug.Log(job.sumScore[0]);
            /*Debug.Log(job.sumScore[1]);
            Debug.Log(job.sumScore[2]);*/
            Intent chosenIntent = new Intent();
            chosenIntent.moveIntent = (ActionsAvailable)(bestActionIndex/(int) ActionsAvailable.NONE);
            chosenIntent.actionIntent = (ActionsAvailable)((bestActionIndex%(int) ActionsAvailable.MOVE_BACK_LEFT)+(int) ActionsAvailable.MOVE_BACK_LEFT);
            job.sumScore.Dispose();
            Debug.Log((int) ActionsAvailable.MOVE_BACK_LEFT);
            Debug.Log(bestActionIndex);
            Debug.Log((int)chosenIntent.moveIntent);
            Debug.Log(chosenIntent.moveIntent);
            Debug.Log((int)chosenIntent.actionIntent);
            Debug.Log(chosenIntent.actionIntent);
            return chosenIntent;
        }
    }
}
