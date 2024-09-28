using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Manager_RoadPathfinding : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    private float currentDuration = 0f;
    public Tilemap tilemap;

    public List<Gameplay_RoadNode> OpenList;
    public List<Gameplay_RoadNode> CloseList;
    private Dictionary<Vector3Int, Gameplay_RoadNode> allTiles;
    private List<Gameplay_RoadNode> currentPath;
    [SerializeField] private Vector3 testStartPos = new Vector3(7.63f, 2.54f, 0);
    [SerializeField] private Vector3 testEndPos = new Vector3(-8.10f, 2.92f, 0);

    private Gameplay_RoadNode startNode, endNode;
    public void Initialized()
    {
        OpenList = new List<Gameplay_RoadNode>();
        CloseList = new List<Gameplay_RoadNode>();    
        allTiles = new Dictionary<Vector3Int, Gameplay_RoadNode>();
        /*Debug.Log(tilemap.cellBounds);
        Debug.Log(tilemap.cellBounds.allPositionsWithin);*/
        BoundsInt boundaryTilemap = tilemap.cellBounds;
        for (int X_Index = boundaryTilemap.xMin; X_Index < boundaryTilemap.xMax; X_Index++)
        {
            for (int Y_Index = boundaryTilemap.yMin; Y_Index < boundaryTilemap.yMax; Y_Index++)
            {
                Vector3Int gridPosition = new Vector3Int(X_Index, Y_Index, 0);
                TileBase currentTile = tilemap.GetTile(gridPosition);
                if (currentTile == null) continue;
                Gameplay_RoadNode newTile = new Gameplay_RoadNode(gridPosition, true);
                newTile.worldPosition = tilemap.GetCellCenterLocal(gridPosition);
                allTiles.Add(gridPosition, newTile);
            }
        }
        foreach (Gameplay_RoadNode currentTile in allTiles.Values)
        {
            Debug.Log("Current Tile in " + currentTile.gridPosition);
        }
    }
    public List<Gameplay_RoadNode> SetPathFromWorldPosition(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        Vector3Int startPos = tilemap.WorldToCell(startWorldPos);
        Vector3Int endPos = tilemap.WorldToCell(endWorldPos);

        startNode = allTiles[startPos];
        endNode = allTiles[endPos];
        return CalculatePath();
    }
    public List<Gameplay_RoadNode> CalculatePath()
    {
        if (startNode == null || endNode == null) return null;

        OpenList.Add(startNode);
        while (OpenList.Count > 0)
        {
            Gameplay_RoadNode currentNode = OpenList[0];
            for (int index = 1; index < OpenList.Count; index++)
            {
                Gameplay_RoadNode newNode = OpenList[index];
                if (newNode.FCost <= currentNode.FCost && newNode.HCost < currentNode.HCost)
                {
                    currentNode = newNode;
                }
            }

            OpenList.Remove(currentNode);
            CloseList.Add(currentNode);

            if (currentNode == endNode)
            {
                OpenList.Clear();
                CloseList.Clear();
                return RetracePath(startNode, endNode);
            }

            foreach (var neighbor in GetNeighbours(currentNode))
            {
                if (!neighbor.walkable || CloseList.Contains(neighbor)) continue;
                int newDistanceFromCurrentNodeToNeighbor = currentNode.GCost + GetDistanceBetweenNode(currentNode, neighbor);
                if (newDistanceFromCurrentNodeToNeighbor < neighbor.GCost || !OpenList.Contains(neighbor))
                {
                    neighbor.GCost = newDistanceFromCurrentNodeToNeighbor;
                    neighbor.HCost = GetDistanceBetweenNode(neighbor, endNode);
                    neighbor.previousNode = currentNode;
                    if (!OpenList.Contains(neighbor))
                    {
                        OpenList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }
    private List<Gameplay_RoadNode> GetNeighbours(Gameplay_RoadNode currentNode)
    {
        List<Gameplay_RoadNode> neighbours = new List<Gameplay_RoadNode>();
        Vector3Int currentNodePos = currentNode.gridPosition;

        for (int Index_X = -1; Index_X <= 1; Index_X++)
        {
            for (int Index_Y = -1; Index_Y <= 1; Index_Y++)
            {
                Vector3Int offSet = new Vector3Int(Index_X, Index_Y, 0);
                Vector3Int neighborPos = currentNodePos + offSet;

                Gameplay_RoadNode newNeighbourNode;

                if (allTiles.TryGetValue(neighborPos, out newNeighbourNode))
                {
                    neighbours.Add(newNeighbourNode);
                }

            }
        }

        return neighbours;

    }
    private int GetDistanceBetweenNode(Gameplay_RoadNode startNode, Gameplay_RoadNode endNode)
    {
        Vector3Int startNodePos = startNode.gridPosition;
        Vector3Int endNodePos = endNode.gridPosition;
        int Dis_X = Mathf.Abs(startNodePos.x - endNodePos.x);
        int Dis_Y = Mathf.Abs(startNodePos.y - endNodePos.y);
        if (Dis_X > Dis_Y) return Dis_Y * 14 + Dis_X * 10;
        else return Dis_X * 14 + Dis_Y * 10;
    }
    private List<Gameplay_RoadNode> RetracePath(Gameplay_RoadNode startNode, Gameplay_RoadNode endNode)
    {
        List<Gameplay_RoadNode> path = new List<Gameplay_RoadNode>();
        Gameplay_RoadNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.previousNode;
            //Debug.Log(currentNode.gridPosition);
        }
        path.Reverse();
        Debug.Log("Path Found");
        return path;
    }
    void Start()
    {
        Initialized();
        //currentPath = SetPathFromWorldPosition(testStartPos, testEndPos);
    }
    private void Update()
    {
        currentDuration += Time.deltaTime;
        if(currentDuration > 1)
        {
            currentDuration = 0;
             currentPath = SetPathFromWorldPosition(startPos.position, endPos.position);
        }

    }
    private void OnDrawGizmos()
    {
        if (currentPath != null)
        {
            Gizmos.color = Color.red;
            for (int index = 0; index < currentPath.Count - 1; index++)
            {
                Gameplay_RoadNode currentNode = currentPath[index];
                Gameplay_RoadNode nextNode = currentPath[index + 1];
                if (nextNode == null) break;
                Gizmos.DrawLine(currentNode.worldPosition, nextNode.worldPosition);
            }
        }
    }
}
