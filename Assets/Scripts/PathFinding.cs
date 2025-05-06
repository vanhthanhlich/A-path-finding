using UnityEngine;
using System.Collections.Generic;
using System;

public class PathFinding : MonoBehaviour 
{
    private Gridd grid;
    public Transform seeker, target;

    private void Awake()
    {
        grid = GetComponent<Gridd>();
    }

    private void Update()
    {
        FindPath(seeker.position, target.position);
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GridFromWorldPoint(startPos);
        Node targetNode = grid.GridFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node> {startNode};
        HashSet<Node> visited = new HashSet<Node>(); 

        while(openSet.Count > 0)
        {
            Node currentNode = GetMinimumNode(openSet);

            if(currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            openSet.Remove(currentNode);
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

                    if(!openSet.Contains(node)) openSet.Add(node);
                }
            }

        }

    }

    private Node GetMinimumNode(List<Node> Set)
    {
        Node currentNode = null;
        foreach(Node node in Set)
        {
            if(currentNode == null) currentNode = node;

            if(node.fCost <  currentNode.fCost || (node.fCost == currentNode.fCost && node.hCost < currentNode.hCost))
            {
                currentNode = node;
            }
        }

        return currentNode;
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
