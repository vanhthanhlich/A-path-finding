using UnityEngine;
using System.Collections.Generic;
using System;

public class PathFinding : MonoBehaviour 
{
    private Gridd grid;
    public Transform seeker, target;
    bool success = true;

    private void Awake()
    {
        grid = GetComponent<Gridd>();
    }

    private void Update()
    {
        if(success && grid.InsideBound(seeker.position) && grid.InsideBound(target.position))
        {
            FindPath(seeker.position, target.position);
        }
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {


        success = false;
        Node startNode = grid.GridFromWorldPoint(startPos);
        Node targetNode = grid.GridFromWorldPoint(targetPos);

        if(!targetNode.walkable || !startNode.walkable)
        {
            success = true;
            return;
        }

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);

        HashSet<Node> visited = new HashSet<Node>();


        openSet.Add(startNode);
        while(openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();

            if(currentNode == targetNode)
            {
                success = true;
                RetracePath(startNode, targetNode);
                return;
            }

            visited.Add(currentNode);

            foreach (Node node in grid.GetNeighbours(currentNode))
            { 
                if (!node.walkable || visited.Contains(node)) continue;
                
                int newPath = currentNode.gCost + GetDistance(currentNode, node);
                if(newPath < node.gCost || !openSet.Contains(node))
                {
                    node.gCost = newPath;
                    node.hCost = GetDistance(node, targetNode);
                    node.Parent = currentNode;

                    openSet.Add(node);
                }
            }

        }

    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        while(startNode != endNode)
        {
            path.Add(endNode);
            endNode = endNode.Parent;
        }
        path.Add(startNode);

        path.Reverse();

        grid.path = path;
    }

    private int GetDistance(Node A, Node B)
    {
        int dx = Mathf.Abs(A.GridX - B.GridX);
        int dy = Mathf.Abs(A.GridY - B.GridY);

        if (dx > dy) (dx, dy) = (dy, dx);
        return  14 * dx + 10 * (dy - dx);
    }

}
