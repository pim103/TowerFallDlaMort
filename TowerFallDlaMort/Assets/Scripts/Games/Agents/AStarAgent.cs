using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Games.GameState;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using Debug = UnityEngine.Debug;


namespace Games.Agents
{
    public class AStarAgent : Agent
    {
        private Intent intent;

        private static int ComputeH(int x, int z, int targetX, int targetZ)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetZ - z);
        }
        
        public class Location
        {
            public int x;
            public int z;
            public int f;
            public int g;
        }

        public struct AStartJob: IJob
        {
            public GameStateData gs;
            public int idPlayer;
            public int idTarget;
            
            public NativeArray<ActionsAvailable> intent;

            private int[] GetLowestScore(Location[][] openList)
            {
                int lowestScore = 1000000;
                int[] indexes = new int[2];
                
                for (int i = 0; i < GameStateRules.MAX_X * 2; i++)
                {
                    for (int j = 0; j < GameStateRules.MAX_Z * 2; j++)
                    {
                        if (lowestScore > openList[i][j].f && openList[i][j].f < 100)
                        {
                            lowestScore = openList[i][j].f;
                            indexes[0] = i;
                            indexes[1] = j;
                        }
                    }
                }

                return indexes;
            }

            private bool CheckProxProj(ref GameStateData gs, int targetX, int targetZ, int sourceX, int sourceZ, int idPlayer)
            {
                bool canGo = true;
                
                for (int i = 0; i < gs.projectiles.Length; i++)
                {
                    var projX = (int)gs.projectiles[i].position.x + 10;
                    var projZ = (int)gs.projectiles[i].position.z + 10;

                    if (targetX == projX && targetZ == projZ && sourceX != projX && sourceZ != projZ && gs.projectiles[i].ownerId != idPlayer)
                    {
                        canGo = false;
                    }
                }

                return canGo;
            }
            
            public void Execute()
            {
                var openList = new Location[20][];
                
                for(int i = 0; i < 20; i++)
                {
                    openList[i] = new Location[20];
                }
                
                var closedList = new List<Location>();
                /*
                var start = new Location
                {
                    x = (int)gs.players[idPlayer].playerPosition.x,
                    z = (int)gs.players[idPlayer].playerPosition.z,
                    f = 0
                };
                var availableLocations = GetAvailableLocations(start.x, start.z, ref gs, idPlayer);
                */
                for (int i = 0; i < GameStateRules.MAX_X * 2; i++)
                {
                    for (int j = 0; j < GameStateRules.MAX_Z * 2; j++)
                    {
                        var score = 100;
                        int g = 0;
                        /*
                        if (j == 10 && (i != 0 || i != 1 || i != 2) )
                        {
                            score = 222222;
                            g = 100;
                        }*/

                        if (i == (int) gs.players[idPlayer].playerPosition.x + 10 &&
                            j == (int) gs.players[idPlayer].playerPosition.z + 10)
                        {
                            score = 0;
                        }

                        openList[i][j] = new Location
                        {
                            f = score,
                            g = g,
                            x = i,
                            z = j
                        };
                    }
                }

                int targetX = (int) gs.players[idTarget].playerPosition.x + 10;
                int targetZ = (int) gs.players[idTarget].playerPosition.z + 10;

                int sourceX = (int) gs.players[idPlayer].playerPosition.x + 10;
                int sourceZ = (int) gs.players[idPlayer].playerPosition.z + 10;

                if (targetX >= 9 && targetX <= 10 && (targetZ >= 5 && targetZ <= 15) || (targetX >= 5 && targetX <= 15) && targetZ >= 9 && targetZ <= 10)
                {
                    return;
                }

                Location destination = openList[targetX][targetZ];
                Location Nmin = null;
                
                while (Nmin != destination)
                {
                    int[] ids = GetLowestScore(openList);
                    Nmin = openList[ids[0]][ids[1]];

                    if (CheckProxProj(ref gs, ids[0] + 1, ids[1] - 1, targetX, targetZ, idPlayer) && (ids[0] < 19 && ids[1] > 0) && openList[ids[0] + 1][ids[1] - 1].g == 0)
                    {
                        openList[ids[0] + 1][ids[1] - 1].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0] + 1][ids[1] - 1].f = openList[ids[0] + 1][ids[1] - 1].g + ComputeH(ids[0] + 1, ids[1] - 1, targetX, targetZ);
                        if (ids[0] + 1 <= 10 && ids[0] + 1 >= 9 && (ids[1] - 1 >= 5 && ids[1] - 1 <= 15) || (ids[0] + 1 >= 5 && ids[0] + 1 <= 15) && ids[1] - 1 >= 9 && ids[1] - 1 <= 10)
                        {
                            openList[ids[0] + 1][ids[1] - 1].f = 222222;
                        }
                    }

                    if (CheckProxProj(ref gs, ids[0] + 1, ids[1], targetX, targetZ, idPlayer) && (ids[0] < 19) && openList[ids[0] + 1][ids[1]].g == 0)
                    {
                        openList[ids[0] + 1][ids[1]].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0] + 1][ids[1]].f = openList[ids[0] + 1][ids[1]].g + 1 + ComputeH(ids[0] + 1, ids[1], targetX, targetZ);
                        if (ids[0] + 1 >= 9 && ids[0] + 1 <= 10 && (ids[1] >= 5 && ids[1] <= 15) || (ids[0] + 1 >= 5 && ids[0] + 1 <= 15) && ids[1] <= 10 && ids[1] >= 9)
                        {
                            
                            intent[1] = ActionsAvailable.SHOT_RIGHT;
                            openList[ids[0] + 1][ids[1]].f = 222222;
                        }
                    }

                    if (CheckProxProj(ref gs, ids[0] + 1, ids[1] + 1, targetX, targetZ, idPlayer) && (ids[0] < 19 && ids[1] < 19) && openList[ids[0] + 1][ids[1] + 1].g == 0)
                    {
                        openList[ids[0] + 1][ids[1] + 1].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0] + 1][ids[1] + 1].f = openList[ids[0] + 1][ids[1] + 1].g + 1 + ComputeH(ids[0] + 1, ids[1] + 1, targetX, targetZ);
                        if (ids[0] + 1 <= 10 && ids[0] + 1 >= 9 && ids[1] + 1 >= 5 && ids[1] + 1 <= 15 || ids[1] + 1 <= 10 && ids[1] + 1 >= 9 && ids[0] + 1 >= 5 && ids[0] + 1 <= 15)
                        {
                            openList[ids[0] + 1][ids[1] + 1].f = 222222;
                        }
                    }

                    if (CheckProxProj(ref gs, ids[0], ids[1] - 1, targetX, targetZ, idPlayer) && (ids[1] > 0) && openList[ids[0]][ids[1] - 1].g == 0)
                    {
                        openList[ids[0]][ids[1] - 1].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0]][ids[1] - 1].f = openList[ids[0]][ids[1] - 1].g + 1 + ComputeH(ids[0], ids[1] - 1, targetX, targetZ);
                        if (ids[0] <= 10 && ids[0] >= 9 && ids[1] - 1 >= 5 && ids[1] - 1 <= 15 || ids[1] - 1 >= 9 && ids[1] - 1 <= 10 && ids[0] >= 5 && ids[0] <= 15)
                        {
                            intent[1] = ActionsAvailable.SHOT_BACK;
                            openList[ids[0]][ids[1] - 1].f = 222222;
                        }
                    }
                    
                    if (CheckProxProj(ref gs, ids[0], ids[1] + 1, targetX, targetZ, idPlayer) && (ids[1] < 19) && openList[ids[0]][ids[1] + 1].g == 0)
                    {
                        openList[ids[0]][ids[1] + 1].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0]][ids[1] + 1].f = openList[ids[0]][ids[1] + 1].g + 1 + ComputeH(ids[0], ids[1] + 1, targetX, targetZ);
                        if (ids[0] >= 9 && ids[0] <= 10 && ids[1] + 1 >= 5 && ids[1] + 1 <= 15 || ids[1] + 1 <= 10 && ids[1] + 1 >= 9 && ids[0] >= 5 && ids[0] <= 15) 
                        {
                            intent[1] = ActionsAvailable.SHOT_FORWARD;
                            openList[ids[0]][ids[1] + 1].f = 222222;
                        }
                    }

                    if (CheckProxProj(ref gs, ids[0] - 1, ids[1] - 1, targetX, targetZ, idPlayer) && (ids[0] > 0 && ids[1] > 0) && openList[ids[0] - 1][ids[1] - 1].g == 0)
                    {
                        openList[ids[0] - 1][ids[1] - 1].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0] - 1][ids[1] - 1].f = openList[ids[0] - 1][ids[1] - 1].g + 1 + ComputeH(ids[0] - 1, ids[1] - 1, targetX, targetZ);
                        if (ids[0] - 1 <= 10 && ids[0] - 1 >= 9 && ids[1] - 1 >= 5 && ids[1] - 1 <= 15 || ids[1] - 1 <= 10 && ids[1] - 1 >= 9 && ids[0] - 1 >= 5 && ids[0] - 1 <= 15)
                        {
                            openList[ids[0] - 1][ids[1] - 1].f = 222222;
                        }
                    }

                    if (CheckProxProj(ref gs, ids[0] - 1, ids[1], targetX, targetZ, idPlayer) && (ids[0] > 0) && openList[ids[0] - 1][ids[1]].g == 0)
                    {
                        openList[ids[0] - 1][ids[1]].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0] - 1][ids[1]].f = openList[ids[0] - 1][ids[1]].g + 1 + ComputeH(ids[0] - 1, ids[1], targetX, targetZ);
                        if (ids[0] - 1 <= 10 && ids[0] - 1 >= 9 && ids[1] >= 5 && ids[1] <= 15 || ids[1] <= 10 && ids[1] >= 9 && ids[0] - 1 >= 5 && ids[0] - 1 <= 15)
                        {
                            intent[1] = ActionsAvailable.SHOT_LEFT;
                            openList[ids[0] - 1][ids[1]].f = 222222;
                        }
                    }

                    if (CheckProxProj(ref gs, ids[0] - 1, ids[1] + 1, targetX, targetZ, idPlayer) && (ids[0] > 0 && ids[1] < 19) && openList[ids[0] - 1][ids[1] + 1].g == 0)
                    {
                        openList[ids[0] - 1][ids[1] + 1].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0] - 1][ids[1] + 1].f = openList[ids[0] - 1][ids[1] + 1].g + 1 + ComputeH(ids[0] - 1, ids[1] + 1, targetX, targetZ);
                        if (ids[0] - 1 <= 10 && ids[0] - 1 >= 9 && ids[1] + 1 >= 5 && ids[1] + 1 <= 15 || ids[1] + 1 >= 9 && ids[1] + 1 <= 10 && ids[0] - 1 >= 5 && ids[0] - 1 <= 15)
                        {
                            openList[ids[0] - 1][ids[1] + 1].f = 222222;
                        }
                    }

                    closedList.Add(Nmin);
                    Nmin.f += 99999;
                }

                Location loc = null;
                int indexLoc;
                for (indexLoc = closedList.Count - 1; indexLoc >= 0; indexLoc--)
                {
                    loc = closedList[indexLoc];

                    if (loc.f - loc.g - 100000!= 0)
                    {
                        continue;
                    }

                    break;
                }

                indexLoc--;
                if (indexLoc <= 0)
                {
                    return;
                }
                var preLoc = closedList[indexLoc];

                int loop = closedList.Count;
                int counterLoop = 0;
                
                while (indexLoc > 0)
                {
                    if (preLoc.g == loc.g - 1 &&
                        (preLoc.x == loc.x || preLoc.x == loc.x - 1 || preLoc.x == loc.x + 1) &&
                        (preLoc.z == loc.z || preLoc.z == loc.z - 1 || preLoc.z == loc.z + 1)
                    )
                    {
                        if (preLoc.g == 0)
                        {
                            break;
                        }
                        
                        loc = preLoc;
                    }

                    indexLoc--;
                    preLoc = closedList[indexLoc];
                }

                Location start = closedList[0];

                intent[0] = ActionsAvailable.NONE;
                intent[1] = ActionsAvailable.NONE;
                if (loc.x == start.x)
                {
                    if (loc.z > start.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_FORWARD;
                        intent[1] = ActionsAvailable.SHOT_FORWARD;
                    }
                    else if (loc.z < start.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_BACK;
                        intent[1] = ActionsAvailable.SHOT_BACK;
                    }
                }
                else if (loc.x > start.x)
                {
                    if (loc.z > start.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_FORWARD_RIGHT;
                    }
                    else if (loc.z < start.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_BACK_RIGHT;
                    }
                    else
                    {
                        intent[0] = ActionsAvailable.MOVE_RIGHT;
                        intent[1] = ActionsAvailable.SHOT_RIGHT;
                    }
                }
                else
                {
                    if (loc.z > start.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_FORWARD_LEFT;
                    }
                    else if (loc.z < start.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_BACK_LEFT;
                    }
                    else
                    {
                        intent[0] = ActionsAvailable.MOVE_LEFT;
                        intent[1] = ActionsAvailable.SHOT_LEFT;
                    }
                }
            }
        }
        
        public AStarAgent()
        {
            intent = new Intent();
        }

        public Intent Act(ref GameStateData gs, int id)
        {
            var job = new AStartJob
            {
                gs = gs,
                idPlayer = id,
                idTarget = id == 0 ? 1 : 0,
                intent = new NativeArray<ActionsAvailable>(2, Allocator.TempJob)
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