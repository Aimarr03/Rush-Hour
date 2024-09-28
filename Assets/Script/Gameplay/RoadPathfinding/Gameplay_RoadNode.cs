using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Gameplay_RoadNode(Vector3Int gridPosition, bool walkable)
    {
        this.gridPosition = gridPosition;
        this.walkable = walkable;
    }
}
