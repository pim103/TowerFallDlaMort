using System.Collections;
using System.Collections.Generic;
using Games.Agents;
using Games.GameState;
using UnityEngine;
    
public struct MCTSTreeNode
{
    public int sumScore;
    public int nSelect;
    public float nodeChoosingValue;
    public List<ActionsAvailable> actions;
    public List<MCTSTreeNode> childNodes;
    public GameStateData currentGS;
    public List<MCTSTreeNode> parentNode;
    public int depth;
}
