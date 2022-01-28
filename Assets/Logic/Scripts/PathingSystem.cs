using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(EXEC_ORDER)]
public class PathingSystem : MonoBehaviour
{
    public const int EXEC_ORDER = 0;

    const int ARENA_WIDTH = 60;
    const int NUM_TILES_TO_PROCESS_PER_BATCH = 500;
    const float BLOCKER_EXTRA_WEIGHT = 1.42f * (ARENA_WIDTH + 1);

    static Vector2Int[] _dirs = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(1, -1)
    };

    static Vector2Int[] _vec2IntCtnr3 = new Vector2Int[3];

#pragma warning disable CS0649
    [SerializeField] Tilemap _pathableTilemap;
    [SerializeField] Tilemap[] _blockingTilemaps;

#if UNITY_EDITOR
    [SerializeField] MonoBehaviour _gizmoVisualisationTarget;
#endif
#pragma warning restore CS0649

    HashSet<Vector2Int> _pathableTiles = new HashSet<Vector2Int>();
    Dictionary<Vector2Int, PathingBlocker> _blockedTiles = new Dictionary<Vector2Int, PathingBlocker>();
    Dictionary<IPathingTarget, PathingData> _pathingData = new Dictionary<IPathingTarget, PathingData>();

    bool _staticsFinalised;
    bool _doCalculationsImmediately;
    bool _recalculateOnNextRequest;

    void Awake()
    {
        _pathableTiles.UnionWith(GetCoordsInTilemap(_pathableTilemap));

        foreach (var tilemap in _blockingTilemaps)
            _pathableTiles.ExceptWith(GetCoordsInTilemap(tilemap));
    }
    
    void OnEnable()
    {
        Refs.I.ps = this;
        GameController.onTick += HandleTick;
    }
    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.ps = null;
        GameController.onTick -= HandleTick;
    }

    public void StaticallyBlockCoord(Vector3 tilePos)
    {
        if (_staticsFinalised)
        {
            Debug.LogError($"Static tile block attempted after finalisation 😮 Ignoring");
            return;
        }

        var coord = WorldPosToCoord(tilePos);

        if (
            !Mathf.Approximately(coord.x, tilePos.x) ||
            !Mathf.Approximately(coord.y, tilePos.y)
        )
        {
            Debug.LogError($"Tile block added with non-integer tilePos (${tilePos}) 😮 Ignoring");
            return;
        }

        _pathableTiles.Remove(coord);
    }

    public void BlockCoord(PathingBlocker blocker)
    {
        var tilePos = blocker.transform.position;
        var coord = WorldPosToCoord(tilePos);

        if (
            !Mathf.Approximately(coord.x, tilePos.x) ||
            !Mathf.Approximately(coord.y, tilePos.y)
        )
        {
            Debug.LogError($"Tile block added with non-integer tilePos (${tilePos}) 😮 Ignoring");
            return;
        }

        if (_blockedTiles.ContainsKey(coord))
        {
            Debug.LogError($"Attempting to block coord ({coord}) which is already blocked 😮 Ignoring");
            return;
        }

        _blockedTiles[coord] = blocker;

        if (_staticsFinalised)
            _recalculateOnNextRequest = true;
    }

    public void UnblockCoord(PathingBlocker blocker)
    {
        var coord = WorldPosToCoord(blocker.transform.position);
        if (!_blockedTiles.ContainsKey(coord))
        {
            Debug.LogError($"Attempting to unblock coord ({coord}) which isn't marked as blocked");
            return;
        }

        if (_blockedTiles[coord] != blocker)
        {
            Debug.LogError($"Attempting to unblock coord ({coord}) but the object already recorded there doesn't match!");
            return;
        }

        _blockedTiles.Remove(coord);

        if (_staticsFinalised)
            _recalculateOnNextRequest = true;
    }

    public PathingBlocker GetBlocker(Vector2Int coord)
    {
        if (_blockedTiles.ContainsKey(coord))
            return _blockedTiles[coord];
        return null;
    }

    public void FinaliseStatics()
    {
        _staticsFinalised = true;
        ImmediatelyCalculateAllPathing();
    }

    public Vector2Int GetNextMoveCoordToTarget(Vector3 curPos, IPathingTarget target)
    {
        if (_recalculateOnNextRequest)
        {
            _recalculateOnNextRequest = false;
            ImmediatelyCalculateAllPathing();
        }

        if (!_pathingData.ContainsKey(target))
            return GetNextCoordDirectlyTowards(curPos, target.PathingTargetPoint);

        var coord = WorldPosToCoord(curPos);
        var data = _pathingData[target].tilePathData;

        if (!data.ContainsKey(coord))
        {
            Debug.LogError($"Requesting move dir from invalid location: {curPos}");
            return GetNextCoordDirectlyTowards(curPos, target.PathingTargetPoint);
        }

        return data[coord].nextPos;
    }

    public Vector2Int WorldPosToCoord(Vector3 worldPos) =>
        new Vector2Int(
            Mathf.RoundToInt(worldPos.x),
            Mathf.RoundToInt(worldPos.y)
        );

    Vector2Int GetNextCoordDirectlyTowards(Vector3 curPos, Vector3 targetPos)
    {
        var dir = targetPos - curPos;
        return WorldPosToCoord(curPos + dir.normalized);
    }

    void HandleTick()
    {
        foreach (var crystal in Refs.I.Crystals)
        {
            if (!_pathingData.ContainsKey(crystal))
            {
                Debug.LogError("Crystal added after pathing data was finalised");
                continue;
            }
        }

        foreach (var pair in _pathingData)
        {
            var target = pair.Key;
            var curCoord = WorldPosToCoord(target.PathingTargetPoint);
            if (_pathingData[target].lastTargetCoord != curCoord)
                RecalculatePathing(target);
        }

        foreach (var pair in _pathingData)
            if (pair.Value.tilesToVisitQueue.Count != 0)
                ProcessSomeTiles(pair.Value);
    }

    IEnumerable<Vector2Int> GetCoordsInTilemap(Tilemap tilemap)
    {
        for (var x = tilemap.cellBounds.xMin; x != tilemap.cellBounds.xMax; ++x)
        {
            for (var y = tilemap.cellBounds.yMin; y != tilemap.cellBounds.yMax; ++y)
            {
                var vec = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(vec))
                    yield return (Vector2Int)vec;
            }
        }
    }

    void ImmediatelyCalculateAllPathing()
    {
        _doCalculationsImmediately = true;

        foreach (var crystal in Refs.I.Crystals)
        {
            if (!_pathingData.ContainsKey(crystal))
                _pathingData[crystal] = new PathingData();
            RecalculatePathing(crystal);
        }
        foreach (var castle in Refs.I.Castles)
        {
            if (!_pathingData.ContainsKey(castle))
                _pathingData[castle] = new PathingData();
            RecalculatePathing(castle);
        }

        _doCalculationsImmediately = false;
    }

    void RecalculatePathing(IPathingTarget obj)
    {
        var pathingData = _pathingData[obj];
        pathingData.visitedTiles.Clear();
        pathingData.tilesToVisitQueue.Clear();
        pathingData.tilesToVisitSet.Clear();
        // Note that we're intentionally NOT clearing `pathingData.tilePathData`, so previously calculated values can still be used

        var coord = WorldPosToCoord(obj.PathingTargetPoint);
        pathingData.lastTargetCoord = coord;
        pathingData.visitedTiles.Add(coord);
        pathingData.tilePathData[coord] = new TilePathData { nextPos = WorldPosToCoord(obj.PathingTargetPoint) }; 

        SetNeighbourTileData(pathingData, coord);
        ProcessSomeTiles(pathingData);
    }

    void SetNeighbourTileData(PathingData pathingData, Vector2Int visitedTileCoord)
    {
        foreach (var dir in _dirs)
        {
            var neighbourCoord = visitedTileCoord + dir;

            if (IsNeighbourBlocked(visitedTileCoord, dir, pathingData.lastTargetCoord))
                continue;

            var neighbourWeight = pathingData.tilePathData[visitedTileCoord].weight + dir.magnitude;
            if (_blockedTiles.ContainsKey(neighbourCoord))
                neighbourWeight += BLOCKER_EXTRA_WEIGHT;

            if (
                (
                    pathingData.tilePathData.ContainsKey(neighbourCoord) &&
                    pathingData.tilePathData[neighbourCoord].weight > neighbourWeight
                ) ||
                (
                    !pathingData.visitedTiles.Contains(neighbourCoord) &&
                    !pathingData.tilesToVisitSet.Contains(neighbourCoord)
                )
            )
            {
                pathingData.tilePathData[neighbourCoord] = new TilePathData
                {
                    weight = neighbourWeight,
                    nextPos = visitedTileCoord
                };
                pathingData.tilesToVisitQueue.Enqueue(neighbourCoord);
                pathingData.tilesToVisitSet.Add(neighbourCoord);
            }
        }
    }

    void ProcessSomeTiles(PathingData pathingData)
    {
        var numToProcess = _doCalculationsImmediately ? int.MaxValue : NUM_TILES_TO_PROCESS_PER_BATCH;
        while (
            numToProcess != 0 &&
            pathingData.tilesToVisitQueue.Count != 0
        )
        {
            var coord = pathingData.tilesToVisitQueue.Dequeue();
            pathingData.tilesToVisitSet.Remove(coord);
            --numToProcess;

            pathingData.visitedTiles.Add(coord);
            SetNeighbourTileData(pathingData, coord);
        }
    }

    bool IsNeighbourBlocked(Vector2Int fromCoord, Vector2Int dir, Vector2Int targetCoord)
    {
        var dirIsDiagonal = dir.x != 0 && dir.y != 0;

        var cellsToCheck = dirIsDiagonal ? 3 : 1;
        _vec2IntCtnr3[0] = fromCoord + dir;

        if (dirIsDiagonal)
        {
            _vec2IntCtnr3[1] = fromCoord + new Vector2Int(dir.x, 0);
            _vec2IntCtnr3[2] = fromCoord + new Vector2Int(0, dir.y);
        }

        for (var i = 0; i != cellsToCheck; ++i)
        {
            var potentiallyBlockingCoord = _vec2IntCtnr3[i];
            if (potentiallyBlockingCoord == targetCoord)
                continue;

            if (
                !_pathableTiles.Contains(potentiallyBlockingCoord) ||
                (
                    _blockedTiles.ContainsKey(potentiallyBlockingCoord) &&
                    (
                        // Can move through blockers (by destroying them first) if need be, only if not diagonal
                        dirIsDiagonal ||
                        
                        !_blockedTiles[potentiallyBlockingCoord].isDestructible
                    )
                )
            ) return true;
        }

        return false;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        PathingData pathingData = null;
        var target = _gizmoVisualisationTarget as IPathingTarget;
        if (
            target != null &&
            _pathingData.ContainsKey(target)
        ) pathingData = _pathingData[target];

        foreach (var coord in _pathableTiles)
        {
            Gizmos.color = _blockedTiles.ContainsKey(coord)
                ? new Color(1f, 0f, 0f, 0.7f)
                : new Color(1f, 1f, 0f, 0.5f);

            Gizmos.DrawLine(
                new Vector3(coord.x, coord.y, 0),
                new Vector3(coord.x + 1, coord.y, 0)
            );
            Gizmos.DrawLine(
                new Vector3(coord.x + 1, coord.y, 0),
                new Vector3(coord.x + 1, coord.y + 1, 0)
            );
            Gizmos.DrawLine(
                new Vector3(coord.x + 1, coord.y + 1, 0),
                new Vector3(coord.x, coord.y + 1, 0)
            );
            Gizmos.DrawLine(
                new Vector3(coord.x, coord.y + 1, 0),
                new Vector3(coord.x, coord.y, 0)
            );

            if (pathingData != null && pathingData.tilePathData.ContainsKey(coord))
            {
                var tileMid = new Vector3(coord.x + 0.5f, coord.y + 0.5f, 0);
                var dir = pathingData.tilePathData[coord].nextPos - coord;

                Gizmos.DrawLine(
                    tileMid,
                    tileMid + new Vector3(dir.x, dir.y, 0) * 0.5f
                );
            }
        }
    }
#endif

    class PathingData
    {
        public Vector2Int lastTargetCoord;
        public HashSet<Vector2Int> visitedTiles = new HashSet<Vector2Int>();
        public Queue<Vector2Int> tilesToVisitQueue = new Queue<Vector2Int>();
        public HashSet<Vector2Int> tilesToVisitSet = new HashSet<Vector2Int>();
        public Dictionary<Vector2Int, TilePathData> tilePathData = new Dictionary<Vector2Int, TilePathData>();
    }

    struct TilePathData
    {
        public float weight;
        public Vector2Int nextPos;
    }
}
