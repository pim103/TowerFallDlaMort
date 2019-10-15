using System.Collections;
using System.Collections.Generic;
using Games.GameState;
using Unity.Collections;
using UnityEngine;

namespace Games.Agents
{
    public enum ActionsAvailable
    {
        MOVE_FORWARD,
        MOVE_BACK,
        MOVE_LEFT,
        MOVE_RIGHT,
        MOVE_FORWARD_RIGHT,
        MOVE_FORWARD_LEFT,
        MOVE_BACK_RIGHT,
        MOVE_BACK_LEFT,
        
        NONE,
        
        SHOT_FORWARD,
        SHOT_BACK,
        SHOT_LEFT,
        SHOT_RIGHT,
        
        BLOCK
    }

    public interface Agent
    {
        Intent Act(ref GameStateData gs, int id);
    }
    
}