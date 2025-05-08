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

    void Awake() 
    {
        Bounds = plane.GetComponent<Renderer>().bounds.size;
        
        GridSize.x = Mathf.RoundToInt(Bounds.x / nodeDiameter);
        GridSize.y = Mathf.RoundToInt(Bounds.z / nodeDiameter);

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

                grid[i, j] = new Node(i, j, wlk, wop);
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
    public Node GridFromWorldPoint(Vector3 pos)
    {
        int x = Mathf.RoundToInt((pos.x + Bounds.x / 2) / nodeDiameter);
        int y = Mathf.RoundToInt((pos.z + Bounds.z / 2) / nodeDiameter);

        return grid[x, y];
    }

    public List<Node> path;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, Bounds);

        if(grid != null)
        {
            foreach (Node node in grid)
            { 
                Gizmos.color = node.walkable ? Color.white : Color.red;

                if(path != null && path.Contains(node)) Gizmos.color = Color.cyan;

                Vector3 pos = node.position; pos.y = 1;
                Gizmos.DrawCube(pos, Vector3.one * (nodeDiameter - 0.2f));
            }
        }
    }

    public bool InsideBound(Vector3 pos)
    {
        int x = Mathf.RoundToInt((pos.x + Bounds.x / 2) / nodeDiameter);
        int y = Mathf.RoundToInt((pos.z + Bounds.z / 2) / nodeDiameter);

        if (x >= GridSize.x || y >= GridSize.y || x < 0 || y < 0) return false;
        return true;
    }

}
