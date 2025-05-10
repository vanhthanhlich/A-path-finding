using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

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

    void Awake() 
    {
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

                grid[i, j] = new Node(i, j, wlk, wop, movementPenalty);
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
    
    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }

}
