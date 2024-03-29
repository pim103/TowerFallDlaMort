﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Games.Agents;
using Scripts.Games;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Games.GameState
{
    public static class GameStateRules
    {  
        public const int MAX_PLAYERS = 2;
        public const int MAX_ITEMS = 6;
        public const int MAX_X = 10;
        public const int MAX_Z = 10;
        public const int MIN_X = -10;
        public const int MIN_Z = -10;
        public const int MAX_OBSTACLE = 4;
        private static bool turretHasShoot = false;

        [BurstCompile]
        public struct UpdateProjectilesJob : IJob
        {
            public GameStateData gs;

            public NativeList<ProjectileData> projectilesList;
            
            void UpdateProjectiles()
            {
                if (gs.projectiles.Length > 0)
                {
                    for (var i = 0; i < gs.projectiles.Length; i++)
                    {
                        var projectile = gs.projectiles[i];
                        projectile.position += gs.projectiles[i].speed;
                    
                        Vector3 currentPosition = projectile.position;
                        if (projectile.position.x < MIN_X)
                        {
                            currentPosition.x = MAX_X - 1;
                        }
                        else if (projectile.position.x > MAX_X )
                        {
                            currentPosition.x = MIN_X + 1;
                        }

                        if (projectile.position.z < MIN_Z)
                        {
                            currentPosition.z = MAX_Z - 1;
                        }
                        else if (projectile.position.z > MAX_Z)
                        {
                            currentPosition.z = MIN_Z + 1;
                        }

                        projectile.position = currentPosition;
                        projectile.timer--;
                        gs.projectiles[i] = projectile;

                        if (projectile.timer <= 0)
                        {
                            gs.projectiles.RemoveAtSwapBack(i);
                        }
                    }
                }

                projectilesList.AddRange(gs.projectiles);
            }
            
            public void Execute()
            {
                UpdateProjectiles();
            }
        }
        
        // Fonction qui initialise le gameState
        public static void Init(ref GameStateData gs, bool enablePaintWeapon)
        {
            // On initialise 2 NativeList pour stocker les 2 players et 100 projectiles
            gs.players = new NativeList<PlayerData>(MAX_PLAYERS, Allocator.Persistent);
            gs.projectiles = new NativeList<ProjectileData>(100, Allocator.Persistent);
            gs.items = new NativeList<ItemsData>(MAX_ITEMS, Allocator.Persistent);
            gs.lastDistance = 100;
            gs.canWin = new NativeArray<bool>(MAX_PLAYERS, Allocator.Persistent);
            gs.currentGameStep = 0;
            gs.EndOfGame = false;
            gs.rdm = new Unity.Mathematics.Random((uint) Time.frameCount);
            gs.obstacles = new NativeList<ObstacleData>(MAX_OBSTACLE * 4, Allocator.Persistent);
            gs.timer = 99;
            
            InitPlayers(ref gs, enablePaintWeapon);
            InitItems(ref gs);
            InitObstacles(ref gs);
            // On crée un object en initialisant sa position et sa rotation

        }

        private static void InitPlayers(ref GameStateData gs, bool enablePaintWeapon)
        {
            // On crée les 2 joueurs en initialisant leurs positions respectives
            Weapon weapon = enablePaintWeapon ? WeaponList.paint_weapon : WeaponList.basic_weapon;

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                var player = new PlayerData
                {
                    playerPosition = new Vector3(8 * (i == 1 ? -1 : 1), 1, 8 * (i == 1 ? -1 : 1)),
                    PlayerSpeed = 0.2f,
                    PlayerRadius = 0.5f,
                    PlayerScore = 0,
                    weapon = weapon,
                    PlayerLifeStock = 3,
                    IsUntouchable = false
                };
                // On les ajoutes à la liste
                gs.players.Add(player);
            }
        }

        private static void InitObstacles(ref GameStateData gs)
        {
            for (int i = 1; i < MAX_OBSTACLE + 1; i++)
            {
                var obstacle1 = new ObstacleData
                {
                    position = new Vector3(0, 0, i),
                    radius = GameStateData.obstacleRadius
                };
                var obstacle2 = new ObstacleData
                {
                    position = new Vector3(0, 0, -i),
                    radius = GameStateData.obstacleRadius
                };
                var obstacle3 = new ObstacleData
                {
                    position = new Vector3(i, 0, 0),
                    radius = GameStateData.obstacleRadius
                };
                var obstacle4 = new ObstacleData
                {
                    position = new Vector3(-i, 0, 0),
                    radius = GameStateData.obstacleRadius
                };
                gs.obstacles.Add(obstacle1);
                gs.obstacles.Add(obstacle2);
                gs.obstacles.Add(obstacle3);
                gs.obstacles.Add(obstacle4);
            }
        }

        private static void InitTurret(ref GameStateData gs, Vector3 turretPos, float speed, int durability)
        {
            gs.projectiles.Add(new ProjectileData
            {
                position = turretPos,
                speed = new Vector3(gs.rdm.NextFloat(MIN_X, MAX_X), 0, gs.rdm.NextFloat(MIN_X, MAX_X)) * speed,
                radius = GameStateData.projectileRadius,
                ownerId = 2,
                timer = durability
            });
        }

        private static void InitItems(ref GameStateData gs)
        {
            for (int i = 0; i < MAX_ITEMS; i++)
            {
                var item = new ItemsData
                {
                    position = new Vector3(NoObjectInObstacle(ref gs, true), 
                        NoObjectInObstacle(ref gs, false)),
                    rotation = new Vector3(0, 0, 0),
                    //rotationSpeed = 10,
                    rotationSpeedVector = new Vector3(0, 5, 0),
                    radius = 0.5f,
                    active = true
                };
                gs.items.Add(item);
                //Debug.Log(item.position);
            }
        }
        
        private static float NoObjectInObstacle(ref GameStateData gs, bool isX)
        {
            float nb = 0.0f;
            do
            {
                if (isX)
                {
                    nb = gs.rdm.NextFloat(MIN_X, MAX_X);
                }
                else
                {
                    nb = gs.rdm.NextFloat(MIN_Z, MAX_Z);
                }
                
            } while (nb >= -1.0f && nb <= 1.0f);
            
            return nb;
        }
        
        // Fonction qui prend en entrée une action pour chaques joueurs,
        // qui update le GameState et calcul le score des 2 joueurs

        public static void Step(ref GameStateData gs, Intent player1Actions, Intent player2Actions)
        {
            HandleAgentInputs(ref gs, player1Actions, 0);
            HandleAgentInputs(ref gs, player2Actions, 1);
            //UpdateProjectiles(ref gs);
            CollisionTriggerProjectile(ref gs);
            CheckIfPlayerIsDead(ref gs);
            CheckIfTimerHasEnded(ref gs);

            var job = new UpdateProjectilesJob
            {
                gs = gs,
                projectilesList = new NativeList<ProjectileData>(Allocator.TempJob)
            };
            var handle = job.Schedule();
            handle.Complete();

            gs.projectiles.Clear();
            gs.projectiles.AddRange(job.projectilesList);
            job.projectilesList.Dispose();

            gs.currentGameStep++;

            CalculateScore(ref gs);
            gs.timer -= Time.deltaTime;
            TurretShoot(ref gs, 4);
        }
        
        public static void Step(ref GameStateData gs, Intent player1Actions, int id)
        {
            HandleAgentInputs(ref gs, player1Actions, id);
            UpdateProjectiles(ref gs);
            CollisionTriggerProjectile(ref gs);
            CheckIfPlayerIsDead(ref gs);
            //CheckIfTimerHasEnded(ref gs);
            
            gs.currentGameStep++;

            CalculateScore(ref gs);
            //gs.timer -= Time.deltaTime;
            //TurretShoot(ref gs, 4);
        }

        public static void TurretShoot(ref GameStateData gs, int nbShoot)
        {
            if ((int)gs.timer % 5 == 0 && turretHasShoot == false)
            {
                for (int i = 0; i < nbShoot; i++)
                {
                    InitTurret(ref gs, new Vector3(NoObjectInObstacle(ref gs, true), 0, NoObjectInObstacle(ref gs, false)), 0.02f, 250);
                }
                turretHasShoot = true;
            }

            if ((int)gs.timer % 5 != 0)
            {
                turretHasShoot = false;
            }
        }

        public static void CheckIfTimerHasEnded(ref GameStateData gs)
        {
            if (gs.timer <= 0)
            {
                gs.EndOfGame = true;
            }
        }

        public static GameStateData Clone(ref GameStateData gs)
        {
            GameStateData newGs = new GameStateData();
            
            newGs.players = new NativeList<PlayerData>(MAX_PLAYERS, Allocator.Temp);
            newGs.players.AddRange(gs.players);

            newGs.projectiles = new NativeList<ProjectileData>(100, Allocator.Temp);
            newGs.projectiles.AddRange(gs.projectiles);
            
            newGs.items = new NativeList<ItemsData>(MAX_ITEMS, Allocator.Temp);
            newGs.items.AddRange(gs.items);

            newGs.lastDistance = gs.lastDistance;
            newGs.rdm = gs.rdm;
            newGs.currentGameStep = gs.currentGameStep;
            newGs.EndOfGame = gs.EndOfGame;

            newGs.timer = gs.timer;
            
            newGs.obstacles = new NativeList<ObstacleData>(MAX_OBSTACLE * 4, Allocator.Temp);
            newGs.obstacles.AddRange(gs.obstacles);

            return newGs;
        }
        
        public static void CopyTo(ref GameStateData gs, ref GameStateData gsCopy)
        {
            gsCopy.players.Clear();
            gsCopy.players.AddRange(gs.players);

            gsCopy.projectiles.Clear();
            gsCopy.projectiles.AddRange(gs.projectiles);
            
            gsCopy.items.Clear();
            gsCopy.items.AddRange(gs.items);

            gsCopy.lastDistance = gs.lastDistance;
            gsCopy.rdm = gs.rdm;
            gsCopy.currentGameStep = gs.currentGameStep;
            gsCopy.EndOfGame = gs.EndOfGame;
            
            gsCopy.obstacles.Clear();
            gsCopy.obstacles.AddRange(gs.obstacles);

            gsCopy.timer = gs.timer;
        }

        private static void CheckIfPlayerIsDead(ref GameStateData gs)
        {
            if (gs.players[0].PlayerLifeStock <= 0)
            {
                gs.EndOfGame = true;
            }

            if (gs.players[1].PlayerLifeStock <= 0)
            {
                gs.EndOfGame = true;
            }
        }
        
        public static void CheckPlayerTp(ref GameStateData gs, int id)
        {
            var playerData = gs.players[id];

            Vector3 currentPosition = playerData.playerPosition;
            if (playerData.playerPosition.x < MIN_X)
            {
                currentPosition.x = MAX_X - 1;
            }
            else if (playerData.playerPosition.x > MAX_X )
            {
                currentPosition.x = MIN_X + 1;
            }

            if (playerData.playerPosition.z < MIN_Z)
            {
                currentPosition.z = MAX_Z - 1;
            }
            else if (playerData.playerPosition.z > MAX_Z)
            {
                currentPosition.z = MIN_Z + 1;
            }

            playerData.playerPosition = currentPosition;
            gs.players[id] = playerData;
        }
        
        private static void UpdateProjectiles(ref GameStateData gs)
        {
            if (gs.projectiles.Length > 0)
            {
                for (var i = 0; i < gs.projectiles.Length; i++)
                {
                    var projectile = gs.projectiles[i];
                    projectile.position += gs.projectiles[i].speed;
                    
                    Vector3 currentPosition = projectile.position;
                    if (projectile.position.x < MIN_X)
                    {
                        currentPosition.x = MAX_X - 1;
                    }
                    else if (projectile.position.x > MAX_X )
                    {
                        currentPosition.x = MIN_X + 1;
                    }

                    if (projectile.position.z < MIN_Z)
                    {
                        currentPosition.z = MAX_Z - 1;
                    }
                    else if (projectile.position.z > MAX_Z)
                    {
                        currentPosition.z = MIN_Z + 1;
                    }

                    projectile.position = currentPosition;
                    projectile.timer--;
                    gs.projectiles[i] = projectile;

                    if (projectile.timer <= 0)
                    {
                        gs.projectiles.RemoveAtSwapBack(i);
                    }
                }
            }
        }

        public static void UpdateItems(ref GameStateData gs)
        {
            for (var i = 0; i < MAX_ITEMS; i++)
            {
                var items = gs.items[i];
                items.rotation += gs.items[i].rotationSpeedVector;
                gs.items[i] = items;
            }
        }
        
        static void CollisionTriggerProjectile(ref GameStateData gs)
        {
            var player1 = gs.players[0];
            var player2 = gs.players[1];
            
            if (gs.projectiles.Length > 0)
            {
                for (var i = 0; i < gs.projectiles.Length; i++)
                {
                    //bool isDestroyed = false;
                    var projectile = gs.projectiles[i];

                    switch (projectile.ownerId)
                    {
                        case 0:
                            if ( !(player2.IsUntouchable || projectile.position.x + projectile.radius <
                                   player2.playerPosition.x - player2.PlayerRadius ||
                                   projectile.position.x - projectile.radius >
                                   player2.playerPosition.x + player2.PlayerRadius ||
                                   projectile.position.z + projectile.radius <
                                   player2.playerPosition.z - player2.PlayerRadius ||
                                   projectile.position.z - projectile.radius >
                                   player2.playerPosition.z + player2.PlayerRadius))
                            {
                                player2.playerPosition = new Vector3(NoObjectInObstacle(ref gs, true), 0, NoObjectInObstacle(ref gs, false));
            
                                player2.PlayerLifeStock -= 1;
                                player1.PlayerScore += 1000;
                                player2.PlayerScore -= 2000;
                                
                                gs.players[1] = player2;
                                gs.projectiles.RemoveAtSwapBack(i);
                            }
                            break;
                            
                        case 1:
                            if ( !(player1.IsUntouchable || projectile.position.x + projectile.radius <
                                   player1.playerPosition.x - player1.PlayerRadius || projectile.position.x - projectile.radius >
                                   player1.playerPosition.x + player1.PlayerRadius || projectile.position.z + projectile.radius <
                                   player1.playerPosition.z - player1.PlayerRadius || projectile.position.z - projectile.radius >
                                   player1.playerPosition.z + player1.PlayerRadius))
                            {
                                player1.playerPosition = new Vector3(NoObjectInObstacle(ref gs, true), 0, NoObjectInObstacle(ref gs, false));
            
                                player1.PlayerLifeStock -= 1;
                                player2.PlayerScore += 1000;
                                player1.PlayerScore -= 2000;
                                
                                gs.players[0] = player1;
                                gs.projectiles.RemoveAtSwapBack(i);
                            }
                            break;
                        case 2:
                            if (!(player1.IsUntouchable || projectile.position.x + projectile.radius <
                                   player1.playerPosition.x - player1.PlayerRadius || projectile.position.x - projectile.radius >
                                   player1.playerPosition.x + player1.PlayerRadius || projectile.position.z + projectile.radius <
                                   player1.playerPosition.z - player1.PlayerRadius || projectile.position.z - projectile.radius >
                                   player1.playerPosition.z + player1.PlayerRadius))
                            {
                                player1.playerPosition = new Vector3(NoObjectInObstacle(ref gs, true), 0, NoObjectInObstacle(ref gs, false));
            
                                player1.PlayerLifeStock -= 1;
                                //player2.PlayerScore += 1000;
                                player1.PlayerScore -= 2000;
                                
                                gs.players[0] = player1;
                                gs.projectiles.RemoveAtSwapBack(i);
                            }
                            else if (!(player2.IsUntouchable || projectile.position.x + projectile.radius <
                                       player2.playerPosition.x - player2.PlayerRadius ||
                                       projectile.position.x - projectile.radius >
                                       player2.playerPosition.x + player2.PlayerRadius ||
                                       projectile.position.z + projectile.radius <
                                       player2.playerPosition.z - player2.PlayerRadius ||
                                       projectile.position.z - projectile.radius >
                                       player2.playerPosition.z + player2.PlayerRadius))
                            {
                                player2.playerPosition = new Vector3(NoObjectInObstacle(ref gs, true), 0, NoObjectInObstacle(ref gs, false));
                                                 
                                player2.PlayerLifeStock -= 1;
                                //player1.PlayerScore += 1000;
                                player2.PlayerScore -= 2000;
                                                                     
                                gs.players[1] = player2;
                                gs.projectiles.RemoveAtSwapBack(i);
                            }
                            break;
                    }
                }
            }

            CheckItemsCollisionTrigger(ref gs, player1, player2);
        }

        public static void CheckItemsCollisionTrigger(ref GameStateData gs, PlayerData player1, PlayerData player2)
        {
            if (gs.items.Length > 0)
            {
                for (var i = 0; i < gs.items.Length; i++)
                {
                    var item = gs.items[i];
                    if (!(item.position.x + item.radius <
                          player1.playerPosition.x - player1.PlayerRadius ||
                          item.position.x - item.radius >
                          player1.playerPosition.x + player1.PlayerRadius ||
                          item.position.y + item.radius <
                          player1.playerPosition.z - player1.PlayerRadius ||
                          item.position.y - item.radius >
                          player1.playerPosition.z + player1.PlayerRadius))
                    {
                        buffPlayer(ref gs, player1, item, i);
                    }
                    if (!(item.position.x + item.radius <
                          player2.playerPosition.x - player2.PlayerRadius ||
                          item.position.x - item.radius >
                          player2.playerPosition.x + player2.PlayerRadius ||
                          item.position.y + item.radius <
                          player2.playerPosition.z - player2.PlayerRadius ||
                          item.position.y - item.radius >
                          player2.playerPosition.z + player2.PlayerRadius))
                    {
                        buffPlayer(ref gs, player2, item, i);
                    }
                }
            }
        }

        public static void buffPlayer(ref GameStateData gs, PlayerData player, ItemsData item, int i)
        {
            if (item.active == true)
            {
                switch (item.type)
                {
                    case 0:
                        player.PlayerLifeStock += 1;
                        //Debug.Log("buff life");
                        break;
                    case 1:
                        player.PlayerSpeed += 0.1f;
                        //Debug.Log("buff speed");
                        break;
                    case 2:
                        //player.weapon = WeaponList.paint_weapon;
                        //Debug.Log("buff weapon");
                        break;
                }
                item.active = false;
                gs.items[i] = item;               
                gs.players[0] = player;
            }
        }

        public static bool CollisionTriggerObstacles(ref GameStateData gs, bool isPlayer, int id, ActionsAvailable action)
        {
            if (isPlayer)
            {
                var playerData = gs.players[id];
                for (int i = 0; i < gs.obstacles.Length; i++)
                {
                    var obstacle = gs.obstacles[i];
                    switch (action)
                    {
                        case ActionsAvailable.MOVE_FORWARD:
                            if (!(playerData.playerPosition.x + playerData.PlayerRadius <
                                  obstacle.position.x - obstacle.radius ||
                                  playerData.playerPosition.x - playerData.PlayerRadius >
                                  obstacle.position.x + obstacle.radius ||
                                  playerData.playerPosition.z + playerData.PlayerRadius + 0.2 <
                                  obstacle.position.z - obstacle.radius ||
                                  playerData.playerPosition.z - playerData.PlayerRadius >
                                  obstacle.position.z + obstacle.radius) && 
                                (playerData.playerPosition + Vector3.forward).z - playerData.PlayerRadius 
                                < obstacle.position.z + obstacle.radius)
                            {
                                return true;
                            }
                            break;
                        case ActionsAvailable.MOVE_BACK:
                            if (!(playerData.playerPosition.x + playerData.PlayerRadius <
                                  obstacle.position.x - obstacle.radius ||
                                  playerData.playerPosition.x - playerData.PlayerRadius >
                                  obstacle.position.x + obstacle.radius ||
                                  playerData.playerPosition.z + playerData.PlayerRadius <
                                  obstacle.position.z - obstacle.radius ||
                                  playerData.playerPosition.z - playerData.PlayerRadius - 0.2 >
                                  obstacle.position.z + obstacle.radius) && 
                                (playerData.playerPosition + Vector3.back).z + playerData.PlayerRadius 
                                > obstacle.position.z - obstacle.radius)
                            {
                                return true;
                            }
                            break;
                        case ActionsAvailable.MOVE_LEFT:
                            if (!(playerData.playerPosition.x + playerData.PlayerRadius <
                                  obstacle.position.x - obstacle.radius ||
                                  playerData.playerPosition.x - playerData.PlayerRadius - 0.2 >
                                  obstacle.position.x + obstacle.radius ||
                                  playerData.playerPosition.z + playerData.PlayerRadius <
                                  obstacle.position.z - obstacle.radius ||
                                  playerData.playerPosition.z - playerData.PlayerRadius >
                                  obstacle.position.z + obstacle.radius) && 
                                (playerData.playerPosition + Vector3.left).x + playerData.PlayerRadius 
                                > obstacle.position.x - obstacle.radius)
                            {
                                return true;
                            }
                            break;
                        case ActionsAvailable.MOVE_RIGHT:
                            if (!(playerData.playerPosition.x + playerData.PlayerRadius + 0.2 <
                                  obstacle.position.x - obstacle.radius ||
                                  playerData.playerPosition.x - playerData.PlayerRadius >
                                  obstacle.position.x + obstacle.radius ||
                                  playerData.playerPosition.z + playerData.PlayerRadius <
                                  obstacle.position.z - obstacle.radius ||
                                  playerData.playerPosition.z - playerData.PlayerRadius >
                                  obstacle.position.z + obstacle.radius) && 
                                (playerData.playerPosition + Vector3.right).x - playerData.PlayerRadius 
                                < obstacle.position.x + obstacle.radius)
                            {
                                return true;
                            }
                            break;
                        case ActionsAvailable.MOVE_FORWARD_LEFT:
                            if (!(playerData.playerPosition.x + playerData.PlayerRadius <
                                  obstacle.position.x - obstacle.radius ||
                                  playerData.playerPosition.x - playerData.PlayerRadius - 0.2 >
                                  obstacle.position.x + obstacle.radius ||
                                  playerData.playerPosition.z + playerData.PlayerRadius + 0.2 <
                                  obstacle.position.z - obstacle.radius ||
                                  playerData.playerPosition.z - playerData.PlayerRadius >
                                  obstacle.position.z + obstacle.radius) && 
                                (playerData.playerPosition + Vector3.forward).z - playerData.PlayerRadius 
                                < obstacle.position.z + obstacle.radius && 
                                (playerData.playerPosition + Vector3.left).x + playerData.PlayerRadius 
                                > obstacle.position.x - obstacle.radius)
                            {
                                return true;
                            }
                            break;
                        case ActionsAvailable.MOVE_FORWARD_RIGHT:
                            if (!(playerData.playerPosition.x + playerData.PlayerRadius + 0.2 <
                                  obstacle.position.x - obstacle.radius ||
                                  playerData.playerPosition.x - playerData.PlayerRadius >
                                  obstacle.position.x + obstacle.radius ||
                                  playerData.playerPosition.z + playerData.PlayerRadius + 0.2 <
                                  obstacle.position.z - obstacle.radius ||
                                  playerData.playerPosition.z - playerData.PlayerRadius >
                                  obstacle.position.z + obstacle.radius) && 
                                (playerData.playerPosition + Vector3.forward).z - playerData.PlayerRadius 
                                < obstacle.position.z + obstacle.radius && 
                                (playerData.playerPosition + Vector3.right).x - playerData.PlayerRadius 
                                < obstacle.position.x + obstacle.radius)
                            {
                                return true;
                            }
                            break;
                        case ActionsAvailable.MOVE_BACK_LEFT:
                            if (!(playerData.playerPosition.x + playerData.PlayerRadius <
                                  obstacle.position.x - obstacle.radius ||
                                  playerData.playerPosition.x - playerData.PlayerRadius - 0.2 >
                                  obstacle.position.x + obstacle.radius ||
                                  playerData.playerPosition.z + playerData.PlayerRadius <
                                  obstacle.position.z - obstacle.radius ||
                                  playerData.playerPosition.z - playerData.PlayerRadius - 0.2 >
                                  obstacle.position.z + obstacle.radius) && 
                                (playerData.playerPosition + Vector3.back).z + playerData.PlayerRadius 
                                > obstacle.position.z - obstacle.radius && 
                                (playerData.playerPosition + Vector3.left).x + playerData.PlayerRadius 
                                > obstacle.position.x - obstacle.radius)
                            {
                                return true;
                            }
                            break;
                        case ActionsAvailable.MOVE_BACK_RIGHT:
                            if (!(playerData.playerPosition.x + playerData.PlayerRadius + 0.2 <
                                  obstacle.position.x - obstacle.radius ||
                                  playerData.playerPosition.x - playerData.PlayerRadius >
                                  obstacle.position.x + obstacle.radius ||
                                  playerData.playerPosition.z + playerData.PlayerRadius <
                                  obstacle.position.z - obstacle.radius ||
                                  playerData.playerPosition.z - playerData.PlayerRadius - 0.2 >
                                  obstacle.position.z + obstacle.radius) && 
                                (playerData.playerPosition + Vector3.back).z + playerData.PlayerRadius 
                                > obstacle.position.z - obstacle.radius && 
                                (playerData.playerPosition + Vector3.right).x - playerData.PlayerRadius 
                                < obstacle.position.x + obstacle.radius)
                            {
                                return true;
                            }
                            break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < gs.obstacles.Length; i++)
                {
                    var projectile = gs.projectiles[id];
                    var obstacle = gs.obstacles[i];
                    if (!(projectile.position.x + projectile.radius <
                          obstacle.position.x - obstacle.radius ||
                          projectile.position.x - projectile.radius >
                          obstacle.position.x + obstacle.radius ||
                          projectile.position.z + projectile.radius <
                          obstacle.position.z - obstacle.radius ||
                          projectile.position.z - projectile.radius >
                          obstacle.position.z + obstacle.radius))
                    {
                        gs.projectiles.RemoveAtSwapBack(id);
                        return true;
                    }
                }
            }
            
            return false;
        }

        // Selon l'Id du joueur choisi, execute l'action choisie et update le GameState
        public static void HandleAgentInputs(ref GameStateData gs, Intent actions, int id)
        {
            var playerData = gs.players[id];
            switch (actions.moveIntent)
            {
                case ActionsAvailable.MOVE_FORWARD:
                    if (CollisionTriggerObstacles(ref gs, true, id, ActionsAvailable.MOVE_FORWARD))
                    {
                        return;
                    }
                    playerData.playerPosition += Vector3.forward * playerData.PlayerSpeed;
                    break;
                case ActionsAvailable.MOVE_BACK:
                    if (CollisionTriggerObstacles(ref gs, true, id, ActionsAvailable.MOVE_BACK))
                    {
                        return;
                    }
                    playerData.playerPosition += Vector3.back * playerData.PlayerSpeed;
                    break;
                case ActionsAvailable.MOVE_LEFT:
                    if (CollisionTriggerObstacles(ref gs, true, id, ActionsAvailable.MOVE_LEFT))
                    {
                        return;
                    }
                    playerData.playerPosition += Vector3.left * playerData.PlayerSpeed;
                    break;
                case ActionsAvailable.MOVE_RIGHT:
                    if (CollisionTriggerObstacles(ref gs, true, id, ActionsAvailable.MOVE_RIGHT))
                    {
                        return;
                    }
                    playerData.playerPosition += Vector3.right * playerData.PlayerSpeed;
                    break;
                case ActionsAvailable.MOVE_FORWARD_LEFT:
                    if (CollisionTriggerObstacles(ref gs, true, id, ActionsAvailable.MOVE_FORWARD_LEFT))
                    {
                        return;
                    }
                    playerData.playerPosition += Vector3.forward * playerData.PlayerSpeed + Vector3.left * playerData.PlayerSpeed;
                    break;
                case ActionsAvailable.MOVE_FORWARD_RIGHT:
                    if (CollisionTriggerObstacles(ref gs, true, id, ActionsAvailable.MOVE_FORWARD_RIGHT))
                    {
                        return;
                    }
                    playerData.playerPosition += Vector3.forward * playerData.PlayerSpeed + Vector3.right * playerData.PlayerSpeed;
                    break;
                case ActionsAvailable.MOVE_BACK_LEFT:
                    if (CollisionTriggerObstacles(ref gs, true, id, ActionsAvailable.MOVE_BACK_LEFT))
                    {
                        return;
                    }
                    playerData.playerPosition += Vector3.back * playerData.PlayerSpeed + Vector3.left * playerData.PlayerSpeed;
                    break;
                case ActionsAvailable.MOVE_BACK_RIGHT:
                    if (CollisionTriggerObstacles(ref gs, true, id, ActionsAvailable.MOVE_BACK_RIGHT))
                    {
                        return;
                    }
                    playerData.playerPosition += Vector3.back * playerData.PlayerSpeed + Vector3.right * playerData.PlayerSpeed;
                    break;
            }

            switch (actions.actionIntent)
            {
                case ActionsAvailable.BLOCK:
                    break;
                case ActionsAvailable.SHOT_FORWARD: // SHOOT UP
                    if (gs.currentGameStep - playerData.weapon.lastShot <= playerData.weapon.frequency)
                    {
                        break;
                    }
                    
                    playerData.weapon.lastShot = gs.currentGameStep;
                    
                    gs.projectiles.Add(new ProjectileData
                    {
                        position = gs.players[id].playerPosition + Vector3.forward * 1.2f,
                        speed = Vector3.forward * gs.players[id].weapon.projectileSpeed,
                        radius = GameStateData.projectileRadius,
                        ownerId = id,
                        timer = gs.players[id].weapon.durability
                    });
                    break;
                case ActionsAvailable.SHOT_BACK: // SHOOT BACK
                    if (gs.currentGameStep - playerData.weapon.lastShot <= playerData.weapon.frequency)
                    {
                        break;
                    }
                    
                    playerData.weapon.lastShot = gs.currentGameStep;
                    
                    gs.projectiles.Add(new ProjectileData
                    {
                        position = gs.players[id].playerPosition + Vector3.back * 1.2f,
                        speed = Vector3.back * gs.players[id].weapon.projectileSpeed,
                        radius = GameStateData.projectileRadius,
                        ownerId = id,
                        timer = gs.players[id].weapon.durability
                    });
                    break;
                case ActionsAvailable.SHOT_LEFT: // SHOOT LEFT
                    if (gs.currentGameStep - playerData.weapon.lastShot <= playerData.weapon.frequency)
                    {
                        break;
                    }
                    
                    playerData.weapon.lastShot = gs.currentGameStep;
                    
                    gs.projectiles.Add(new ProjectileData
                    {
                        position = gs.players[id].playerPosition + Vector3.left * 1.2f,
                        speed = Vector3.left * gs.players[id].weapon.projectileSpeed,
                        radius = GameStateData.projectileRadius,
                        ownerId = id,
                        timer = gs.players[id].weapon.durability
                    });
                    break;
                case ActionsAvailable.SHOT_RIGHT: // SHOOT RIGHT
                    if (gs.currentGameStep - playerData.weapon.lastShot <= playerData.weapon.frequency)
                    {
                        break;
                    }

                    playerData.weapon.lastShot = gs.currentGameStep;
                    
                    gs.projectiles.Add(new ProjectileData
                    {
                        position = gs.players[id].playerPosition + Vector3.right * 1.2f,
                        speed = Vector3.right * gs.players[id].weapon.projectileSpeed,
                        radius = GameStateData.projectileRadius,
                        ownerId = id,
                        timer = gs.players[id].weapon.durability
                    });
                    break;
            }

            gs.players[id] = playerData;
            CheckPlayerTp(ref gs, id);
        }
        
        // Calcul le score de chaque joueur et update le GameState
        public static void CalculateScore(ref GameStateData gs)
        {
            var player1Data = gs.players[0];
            var player2Data = gs.players[1];
            
            float distance = Vector3.Distance(player1Data.playerPosition, player2Data.playerPosition);

            if (distance <= 2)
            {
                return;
            }

            player1Data.PlayerScore += 30-(int)distance;
            player2Data.PlayerScore += 30-(int)distance;

            gs.players[0] = player1Data;
            gs.players[1] = player2Data;
        }
        
        public static long GetHashCode(ref GameStateData gs)
        {
            var hash = 0L;
            hash = (long) math.round(math.clamp(gs.players[0].playerPosition.x, -9.99999f, 9.99999f) + 10) + 
                   100* (long) math.round(math.clamp(gs.players[0].playerPosition.z, -9.99999f, 9.99999f) + 10);
            hash+= 10000*(long) math.round(math.clamp(gs.players[1].playerPosition.x, -9.99999f, 9.99999f) + 10) +
                   1000000 * (long) math.round(math.clamp(gs.players[1].playerPosition.z, -9.99999f, 9.99999f) + 10);

            return hash;
        }
    }
}