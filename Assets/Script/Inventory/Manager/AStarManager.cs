using UnityEngine;
using System.Collections.Generic;

public class AStarManager : MonoBehaviour
{
    public static AStarManager Instance;

    [Header("寻路网格设置")]
    public int mapWidth = 80;
    public int mapHeight = 80;
    public float cellSize = 1f;
    public LayerMask obstacleLayer;

    private AStarNode[,] grid;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        GenerateGrid();
    }

    // 生成网格
    void GenerateGrid()
    {
        grid = new AStarNode[mapWidth, mapHeight];
        Vector3 startPos = transform.position - new Vector3(mapWidth * cellSize / 2, mapHeight * cellSize / 2);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 worldPos = startPos + new Vector3(x * cellSize, y * cellSize);
                bool isObstacle = Physics2D.OverlapCircle(worldPos, cellSize * 0.4f, obstacleLayer);
                grid[x, y] = new AStarNode(isObstacle, x, y, worldPos);
            }
        }
    }

    // 对外：获取路径
    public List<AStarNode> FindPath(Vector2 startWorld, Vector2 targetWorld)
    {
        AStarNode start = WorldToNode(startWorld);
        AStarNode target = WorldToNode(targetWorld);

        List<AStarNode> open = new List<AStarNode>();
        HashSet<AStarNode> close = new HashSet<AStarNode>();

        open.Add(start);

        while (open.Count > 0)
        {
            AStarNode cur = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].fCost < cur.fCost || open[i].fCost == cur.fCost && open[i].hCost < cur.hCost)
                    cur = open[i];
            }

            open.Remove(cur);
            close.Add(cur);

            if (cur == target)
                return RetracePath(start, target);

            foreach (AStarNode neighbor in GetNeighbors(cur))
            {
                if (neighbor.isObstacle || close.Contains(neighbor)) continue;

                int newGCost = cur.gCost + GetDistance(cur, neighbor);
                if (newGCost < neighbor.gCost || !open.Contains(neighbor))
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = GetDistance(neighbor, target);
                    neighbor.parent = cur;

                    if (!open.Contains(neighbor)) open.Add(neighbor);
                }
            }
        }
        return null;
    }

    // 回溯路径
    List<AStarNode> RetracePath(AStarNode start, AStarNode end)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode cur = end;
        while (cur != start)
        {
            path.Add(cur);
            cur = cur.parent;
        }
        path.Reverse();
        return path;
    }

    // 工具方法：八方向代价（直线 10，斜线 14），保证对角代价 < 两次直线的 20
    int GetDistance(AStarNode a, AStarNode b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);
        return 14 * min + 10 * (max - min);
    }

    List<AStarNode> GetNeighbors(AStarNode node)
    {
        List<AStarNode> list = new List<AStarNode>(8);
        int x = node.x;
        int y = node.y;

        // 四方向
        if (x + 1 < mapWidth) list.Add(grid[x + 1, y]);
        if (x - 1 >= 0) list.Add(grid[x - 1, y]);
        if (y + 1 < mapHeight) list.Add(grid[x, y + 1]);
        if (y - 1 >= 0) list.Add(grid[x, y - 1]);

        // 四对角：两个直角格都必须是空地才允许斜走，避免斜穿墙角
        for (int dx = -1; dx <= 1; dx += 2)
        {
            for (int dy = -1; dy <= 1; dy += 2)
            {
                int nx = x + dx;
                int ny = y + dy;
                if (nx < 0 || nx >= mapWidth || ny < 0 || ny >= mapHeight) continue;

                if (grid[nx, y].isObstacle || grid[x, ny].isObstacle) continue;

                list.Add(grid[nx, ny]);
            }
        }

        return list;
    }

    AStarNode WorldToNode(Vector2 worldPos)
    {
        float halfW = mapWidth * cellSize / 2f;
        float halfH = mapHeight * cellSize / 2f;

        int x = Mathf.RoundToInt((worldPos.x + halfW) / cellSize);
        int y = Mathf.RoundToInt((worldPos.y + halfH) / cellSize);

        x = Mathf.Clamp(x, 0, mapWidth - 1);
        y = Mathf.Clamp(y, 0, mapHeight - 1);
        return grid[x, y];
    }
}