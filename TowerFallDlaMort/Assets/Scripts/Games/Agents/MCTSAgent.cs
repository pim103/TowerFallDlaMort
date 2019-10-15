using System.Collections.Generic;
using UnityEngine;
using Games.GameState;
using Unity.Mathematics;

namespace Games.Agents
{
    public class MCTSAgent : Agent
    {
        private List<ActionsAvailable> actions = new List<ActionsAvailable>();
        private MCTSTreeNode nodeMaxChoosingValue;
        public Intent Act(ref GameStateData gs,int id)
        {
            MCTSTreeNode parentNode = new MCTSTreeNode();
            parentNode.nodeChoosingValue = 0.1f;
            parentNode.nSelect = 1;
            parentNode.depth = 0;
            int iterations = 10;
            parentNode.childNodes = new List<MCTSTreeNode>();
            for (int i = 0; i <= (int) ActionsAvailable.NONE; i++)
            {
                for (int j = (int) ActionsAvailable.NONE; j <= (int) ActionsAvailable.BLOCK; j++)
                {
                    parentNode.childNodes.Add(new MCTSTreeNode());
                    parentNode.actions = new List<ActionsAvailable>();
                    parentNode.actions.Add(ActionsAvailable.NONE);
                    parentNode.actions.Add(ActionsAvailable.NONE);
                    var parentNodeChildNode = parentNode.childNodes[parentNode.childNodes.Count-1];
                    parentNodeChildNode.actions = new List<ActionsAvailable>();
                    parentNodeChildNode.actions.Add((ActionsAvailable)i);
                    parentNodeChildNode.actions.Add((ActionsAvailable)j);
                    
                    var gsCopy = GameStateRules.Clone(ref gs);

                    Intent currentIntents = new Intent();
                    currentIntents.moveIntent = (ActionsAvailable) i;
                    currentIntents.actionIntent = (ActionsAvailable) j;
                    
                    GameStateRules.Step(ref gsCopy, currentIntents,id);
                    parentNodeChildNode.currentGS = GameStateRules.Clone(ref gsCopy);
                    parentNodeChildNode.sumScore = parentNode.sumScore + gsCopy.players[id].PlayerScore;
                    parentNodeChildNode.nSelect = 1;
                    parentNodeChildNode.depth = 1;
                    parentNodeChildNode.parentNode = new List<MCTSTreeNode>(); 
                    parentNodeChildNode.parentNode.Add(parentNode);
                    parentNodeChildNode.nodeChoosingValue =
                        (float)parentNodeChildNode.sumScore / parentNodeChildNode.nSelect + math.sqrt(2) *
                        math.sqrt(math.log(parentNode.nSelect) / parentNodeChildNode.nSelect);
                    parentNode.childNodes[parentNode.childNodes.Count - 1] = parentNodeChildNode;
                    actions.Clear();
                }
            }
            for (int i = 0; i < iterations; i++)
            {
                nodeMaxChoosingValue.nodeChoosingValue = 0;
                nodeMaxChoosingValue.actions = new List<ActionsAvailable>();
                nodeMaxChoosingValue.actions.Add(ActionsAvailable.NONE);
                nodeMaxChoosingValue.actions.Add(ActionsAvailable.NONE);
                
                nodeMaxChoosingValue = GetMaxChoosingValue(parentNode,nodeMaxChoosingValue);
                
                nodeMaxChoosingValue.nSelect++;
                if (nodeMaxChoosingValue.childNodes == null)
                {
                    nodeMaxChoosingValue.childNodes = new List<MCTSTreeNode>();
                }
                nodeMaxChoosingValue.childNodes.Add(new MCTSTreeNode());
                MCTSTreeNode currentNode = nodeMaxChoosingValue.childNodes[nodeMaxChoosingValue.childNodes.Count-1];
                currentNode.parentNode = new List<MCTSTreeNode>();
                currentNode.parentNode.Add(nodeMaxChoosingValue);
                currentNode.depth = parentNode.depth++;
                currentNode.currentGS = GameStateRules.Clone(ref nodeMaxChoosingValue.currentGS);
                currentNode.actions = new List<ActionsAvailable>();
                currentNode.actions.Add((ActionsAvailable)(nodeMaxChoosingValue.childNodes.Count/(int) ActionsAvailable.NONE));
                currentNode.actions.Add( (ActionsAvailable)(nodeMaxChoosingValue.childNodes.Count%(int) ActionsAvailable.NONE));
                Intent currentIntents = new Intent();
                currentIntents.moveIntent = currentNode.actions[0];
                currentIntents.moveIntent = currentNode.actions[1];
                GameStateRules.Step(ref currentNode.currentGS,currentIntents,id);
                currentNode.sumScore = nodeMaxChoosingValue.sumScore + currentNode.currentGS.players[id].PlayerScore;
                currentNode.nSelect = 1;
                currentNode.nodeChoosingValue =
                    (float)currentNode.sumScore / currentNode.nSelect + math.sqrt(2) *
                    math.sqrt(math.log(nodeMaxChoosingValue.nSelect) / currentNode.nSelect);
                //int currentDepth = currentNode.depth;
                MCTSTreeNode rollbackNode = currentNode.parentNode[0];
                
                while (rollbackNode.depth>0)
                {
                    rollbackNode.nodeChoosingValue =
                        (float)rollbackNode.sumScore / rollbackNode.nSelect + math.sqrt(2) *
                        math.sqrt(math.log(rollbackNode.parentNode[0].nSelect) / rollbackNode.nSelect);
                    rollbackNode = rollbackNode.parentNode[0];
                    //currentDepth = rollbackNode.depth;
                }
            }
            nodeMaxChoosingValue.nodeChoosingValue = 0;
            nodeMaxChoosingValue.actions = new List<ActionsAvailable>();
            nodeMaxChoosingValue.actions.Add(ActionsAvailable.NONE);
            nodeMaxChoosingValue.actions.Add(ActionsAvailable.NONE);
            nodeMaxChoosingValue = GetMaxChoosingValue(parentNode,nodeMaxChoosingValue);
            Intent chosenActions = new Intent();
            chosenActions.moveIntent = nodeMaxChoosingValue.actions[0];
            chosenActions.actionIntent = nodeMaxChoosingValue.actions[1];
            return chosenActions;
        }
        
        private MCTSTreeNode GetMaxChoosingValue(MCTSTreeNode root, MCTSTreeNode newMaxNode)
        {
            MCTSTreeNode currentNode = root;
            if(root.nodeChoosingValue > 0){
                if (root.nodeChoosingValue>newMaxNode.nodeChoosingValue)
                {
                    newMaxNode = root;
                }

                if (currentNode.childNodes!=null)
                {
                    int childrenToCheck = currentNode.childNodes.Count-1;
                    while (childrenToCheck>=0)
                    {
                        newMaxNode = GetMaxChoosingValue(root.childNodes[childrenToCheck],newMaxNode);
                        childrenToCheck--;
                    }
                }
                
            }

            return newMaxNode;
        }
    }
}
