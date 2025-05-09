using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEditor.Callbacks;

public class PathFinding : MonoBehaviour 
{
    private PathRequestManager requestManager;
    private Gridd grid;
<<<<<<< HEAD
=======
    public Transform seeker, target;
    bool success = true;
>>>>>>> origin

    private void Awake()
    {
        grid = GetComponent<Gridd>();
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
<<<<<<< HEAD
        StartCoroutine(FindPath(startPos, targetPos));
=======
        if(success && grid.InsideBound(seeker.position) && grid.InsideBound(target.position))
        {
            FindPath(seeker.position, target.position);
        }
>>>>>>> origin
    }
    private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {


        success = false;
        Node startNode = grid.GridFromWorldPoint(startPos);
        Node targetNode = grid.GridFromWorldPoint(targetPos);

<<<<<<< HEAD
        bool success = TryFindPath(startPos, targetPos);
        Vector3[] waypoints = new Vector3[0];
        yield return null;

        if (success) {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishProcessPath(waypoints, success);
    }

    private bool TryFindPath(Vector3 startPos, Vector3 targetPos)
    {

        Node startNode = grid.GridFromWorldPoint(startPos);
        Node targetNode = grid.GridFromWorldPoint(targetPos);

        if(!startNode.walkable || !targetNode.walkable || !grid.InsideBound(startPos) || !grid.InsideBound(targetPos))
        {
            return false;
        }

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> visited = new HashSet<Node>();


        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();

            if (currentNode == targetNode) return true;
=======
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
>>>>>>> origin

            visited.Add(currentNode);

            foreach (Node node in grid.GetNeighbours(currentNode))
            {
                if (!node.walkable || visited.Contains(node)) continue;

                int newPath = currentNode.gCost + GetDistance(currentNode, node);
                if (newPath < node.gCost || !openSet.Contains(node))
                {
                    node.gCost = newPath;
                    node.hCost = GetDistance(node, targetNode);
                    node.Parent = currentNode;

                    openSet.Add(node);
                }
            }
        }

        return false;
    }

<<<<<<< HEAD
    private Vector3[] RetracePath(Node startNode, Node endNode)
=======
    void RetracePath(Node startNode, Node endNode)
>>>>>>> origin
    {
        List<Node> path = new List<Node>();
        while(startNode != endNode)
        {
            path.Add(endNode);
            endNode = endNode.Parent;
        }
        path.Add(startNode);

        path.Reverse();

        return SimplifyPath(path);
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> spath = new List<Vector3>() ;
        
        Vector2 oldDirection = Vector2.zero;
        for(int i = 1; i < path.Count; i++)
        {
            Vector2 newDirection = new Vector2(path[i].GridX - path[i - 1].GridX, path[i].GridY - path[i - 1].GridY);
            if (oldDirection != newDirection)
            {
                spath.Add(path[i].worldPosition);
                oldDirection = newDirection;
            }
        }
        if (path.Count > 0 && spath.Count > 0 && spath[spath.Count - 1] != path[path.Count - 1].worldPosition) spath.Add(path[path.Count - 1].worldPosition);

        for (int i = 0; i < spath.Count; i ++)
        {
            Vector3 pos = spath[i]; pos.y = 1;
            spath[i] = pos;
        }
        return spath.ToArray();
    }


    private int GetDistance(Node A, Node B)
    {
        int dx = Mathf.Abs(A.GridX - B.GridX);
        int dy = Mathf.Abs(A.GridY - B.GridY);

        if (dx > dy) (dx, dy) = (dy, dx);
        return  14 * dx + 10 * (dy - dx);
    }

}
