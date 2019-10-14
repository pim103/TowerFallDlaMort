using System.Collections.Generic;
using Games.Agents;
using Unity.Collections;
using UnityEngine;

namespace Games
{
    public class GameStateRules
    {
        
        static void UpdateProjectiles(ref GameState gs)
        {
            for (var i = 0; i < gs.projectiles.Length; i++)
            {
                var projectile = gs.projectiles[i];
                projectile.position += gs.projectiles[i].speed;
                gs.projectiles[i] = projectile;
            }
        }
        
        static void HandleAgentInputs(ref GameState gs, ActionsAvailable chosenPlayerAction, int playerId)
        {
            switch (chosenPlayerAction)
            {
                case ActionsAvailable.SHOT_FORWARD: // SHOOT UP
                    gs.projectiles.Add(new ProjectileData
                    {
                        position = gs.players[playerId].playerPosition + Vector2.up * 1.2f,
                        speed = Vector2.up * GameState.projectileSpeed
                    });
                    break;
                case ActionsAvailable.SHOT_BACK: // SHOOT BACK
                    gs.projectiles.Add(new ProjectileData
                    {
                        position = gs.players[playerId].playerPosition + Vector2.down * 1.2f,
                        speed = Vector2.down * GameState.projectileSpeed
                    });
                    break;
                case ActionsAvailable.SHOT_LEFT: // SHOOT LEFT
                    gs.projectiles.Add(new ProjectileData
                    {
                        position = gs.players[playerId].playerPosition + Vector2.left * 1.2f,
                        speed = Vector2.left * GameState.projectileSpeed
                    });
                    break;
                case ActionsAvailable.SHOT_RIGHT: // SHOOT RIGHT
                    gs.projectiles.Add(new ProjectileData
                    {
                        position = gs.players[playerId].playerPosition + Vector2.right * 1.2f,
                        speed = Vector2.right * GameState.projectileSpeed
                    });
                    break;
            }
        }
    }
}