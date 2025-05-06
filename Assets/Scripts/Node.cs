using UnityEngine;

public class Node 
{
    public Node Parent;
    public bool walkable;

    public Vector3 position;
    public int GridX;
    public int GridY;

    public int gCost;
    public int hCost;

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(int x, int y, bool _wlk, Vector3 _pos)
    {
        GridX = x;
        GridY = y;
        walkable = _wlk;
        position = _pos;
    } 

}
