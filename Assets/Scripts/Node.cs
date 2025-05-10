using UnityEngine;

public class Node : HeapItem<Node>
{
    public Node Parent;
    public bool walkable;
    public int movementPenalty;
    private int index;

    public Vector3 worldPosition;
    public int GridX;
    public int GridY;

    public int gCost;
    public int hCost;

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(int x, int y, bool _wlk, Vector3 _pos, int movementPenalty)
    {
        GridX = x;
        GridY = y;
        walkable = _wlk;
        worldPosition = _pos;
        this.movementPenalty = movementPenalty;
    }

    public int Index { 
        get { return index; }
        set { index = value; }
    }

    public int CompareTo(Node other)
    {
        int ans = fCost.CompareTo(other.fCost);
        if (ans == 0)
        {
            ans = hCost.CompareTo(other.hCost);
        }
        return -ans;
    }

}
