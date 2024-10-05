using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;


namespace Gameplay_RoadLogic
{
    public class Manager_RoadPathfinding : MonoBehaviour
    {
        [SerializeField] private Transform startPos;
        [SerializeField] private Transform endPos;
        //private float currentDuration = 0f;
        public Tilemap tilemap;

        public List<Gameplay_RoadNode> OpenList;
        public List<Gameplay_RoadNode> CloseList;

        public List<Gameplay_RoadNode> EdgeTilesList;

        private Dictionary<Vector3Int, Gameplay_RoadNode> allTiles;
        private List<Gameplay_RoadNode> currentPath;



        private Gameplay_RoadNode startNode, endNode;

        public static Manager_RoadPathfinding instance;

        private void Awake()
        {

        }
        void Start()
        {
            Initialized();
            //currentPath = SetPathFromWorldPosition(testStartPos, testEndPos);
        }
        private void Update()
        {
            /*currentDuration += Time.deltaTime;
            if (currentDuration > 1)
            {
                currentDuration = 0;
                currentPath = SetPathFromWorldPosition(startPos.position, endPos.position);
            }*/

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
        #region Initializing
        public void Initialized()
        {
            OpenList = new List<Gameplay_RoadNode>();
            CloseList = new List<Gameplay_RoadNode>();
            allTiles = new Dictionary<Vector3Int, Gameplay_RoadNode>();
            EdgeTilesList = new List<Gameplay_RoadNode>();

            /*Debug.Log(tilemap.cellBounds);
            Debug.Log(tilemap.cellBounds.allPositionsWithin);*/

            BoundsInt boundaryTilemap = tilemap.cellBounds;

            /*Debug.Log($"XMin = {boundaryTilemap.xMin}");
            Debug.Log($"XMax= {boundaryTilemap.xMax}");
            Debug.Log($"YMin = {boundaryTilemap.yMin}");
            Debug.Log($"YMax= {boundaryTilemap.yMax}");*/
            
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
            List<Gameplay_RoadNode> List_IntersectedNodes = new List<Gameplay_RoadNode>();
            
            foreach (Gameplay_RoadNode currentTile in allTiles.Values)
            {
                SetTypeForNode(currentTile);
                switch (currentTile.roadType)
                {
                    case Enum_RoadNode_Type.Edge: 
                        EdgeTilesList.Add(currentTile);
                        break;
                    case Enum_RoadNode_Type.Tri:
                        List_IntersectedNodes.Add(currentTile);
                        break;
                    case Enum_RoadNode_Type.Cross:
                        List_IntersectedNodes.Add(currentTile);
                        break;
                }
            }
            /*Debug.Log("Engage to Find Edge from the ROAD");*/
            foreach(Gameplay_RoadNode gameplay_RoadNode in List_IntersectedNodes)
            {
                foreach(Gameplay_RoadNode CurrentDirection_RoadNode in gameplay_RoadNode.AdjacentConnectedNodes)
                {
                    Debug.Log($"Start Position of Edge grid position {CurrentDirection_RoadNode.gridPosition} world position {CurrentDirection_RoadNode.worldPosition}");
                    /*Debug.Log($"Center Position is grid position {gameplay_RoadNode.gridPosition} world position {gameplay_RoadNode.worldPosition}");*/
                    Vector3Int CurrentDirection = CurrentDirection_RoadNode.gridPosition - gameplay_RoadNode.gridPosition;
                    /*Debug.Log($"Current Direction is {CurrentDirection}"); */
                    int multiplier = 1;
                    Gameplay_RoadNode NextNode = null;
                    if (CurrentDirection == Vector3.zero) continue;
                    do
                    {
                        Vector3Int NextPosition = CurrentDirection_RoadNode.gridPosition + (CurrentDirection * multiplier);
                        Debug.Log($"Next Position is {NextPosition}");
                        NextNode = allTiles[NextPosition];
                        
                        multiplier++;
                        if(NextNode == null) break;
                        if(NextNode.roadType != Enum_RoadNode_Type.Straight)
                        {
                            /*Debug.Log($"The Neighbor is the end node for edge at grid position {NextNode.gridPosition} world position {NextNode.worldPosition}");
                            Debug.Log($"The Length of Edge is {multiplier}");*/
                            break;
                        }
                        else
                        {
                            //Debug.Log($"The Neighbor is on the grid position of {NextNode.gridPosition} world position {NextNode.worldPosition}");
                        }
                    } while (NextNode.roadType == Enum_RoadNode_Type.Straight);
                    
                }
            }
        }
        /*private bool IsEdgeNode(Vector3Int Pos)
        {
            int currentConnection = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0) continue;
                Vector3Int neighborPos = Pos + new Vector3Int(dx, 0);
                if (allTiles.ContainsKey(neighborPos)) currentConnection++;
                if (currentConnection >= 2) return false;
            }
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dy == 0) continue;
                Vector3Int neighborPos = Pos + new Vector3Int(0, dy);
                if (allTiles.ContainsKey(neighborPos)) currentConnection++;
                if (currentConnection >= 2) return false;
            }
            return currentConnection == 1;
        }
        private void FindEdges(Gameplay_RoadNode firstNode)
        {

        }*/
        private void SetTypeForNode(Gameplay_RoadNode node)
        {
            Vector3Int currentGridPos = node.gridPosition;
            int connectedNode = 0;
            for(int dx = -1; dx <= 1; dx ++)
            {
                if (dx == 0) continue;
                Vector3Int neighborPos = currentGridPos + new Vector3Int(dx, 0);
                if (allTiles.TryGetValue(neighborPos, out Gameplay_RoadNode neighborNode))
                {
                    connectedNode++;
                    node.AdjacentConnectedNodes.Add(neighborNode);
                }
            }
            for (int dy = -1; dy <= 1; dy ++)
            {
                if(dy == 0) continue;
                Vector3Int neighborPos = currentGridPos + new Vector3Int(0, dy);
                if (allTiles.TryGetValue(neighborPos, out Gameplay_RoadNode neighborNode))
                {
                    connectedNode++;
                    node.AdjacentConnectedNodes.Add(neighborNode);
                }
            }
            switch (connectedNode)
            {
                case 1: 
                    node.roadType = Enum_RoadNode_Type.Edge;
                    break;
                case 2:
                    node.roadType = Enum_RoadNode_Type.Straight;
                    break;
                case 3:
                    node.roadType = Enum_RoadNode_Type.Tri;
                    break;
                case 4:
                    node.roadType = Enum_RoadNode_Type.Cross;
                    break;
            }
            node.connectedNodeCount = connectedNode;
            Debug.Log($"Node For Grid Pos {node.gridPosition} World Pos {node.worldPosition} is a {node.roadType} with {connectedNode} connected nodes");
        }
        #endregion
        #region PathFinding Algorithm

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

                foreach (var neighbor in currentNode.AdjacentConnectedNodes)
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
        /*private List<Gameplay_RoadNode> GetNeighbours(Gameplay_RoadNode currentNode)
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
        }*/
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
            //Debug.Log("Path Found");
            return path;
        }
        #endregion
    }
}
