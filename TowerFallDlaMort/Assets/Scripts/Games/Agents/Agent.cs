using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Agents
{
    public enum ActionsAvailable
    {
        MOVE_FORWARD,
        MOVE_BACK,
        MOVE_LEFT,
        MOVE_RIGHT,
        SHOT_FORWARD,
        SHOT_BACK,
        SHOT_LEFT,
        SHOT_RIGHT,
        BLOCK
    }

    public interface Agent
    {
        ActionsAvailable[] Act();
    }
}