using UnityEngine;

public class AStarNode
{
    public bool isObstacle;
    public int x, y;
    public Vector2 worldPos;

    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;

    public AStarNode parent;

    public AStarNode(bool obs, int x_, int y_, Vector2 wPos)
    {
        isObstacle = obs;
        x = x_;
        y = y_;
        worldPos = wPos;
    }
}