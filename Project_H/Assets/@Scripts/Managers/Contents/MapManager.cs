using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Define;

public class MapManager
{
    public GameObject Map { get; private set; }
    public string MapName { get; private set; }
    public Grid CellGrid { get; private set; }

    // (CellPos, BaseObject)
    Dictionary<Vector3Int, BaseObject> _cells = new Dictionary<Vector3Int, BaseObject>();
    //public StageTransition StageTransition;

    private int MinX;
    private int MaxX;
    private int MinY;
    private int MaxY;

    public Vector3Int World2Cell(Vector3 worldPos) { return CellGrid.WorldToCell(worldPos); }
    public Vector3 Cell2World(Vector3Int cellPos) { return CellGrid.CellToWorld(cellPos); }

    ECellCollisionType[,] _collision;

    public void LoadMap(string mapName)
    {
        DestroyMap();

        GameObject map = Managers.Resource.Instantiate(mapName);
        map.transform.position = Vector3.zero;
        map.name = $"@Map_{mapName}";

        Map = map;
        MapName = mapName;
        CellGrid = map.GetComponent<Grid>();

        ParseCollisionData(map, mapName);
    }

    public void DestroyMap()
    {
        ClearObjects();

        if (Map != null)
            Managers.Resource.Destroy(Map);
    }

