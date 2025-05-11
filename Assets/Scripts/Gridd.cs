using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System;
using Unity.Jobs;
using System.Net.Sockets;
using UnityEngine.Assertions.Must;
using System.Net.NetworkInformation;

public class Gridd : MonoBehaviour
{

    [SerializeField] private Transform plane;
    [SerializeField] private LayerMask layerMask;

    private Vector2Int GridSize;
    private Vector3 Bounds;
    private Node[,] grid;
    private float nodeRadius = 0.5f;
    private float nodeDiameter { get { return 2 * nodeRadius; } } 

    public int MaxSize {  get { return GridSize.x * GridSize.y; } }

    public TerrainType[] walkableRegion;
    private LayerMask walkableLayer;
    Dictionary<int, int> walkableRegionDictionary = new Dictionary<int, int>();

    private int MinPenalty, MaxPenalty;
    public int obstacalePenalty = 10;
    [SerializeField] public int blurSize;

    void Awake() 
    {
        MinPenalty = int.MaxValue;
        MaxPenalty = int.MinValue;

        Bounds = plane.GetComponent<Renderer>().bounds.size;
        
        GridSize.x = Mathf.RoundToInt(Bounds.x / nodeDiameter);
        GridSize.y = Mathf.RoundToInt(Bounds.z / nodeDiameter);

        foreach(var region in  walkableRegion)
        {
            walkableLayer.value |= region.terrainMask.value;
            walkableRegionDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }
 
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[GridSize.x, GridSize.y];

        Vector2 bottomleft = new Vector2(-Bounds.x / 2, -Bounds.z / 2);
        for (int i = 0; i < GridSize.x; i++)
        {
            for (int j = 0; j < GridSize.y; j++)
            {

                float posx = bottomleft.x + i * nodeDiameter + nodeRadius;
                float posy = bottomleft.y + j * nodeDiameter + nodeRadius;
                Vector3 wop = new Vector3(posx, 0, posy);

                bool wlk = !Physics.CheckSphere(wop, nodeRadius, layerMask);

                int movementPenalty = 0;
                if(wlk)
                {
                    Ray ray = new Ray(wop + Vector3.up * 50, Vector3.down);
                    if(Physics.Raycast(ray, out RaycastHit hit, 100))
                    {
                        walkableRegionDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }
                }
                else movementPenalty = obstacalePenalty;
                Debug.Log(movementPenalty);
                grid[i, j] = new Node(i, j, wlk, wop, movementPenalty);
            }
        }
        BlurMovementPenalty(blurSize);
    }

    private void BlurMovementPenalty(int blursize)
    {        
        int[,] blurRow = new int[GridSize.x, GridSize.y];
        int[,] blurCol = new int[GridSize.x, GridSize.y];

        for(int i = 0; i < GridSize.x; i++) 
        {
            // calculate for j = 0
            for(int j = -blursize; j <= blursize; j ++) 
            {
                int y = Mathf.Clamp(j, 0, GridSize.y - 1);
                blurRow[i, 0] += grid[i, y].movementPenalty;
            }

            // calculate the rest
            for(int j = 1; j < GridSize.y; j ++) {
                int delColumnIndex = Mathf.Clamp(j - blursize - 1, 0, GridSize.y - 1);
                int addColumnIndex = Mathf.Clamp(j + blursize, 0, GridSize.y - 1);

                int addValue = grid[i, addColumnIndex].movementPenalty;
                int delValue = grid[i, delColumnIndex].movementPenalty;

                blurRow[i, j] = blurRow[i, j - 1] + addValue - delValue;
            }
        }

        for(int j = 0; j < GridSize.y; j ++) 
        {
            for(int i = -blursize; i <= blursize; i++) {
                int x = Mathf.Clamp(i, 0, GridSize.x - 1);
                blurCol[0, j] += blurRow[x, j];
            }

            for(int i = 1; i < GridSize.x; i ++) 
            {
                int delRowIndex = Mathf.Clamp(i - blursize - 1, 0, GridSize.x - 1);
                int addRowIndex = Mathf.Clamp(i + blursize, 0, GridSize.x - 1);

                int delValue = blurRow[delRowIndex, j];
                int addValue = blurRow[addRowIndex, j];

                blurCol[i, j] = blurCol[i - 1, j] + addValue - delValue;
            }
        }

        int squareSize = 2 * blursize + 1;
        for(int i = 0; i < GridSize.x; i ++) {
            for(int j = 0; j < GridSize.y; j ++) {
                grid[i, j].movementPenalty = Mathf.RoundToInt((float)blurCol[i, j] / (squareSize * squareSize));

                MinPenalty = Mathf.Min(MinPenalty, grid[i, j].movementPenalty);
                MaxPenalty = Mathf.Max(MaxPenalty, grid[i, j].movementPenalty);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        int x = node.GridX, y = node.GridY;

        bool inside(int i, int j) => 0 <= i && i < GridSize.x && 0 <= j && j < GridSize.y;

        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if (inside(x + i, y + j)) neighbours.Add(grid[x + i, y + j]);
            }
        }

        return neighbours;
    }
  
    public Node GridFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + Bounds.x / 2) / Bounds.x;
        float percentY = (worldPosition.z + Bounds.z / 2) / Bounds.z;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((GridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((GridSize.y - 1) * percentY);
        return grid[x, y];
    }

    public bool InsideBound(Vector3 pos)
    {
        float percentX = (pos.x + Bounds.x / 2) / Bounds.x;
        float percentY = (pos.z + Bounds.z / 2) / Bounds.z;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((GridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((GridSize.y - 1) * percentY);

        if (x >= GridSize.x || y >= GridSize.y || x < 0 || y < 0) return false;
        return true;
    }
    
    [Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }

    public bool displayGridGizmos = false;
    void OnDrawGizmos()
    {
        if(grid == null || !displayGridGizmos) return;
        foreach(var n in grid) {
            	
                Gizmos.color = Color.Lerp (Color.white, Color.black, Mathf.InverseLerp (MinPenalty, MaxPenalty, n.movementPenalty));
				Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
        }
    }

}
