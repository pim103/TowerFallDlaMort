using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            public int h;
            public Location parent;
        }

        private static List<Location> GetAvailableLocations(int x, int z, ref GameStateData gs, int id)
        {
            var locations = new List<Location>()
            {
                // UP
                new Location { x = x + 1, z = z, f = 100},
                // BACK
                new Location { x = x - 1, z = z, f = 100},
                // LEFT
                new Location { x = x, z = z - 1, f = 100},
                // RIGHT
                new Location { x = x, z = z + 1, f = 100},
                // UP_LEFT
                new Location { x = x + 1, z = z - 1, f = 100},
                // UP_RIGHT
                new Location { x = x + 1, z = z + 1, f = 100},
                // BACK_LEFT
                new Location { x = x - 1, z = z - 1, f = 100},
                // BACK_RIGHT
                new Location { x = x - 1, z = z + 1, f = 100},
            };

            /*for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i].x < GameStateRules.MIN_X)
                {
                    locations[i].x = GameStateRules.MAX_X - 1;
                }
                else if (locations[i].x > GameStateRules.MAX_X)
                {
                    locations[i].x = GameStateRules.MIN_X + 1;
                }

                if (locations[i].z < GameStateRules.MIN_Z)
                {
                    locations[i].z = GameStateRules.MAX_Z - 1;
                }
                else if (locations[i].z > GameStateRules.MAX_Z)
                {
                    locations[i].z = GameStateRules.MIN_Z + 1;
                }
            }*/

            /*
            for (int i = 0; i < gs.projectiles.Length; i++)
            {
                var proj = gs.projectiles[i];
                var locationIndex = locations.FindIndex(l => 
                    l.x == (int)proj.position.x && (int)proj.position.x != startX && 
                    l.z == (int)proj.position.z && (int)proj.position.z != startZ);
                
                if (locationIndex >= 0)
                {
                    locations[locationIndex].f = 10000;
                }
            }

            var location = locations.FindAll(l => 
                (l.x == 0 && l.z == 0) || 
                (l.x == 0 && l.z == -1) || 
                (l.x == 0 && l.z == -2) || 
                (l.x == 0 && l.z == -3) ||
                (l.x == 0 && l.z == -4) ||
                (l.x == 0 && l.z == -5) || 
                (l.x == 0 && l.z == -6) || 
                (l.x == 0 && l.z == -7) ||
                (l.x == 0 && l.z == -8) ||
                (l.x == 0 && l.z == -9) ||
                (l.x == -1 && l.z == -10) || 
                (l.x == -1 && l.z == -11) || 
                (l.x == -1 && l.z == -12) ||
                (l.x == -1 && l.z == -13) ||
                (l.x == -1 && l.z == -14) ||
                (l.x == -1 && l.z == -15) 
            );
            foreach (var loc in location)
            {
                locations.Remove(loc);
            }*/

            return locations;
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

                    if ( (ids[0] < 19 && ids[1] > 0) && openList[ids[0] + 1][ids[1] - 1].g == 0)
                    {
                        openList[ids[0] + 1][ids[1] - 1].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0] + 1][ids[1] - 1].f = openList[ids[0] + 1][ids[1] - 1].g + ComputeH(ids[0] + 1, ids[1] - 1, targetX, targetZ);
                        if (ids[0] + 1 <= 10 && ids[0] + 1 >= 9 && (ids[1] - 1 >= 5 && ids[1] - 1 <= 15) || (ids[0] + 1 >= 5 && ids[0] + 1 <= 15) && ids[1] - 1 >= 9 && ids[1] - 1 <= 10)
                        {
                            openList[ids[0] + 1][ids[1] - 1].f = 222222;
                        }
                    }

                    if ((ids[0] < 19) && openList[ids[0] + 1][ids[1]].g == 0)
                    {
                        openList[ids[0] + 1][ids[1]].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0] + 1][ids[1]].f = openList[ids[0] + 1][ids[1]].g + 1 + ComputeH(ids[0] + 1, ids[1], targetX, targetZ);
                        if (ids[0] + 1 >= 9 && ids[0] + 1 <= 10 && (ids[1] >= 5 && ids[1] <= 15) || (ids[0] + 1 >= 5 && ids[0] + 1 <= 15) && ids[1] <= 10 && ids[1] >= 9)
                        {
                            openList[ids[0] + 1][ids[1]].f = 222222;
                        }
                    }

                    if ((ids[0] < 19 && ids[1] < 19) && openList[ids[0] + 1][ids[1] + 1].g == 0)
                    {
                        openList[ids[0] + 1][ids[1] + 1].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0] + 1][ids[1] + 1].f = openList[ids[0] + 1][ids[1] + 1].g + 1 + ComputeH(ids[0] + 1, ids[1] + 1, targetX, targetZ);
                        if (ids[0] + 1 <= 10 && ids[0] + 1 >= 9 && ids[1] + 1 >= 5 && ids[1] + 1 <= 15 || ids[1] + 1 <= 10 && ids[1] + 1 >= 9 && ids[0] + 1 >= 5 && ids[0] + 1 <= 15)
                        {
                            openList[ids[0] + 1][ids[1] + 1].f = 222222;
                        }
                    }

                    if ((ids[1] > 0) && openList[ids[0]][ids[1] - 1].g == 0)
                    {
                        openList[ids[0]][ids[1] - 1].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0]][ids[1] - 1].f = openList[ids[0]][ids[1] - 1].g + 1 + ComputeH(ids[0], ids[1] - 1, targetX, targetZ);
                        if (ids[0] <= 10 && ids[0] >= 9 && ids[1] - 1 >= 5 && ids[1] - 1 <= 15 || ids[1] - 1 >= 9 && ids[1] - 1 <= 10 && ids[0] >= 5 && ids[0] <= 15)
                        {
                            openList[ids[0]][ids[1] - 1].f = 222222;
                        }
                    }
                    
                    if ((ids[1] < 19) && openList[ids[0]][ids[1] + 1].g == 0)
                    {
                        openList[ids[0]][ids[1] + 1].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0]][ids[1] + 1].f = openList[ids[0]][ids[1] + 1].g + 1 + ComputeH(ids[0], ids[1] + 1, targetX, targetZ);
                        if (ids[0] >= 9 && ids[0] <= 10 && ids[1] + 1 >= 5 && ids[1] + 1 <= 15 || ids[1] + 1 <= 10 && ids[1] + 1 >= 9 && ids[0] >= 5 && ids[0] <= 15)                        {
                            openList[ids[0]][ids[1] + 1].f = 222222;
                        }
                    }

                    if ((ids[0] > 0 && ids[1] > 0) && openList[ids[0] - 1][ids[1] - 1].g == 0)
                    {
                        openList[ids[0] - 1][ids[1] - 1].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0] - 1][ids[1] - 1].f = openList[ids[0] - 1][ids[1] - 1].g + 1 + ComputeH(ids[0] - 1, ids[1] - 1, targetX, targetZ);
                        if (ids[0] - 1 <= 10 && ids[0] - 1 >= 9 && ids[1] - 1 >= 5 && ids[1] - 1 <= 15 || ids[1] - 1 <= 10 && ids[1] - 1 >= 9 && ids[0] - 1 >= 5 && ids[0] - 1 <= 15)
                        {
                            openList[ids[0] - 1][ids[1] - 1].f = 222222;
                        }
                    }

                    if ((ids[0] > 0) && openList[ids[0] - 1][ids[1]].g == 0)
                    {
                        openList[ids[0] - 1][ids[1]].g = openList[ids[0]][ids[1]].g + 1;
                        openList[ids[0] - 1][ids[1]].f = openList[ids[0] - 1][ids[1]].g + 1 + ComputeH(ids[0] - 1, ids[1], targetX, targetZ);
                        if (ids[0] - 1 <= 10 && ids[0] - 1 >= 9 && ids[1] >= 5 && ids[1] <= 15 || ids[1] <= 10 && ids[1] >= 9 && ids[0] - 1 >= 5 && ids[0] - 1 <= 15)
                        {
                            openList[ids[0] - 1][ids[1]].f = 222222;
                        }
                    }

                    if ((ids[0] > 0 && ids[1] < 19) && openList[ids[0] - 1][ids[1] + 1].g == 0)
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
                    }
                    else if (loc.z < start.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_BACK;
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
                    }
                }

                //Debug.Log("Movement needed : " + loc.x + " " + loc.z + " Start : " + closedList[0].x + " " + closedList[0].z);

                /*
                Location current = new Location();

                var target = new Location
                {
                    x = (int)gs.players[idTarget].playerPosition.x,
                    z = (int)gs.players[idTarget].playerPosition.z
                };
                
                var closeList = new List<Location>();
                
                int g = 0;
                
                openList.Add(start);

                while (openList.Count > 0)
                {
                    var lowest = openList.Min(l => l.f);
                    current = openList.FindLast(l => l.f == lowest);
                    closeList.Add(current);
                    openList.Remove(current);

                    var destination = closeList.FirstOrDefault(l => l.x == target.x && l.z == target.z);
                    if (destination != null)
                    {
                        break;
                    }

                    var availableLocations = GetAvailableLocations(current.x, current.z, start.x, start.z, ref gs, idPlayer);
                    for (int i = 0; i < availableLocations.Count; i++)
                    {
                        var location = availableLocations[i];

                        if (location.f > 100)
                        {
                            Debug.Log("Oulah un proj");
                            continue;
                        }
                        
                        if (closeList.FirstOrDefault(l =>
                            l.x == location.x && l.z == location.z) != null)
                        {
                            continue;
                        }
                        
                        if (openList.FirstOrDefault(l =>
                            l.x == location.x && l.z == location.z) != null)
                        {
                            if (g + location.h < location.f)
                            {
                                location.g = g;
                                location.f = location.g + location.h;
                                location.parent = current;
                            }
                        }
                        else
                        {
                            location.g = g;
                            location.h = ComputeH(location.x, location.z, target.x, target.z);
                            location.f = location.g + location.h;
                            location.parent = current;
                            
                            openList.Add(location);
                        }
                    }
                }

                //openList.Clear();

                Location lastCurrent = new Location();
                while (current.parent != null)
                {
                    lastCurrent = current;
                    current = current.parent;
                }

                intent[0] = ActionsAvailable.NONE;
                intent[1] = ActionsAvailable.NONE;

                if (lastCurrent == null)
                {
                    return;
                }

                if (lastCurrent.x == start.x)
                {
                    if (lastCurrent.z > target.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_BACK;
                    }
                    else if (lastCurrent.z < target.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_FORWARD;
                    }
                }
                else if (lastCurrent.x > target.x)
                {
                    if (lastCurrent.z > target.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_BACK_LEFT;
                    }
                    else if (lastCurrent.z < target.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_FORWARD_LEFT;
                    }
                    else
                    {
                        intent[0] = ActionsAvailable.MOVE_LEFT;
                    }
                }
                else
                {
                    if (lastCurrent.z > target.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_BACK_RIGHT;
                    }
                    else if (lastCurrent.z < target.z)
                    {
                        intent[0] = ActionsAvailable.MOVE_FORWARD_RIGHT;
                    }
                    else
                    {
                        intent[0] = ActionsAvailable.MOVE_RIGHT;
                    }
                }*/
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