    void ParseCollisionData(GameObject map, string mapName, string tilemap = "Tilemap_Collision")
    {
        GameObject collision = Util.FindChild(map, tilemap, true);
        if (collision != null)
            collision.SetActive(false);

        // Collision정보 생성
        TextAsset txt = Managers.Resource.Load<TextAsset>($"{mapName}Collision");
        StringReader reader = new StringReader(txt.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX + 1;
        int yCount = MaxY - MinY + 1;
        _collision = new ECellCollisionType[xCount, yCount];

        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; x++)
            {
                switch (line[x])
                {
                    case Define.MAP_TOOL_WALL:
                        _collision[x, y] = ECellCollisionType.Wall;
                        break;
                    case Define.MAP_TOOL_NONE:
                        _collision[x, y] = ECellCollisionType.None;
                        break;
                    case Define.MAP_TOOL_SEMI_WALL:
                        _collision[x, y] = ECellCollisionType.SemiWall;
                        break;
                }
            }
        }
    }

    public void ClearObjects()
    {
        _cells.Clear();
    }

    public BaseObject GetObject(Vector3 worldPos)
    {
        Vector3Int cellPos = World2Cell(worldPos);
        return GetObject(cellPos);
    }
    public BaseObject GetObject(Vector3Int cellPos)
    {
        // 없으면 null
        _cells.TryGetValue(cellPos, out BaseObject value);
        return value;
    }

    // MoveTo
    // CanGo > RemoveObject > AddObject > SetCellPos(이동)
    public bool MoveTo(Creature obj, Vector3Int cellPos, bool forceMove = false)
    {
        if (CanGo(obj, cellPos) == false)
            return false;

        RemoveObject(obj);

        AddObject(obj, cellPos);

        obj.SetCellPos(cellPos, forceMove);

        return true;
    }

    #region Helpers
    public bool CanGo(BaseObject self, Vector3 worldPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
    {
        return CanGo(self, World2Cell(worldPos), ignoreObjects, ignoreSemiWall);
    }

    public bool CanGo(BaseObject self, Vector3Int cellPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
    {
        int extraCells = 0;
        if (self != null)
            extraCells = self.ExtraCells;

        for (int dx = -extraCells; dx <= extraCells; dx++)
        {
            for (int dy = -extraCells; dy <= extraCells; dy++)
            {
                Vector3Int checkPos = new Vector3Int(cellPos.x + dx, cellPos.y + dy);

                if (CanGo_Internal(self, cellPos, ignoreObjects, ignoreSemiWall) == false)
                    return false;
            }
        }

        return true;
    }

    bool CanGo_Internal(BaseObject self, Vector3Int cellPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        if (ignoreObjects == false)
        {
            BaseObject obj = GetObject(cellPos);
            if (obj != null && obj != self)
                return false;
        }

        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y;
        ECellCollisionType type = _collision[x, y];
        if (type == ECellCollisionType.None)
            return true;

        if (ignoreSemiWall && type == ECellCollisionType.SemiWall)
            return true;

        return false;
    }

    void RemoveObject(BaseObject obj)
    {
        int extraCells = 0;
        if (obj != null)
            extraCells = obj.ExtraCells;

        Vector3Int cellPos = obj.CellPos;

        for (int dx = -extraCells; dx <= extraCells; dx++)
        {
            for (int dy = -extraCells; dy <= extraCells; dy++)
            {
                Vector3Int newCellPos = new Vector3Int(cellPos.x + dx, cellPos.y + dy);
                BaseObject prev = GetObject(newCellPos);

                if (prev == obj)
                    _cells[newCellPos] = null;
            }
        }
    }

    void AddObject(BaseObject obj, Vector3Int cellPos)
    {
        int extraCells = 0;
        if (obj != null)
            extraCells = obj.ExtraCells;

        for (int dx = -extraCells; dx <= extraCells; dx++)
        {
            for (int dy = -extraCells; dy <= extraCells; dy++)
            {
                Vector3Int newCellPos = new Vector3Int(cellPos.x + dx, cellPos.y + dy);
                BaseObject prev = GetObject(newCellPos);
                if (prev != null && prev != obj)
                    Debug.LogWarning($"AddObject 덮어씌워짐");

                _cells[newCellPos] = obj;
            }
        }
    }
    #endregion

    #region A* Pathfinding
    public struct PQNode : IComparable<PQNode>
    {
        public int H;
        public Vector3Int CellPos;
        public int Depth;

        public int CompareTo(PQNode other)
        {
            if (H == other.H)
                return 0;
            return H < other.H ? 1 : -1;
        }
    }

    List<Vector3Int> _delta = new List<Vector3Int>()
    {
        new Vector3Int(0, 1, 0), // U
		new Vector3Int(1, 1, 0), // UR
		new Vector3Int(1, 0, 0), // R
		new Vector3Int(1, -1, 0), // DR
		new Vector3Int(0, -1, 0), // D
		new Vector3Int(-1, -1, 0), // LD
		new Vector3Int(-1, 0, 0), // L
		new Vector3Int(-1, 1, 0), // LU
	};

    public List<Vector3Int> FindPath(BaseObject self, Vector3Int startCellPos, Vector3Int destCellPos, int maxDepth = 10)
    {
        // 후보들 목록
        Dictionary<Vector3Int, int> best = new Dictionary<Vector3Int, int>();

        // 경로 추적용
        Dictionary<Vector3Int, Vector3Int> parent = new Dictionary<Vector3Int, Vector3Int>();

        // 가중치 높은 후보 뽑기, 효율적으로 관리(+ OpenList)
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        Vector3Int pos = startCellPos;
        Vector3Int dest = destCellPos;

        // destCellPos에 도착 못하면 가장 가까운 곳
        Vector3Int closestCellPos = startCellPos;
        int closestH = (dest - pos).sqrMagnitude;

        // 시작점 부터 등록
        {
            int h = (dest - pos).sqrMagnitude;
            pq.Push(new PQNode() { H = h, CellPos = pos, Depth = 1 });
            parent[pos] = pos;
            best[pos] = h;
        }

        while (pq.Count > 0)
        {
            PQNode node = pq.Pop();
            pos = node.CellPos;

            // 목적지 도착, 종료
            if (pos == dest)
                break;

            // 설정한 깊이만큼 조사(너무 길면 게임 느려짐)
            if (node.Depth >= maxDepth)
                break;

            // 상하좌우 확인하여 탐색
            foreach (Vector3Int delta in _delta)
            {
                Vector3Int next = pos + delta;

                if (CanGo(self, next) == false)
                    continue;

                int h = (dest - next).sqrMagnitude;

                // 더 좋은 후보 있나 탐색
                if (best.ContainsKey(next) == false)
                    best[next] = int.MaxValue;

                // 이미 더 좋은 경로 있으면 스킵
                if (best[next] <= h)
                    continue;

                // 더 좋은 경로라면 갱신
                best[next] = h;

                pq.Push(new PQNode() { H = h, CellPos=next, Depth = node.Depth + 1 });
                parent[next] = pos;

                // 목적지까지는 못 가더라도, 제일 좋은 후보 기억해두기
                if (closestH > h)
                {
                    closestH = h;
                    closestCellPos = next;
                }
            }
        }

        // 제일 가까운 노드라도 찾아서 가도록
        if (parent.ContainsKey(dest) == false)
            return CalcCellPathFromParent(parent, closestCellPos);
        return CalcCellPathFromParent(parent, dest);
    }

    List<Vector3Int> CalcCellPathFromParent(Dictionary<Vector3Int, Vector3Int> parent, Vector3Int dest)
    {
        List<Vector3Int> cells = new List<Vector3Int>();

        if (parent.ContainsKey(dest) == false)
            return cells;

        Vector3Int now = dest;

        while (parent[now] != now)
        {
            cells.Add(now);
            now = parent[now];
        }

        cells.Add(now);
        cells.Reverse();

        return cells;
    }
    #endregion
}
