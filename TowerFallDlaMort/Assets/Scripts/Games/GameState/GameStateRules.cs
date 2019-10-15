using System;
using System.Collections.Generic;
using Games.Agents;
using Unity.Collections;
using UnityEngine;

namespace Games.GameState
{
    public static class GameStateRules
    {
        public const int MAX_PLAYERS = 2;
        
        public static void Init(ref GameStateData gs)
        {
            gs.players = new NativeList<PlayerData>(MAX_PLAYERS, Allocator.Persistent);
            gs.projectiles = new NativeList<ProjectileData>(100, Allocator.Persistent);
            gs.lastDistance = 100;

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                var player = new PlayerData
                {
                    playerPosition = new Vector3(8 * (i == 1 ? -1 : 1), 1, 8 * (i == 1 ? -1 : 1)),
                    PlayerSpeed = 0.2f,
                    PlayerRadius = 0.5f,
                    PlayerScore = 0
                };
                
                gs.players.Add(player);
            }
        }

        public static void Step(ref GameStateData gs, List<ActionsAvailable> player1Actions, List<ActionsAvailable> player2Actions)
        {
            HandleAgentInputs(ref gs, player1Actions, 0);
            HandleAgentInputs(ref gs, player2Actions, 1);
            
            //CalculateScore(ref gs);
        }
        
        public static void Step(ref GameStateData gs, List<ActionsAvailable> player1Actions)
        {
            HandleAgentInputs(ref gs, player1Actions, 1);
            CalculateScore(ref gs);
        }

        public static GameStateData Clone(GameStateData gs)
        {
            GameStateData newGs = new GameStateData();
            
            newGs.players = new NativeList<PlayerData>(MAX_PLAYERS, Allocator.Temp);
            newGs.players.AddRange(gs.players);

            newGs.projectiles = new NativeList<ProjectileData>(100, Allocator.Temp);
            newGs.projectiles.AddRange(gs.projectiles);

            newGs.lastDistance = gs.lastDistance;

            return newGs;
        }
        
        static void UpdateProjectiles(ref GameStateData gs)
        {
            for (var i = 0; i < gs.projectiles.Length; i++)
            {
                var projectile = gs.projectiles[i];
                projectile.position += gs.projectiles[i].speed;
                gs.projectiles[i] = projectile;
            }
        }

        public static void HandleAgentInputs(ref GameStateData gs, List<ActionsAvailable> actions, int i)
        {
            var playerData = gs.players[i];
            foreach (var action in actions)
            {
                switch (action)
                {
                    case ActionsAvailable.MOVE_FORWARD:
                        playerData.playerPosition += Vector3.forward * playerData.PlayerSpeed;
                        break;
                    case ActionsAvailable.MOVE_BACK:
                        playerData.playerPosition += Vector3.back * playerData.PlayerSpeed;
                        break;
                    case ActionsAvailable.MOVE_LEFT:
                        playerData.playerPosition += Vector3.left * playerData.PlayerSpeed;
                        break;
                    case ActionsAvailable.MOVE_RIGHT:
                        playerData.playerPosition += Vector3.right * playerData.PlayerSpeed;
                        break;
                    case ActionsAvailable.MOVE_FORWARD_LEFT:
                        playerData.playerPosition += Vector3.forward * playerData.PlayerSpeed + Vector3.left * playerData.PlayerSpeed;
                        break;
                    case ActionsAvailable.MOVE_FORWARD_RIGHT:
                        playerData.playerPosition += Vector3.forward * playerData.PlayerSpeed + Vector3.right * playerData.PlayerSpeed;
                        break;
                    case ActionsAvailable.MOVE_BACK_LEFT:
                        playerData.playerPosition += Vector3.back * playerData.PlayerSpeed + Vector3.left * playerData.PlayerSpeed;
                        break;
                    case ActionsAvailable.MOVE_BACK_RIGHT:
                        playerData.playerPosition += Vector3.back * playerData.PlayerSpeed + Vector3.right * playerData.PlayerSpeed;
                        break;
                    case ActionsAvailable.BLOCK:
                        break;
                    case ActionsAvailable.SHOT_FORWARD: // SHOOT UP
                        gs.projectiles.Add(new ProjectileData
                        {
                            position = gs.players[i].playerPosition + Vector3.forward * 1.2f,
                            speed = Vector3.forward * GameStateData.projectileSpeed
                        });
                        break;
                    case ActionsAvailable.SHOT_BACK: // SHOOT BACK
                        gs.projectiles.Add(new ProjectileData
                        {
                            position = gs.players[i].playerPosition + Vector3.back * 1.2f,
                            speed = Vector3.back * GameStateData.projectileSpeed
                        });
                        break;
                    case ActionsAvailable.SHOT_LEFT: // SHOOT LEFT
                        gs.projectiles.Add(new ProjectileData
                        {
                            position = gs.players[i].playerPosition + Vector3.left * 1.2f,
                            speed = Vector3.left * GameStateData.projectileSpeed
                        });
                        break;
                    case ActionsAvailable.SHOT_RIGHT: // SHOOT RIGHT
                        gs.projectiles.Add(new ProjectileData
                        {
                            position = gs.players[i].playerPosition + Vector3.right * 1.2f,
                            speed = Vector3.right * GameStateData.projectileSpeed
                        });
                        break;
                }
            }
            
            gs.players[i] = playerData;
        }

        public static void CalculateScore(ref GameStateData gs)
        {
            var player1Data = gs.players[0];
            var player2Data = gs.players[1];
            
            float distance = Vector3.Distance(player1Data.playerPosition, player2Data.playerPosition);

            if (distance < gs.lastDistance)
            {
                gs.lastDistance = distance;
                player1Data.PlayerScore += 10;
                player2Data.PlayerScore += 10;
            }

            //gs.players[0] = player1Data;
            gs.players[1] = player2Data;
        }
    }
}