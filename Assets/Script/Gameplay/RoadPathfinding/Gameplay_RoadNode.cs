using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay_RoadLogic
{
    public class Gameplay_RoadNode
    {
        [Tooltip("The Position of this Tile in the Tilemap")]
        public Vector3Int gridPosition;
        [Tooltip("The Center World Position of this Tile")]
        public Vector3 worldPosition;

        [Tooltip("The Distance between the <b>START NODE</b>to <b>the END NODE</b> with <b>THIS NODE</b> as the connector between them")]
        public int FCost => HCost + GCost;
        [Tooltip("The Distance between the <b>CURRENT NODE</b> to the <b>END NODE</b>")]
        public int HCost;
        [Tooltip("The Distance between the <b>CURRENT NODE</b> to the <b>START NODE</b>")]
        public int GCost;

        [Tooltip("The Connector of <b>THIS NODE</b> to the previous <b>NODE</b>")]
        public Gameplay_RoadNode previousNode;

        public bool walkable;
        
        
        public List<Gameplay_RoadNode> AdjacentConnectedNodes;
        public Enum_RoadNode_Type roadType;
        public int connectedNodeCount = 0;

        public Gameplay_RoadNode(Vector3Int gridPosition, bool walkable)
        {
            this.gridPosition = gridPosition;
            this.walkable = walkable;
            AdjacentConnectedNodes = new List<Gameplay_RoadNode>();
        }
    }
    public enum Enum_RoadNode_Type
    {
        Edge,
        Straight,
        Tri,
        Cross
    }
}
