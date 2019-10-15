using System;
using System.Collections.Generic;
using Games.Agents;
using Unity.Collections;
using UnityEngine;

namespace Games.GameState
{
    public static class GameStateRules
    {   
        //public static Unity.Mathematics.Random rdm;
        
        public const int MAX_PLAYERS = 2;
        public const int MAX_ITEMS = 6;
        public const int MAX_X = 10;
        public const int MAX_Z = 10;
        public const int MIN_X = -10;
        public const int MIN_Z = -10;
        
        // Fonction qui initialise le gameState
        public static void Init(ref GameStateData gs)
        {
            // On initialise 2 NativeList pour stocker les 2 players et 100 projectiles
            gs.players = new NativeList<PlayerData>(MAX_PLAYERS, Allocator.Persistent);
            gs.projectiles = new NativeList<ProjectileData>(100, Allocator.Persistent);
            gs.items = new NativeList<ItemsData>(MAX_ITEMS, Allocator.Persistent);
            gs.lastDistance = 100;
            gs.canWin = new NativeArray<bool>(MAX_PLAYERS, Allocator.Persistent);
               
            InitPlayers(ref gs);
            InitItems(ref gs);
            // On crée un object en initialisant sa position et sa rotation

        }

        private static void InitPlayers(ref GameStateData gs)
        {
            // On crée les 2 joueurs en initialisant leurs positions respectives
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                var player = new PlayerData
                {
                    playerPosition = new Vector3(8 * (i == 1 ? -1 : 1), 1, 8 * (i == 1 ? -1 : 1)),
                    PlayerSpeed = 0.2f,
                    PlayerRadius = 0.5f,
                    PlayerScore = 0
                };
                // On les ajoutes à la liste
                gs.players.Add(player);
            }
        }

        private static void InitItems(ref GameStateData gs)
        {
            for (int i = 0; i < MAX_ITEMS; i++)
            {
                var item = new ItemsData
                {
                    position = new Vector2(UnityEngine.Random.Range(-8.0f, 8.0f), 
                        UnityEngine.Random.Range(-8.0f, 8.0f)),
                    rotation = new Vector3(0, 0, 0),
                    //rotationSpeed = 10,
                    rotationSpeedVector = new Vector3(5, 5, 5),
                    radius = 1
                };
                gs.items.Add(item);
                Debug.Log(item.position);
            }
        }
        
        // Fonction qui prend en entrée une action pour chaques joueurs,
        // qui update le GameState et calcul le score des 2 joueurs

        public static void Step(ref GameStateData gs, Intent player1Actions, Intent player2Actions)
        {
            HandleAgentInputs(ref gs, player1Actions, 0);
            HandleAgentInputs(ref gs, player2Actions, 1);
            UpdateProjectiles(ref gs);

            CalculateScore(ref gs);
        }
        
        public static void Step(ref GameStateData gs, Intent player1Actions, int id)
        {
            HandleAgentInputs(ref gs, player1Actions, id);
            //UpdateProjectiles(ref gs);
            CalculateScore(ref gs);
        }

        public static GameStateData Clone(ref GameStateData gs)
        {
            GameStateData newGs = new GameStateData();
            
            newGs.players = new NativeList<PlayerData>(MAX_PLAYERS, Allocator.Temp);
            newGs.players.AddRange(gs.players);

            newGs.projectiles = new NativeList<ProjectileData>(100, Allocator.Temp);
            newGs.projectiles.AddRange(gs.projectiles);

            newGs.lastDistance = gs.lastDistance;

            return newGs;
        }
        
        public static void CopyTo(ref GameStateData gs, ref GameStateData gsCopy)
        {
            gsCopy.players.Clear();
            gsCopy.players.AddRange(gs.players);

            gsCopy.projectiles.Clear();
            gsCopy.projectiles.AddRange(gs.projectiles);

            gsCopy.lastDistance = gs.lastDistance;
        }
        
        static void UpdateProjectiles(ref GameStateData gs)
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
                    gs.projectiles[i] = projectile;
                }
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

        public static void UpdateItems(ref GameStateData gs)
        {
            for (var i = 0; i < MAX_ITEMS; i++)
            {
                var items = gs.items[i];
                //items.rotation.y += gs.items[i].rotationSpeed;
                items.rotation += gs.items[i].rotationSpeedVector;
                gs.items[i] = items;
            }
        }

        /*static void CollisionTrigger(ref GameStateData gs)
        {
            if (gs.projectiles.Length > 0)
            {
                for (var i = 0; i < gs.projectiles.Length; i++)
                {
                    var projectile = gs.projectiles[i];
                    if (projectile[i])
                    {
                        
                    }
                }
            }
        }*/

        // Selon l'Id du joueur choisi, execute l'action choisie et update le GameState
        public static void HandleAgentInputs(ref GameStateData gs, Intent actions, int id)
        {
            var playerData = gs.players[id];
            switch (actions.moveIntent)
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
            }

            switch (actions.actionIntent)
            {
                case ActionsAvailable.SHOT_FORWARD: // SHOOT UP
                    gs.projectiles.Add(new ProjectileData
                    {
                        position = gs.players[id].playerPosition + Vector3.forward * 1.2f,
                        speed = Vector3.forward * GameStateData.projectileSpeed,
                        radius = GameStateData.projectileRadius,
                        ownerId = id
                    });
                    break;
                case ActionsAvailable.SHOT_BACK: // SHOOT BACK
                    gs.projectiles.Add(new ProjectileData
                    {
                        position = gs.players[id].playerPosition + Vector3.back * 1.2f,
                        speed = Vector3.back * GameStateData.projectileSpeed,
                        radius = GameStateData.projectileRadius,
                        ownerId = id
                    });
                    break;
                case ActionsAvailable.SHOT_LEFT: // SHOOT LEFT
                    gs.projectiles.Add(new ProjectileData
                    {
                        position = gs.players[id].playerPosition + Vector3.left * 1.2f,
                        speed = Vector3.left * GameStateData.projectileSpeed,
                        radius = GameStateData.projectileRadius,
                        ownerId = id
                    });
                    break;
                case ActionsAvailable.SHOT_RIGHT: // SHOOT RIGHT
                    gs.projectiles.Add(new ProjectileData
                    {
                        position = gs.players[id].playerPosition + Vector3.right * 1.2f,
                        speed = Vector3.right * GameStateData.projectileSpeed,
                        radius = GameStateData.projectileRadius,
                        ownerId = id
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

            if (distance < gs.lastDistance)
            {
                gs.lastDistance = distance;
                player1Data.PlayerScore += 10;
                player2Data.PlayerScore += 10;
            }

            gs.players[0] = player1Data;
            gs.players[1] = player2Data;
        }
    }
}