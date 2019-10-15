using System.Collections.Generic;
using Games.GameState;
using Unity.Collections;
using UnityEngine;

namespace Games.Agents
{
    public struct RandomAgent : Agent
    {
        public Unity.Mathematics.Random rdm;
        private Intent intent;

        public Intent Act(ref GameStateData gs, int id)
        {
            intent.actionIntent =
                (ActionsAvailable) rdm.NextInt((int) ActionsAvailable.NONE, (int) ActionsAvailable.BLOCK);
            intent.moveIntent = (ActionsAvailable) rdm.NextInt(0, (int) ActionsAvailable.NONE);

            return intent;
        }
    }
}
