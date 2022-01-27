using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(EXEC_ORDER)]
public class PathingSystem : MonoBehaviour
{
    public const int EXEC_ORDER = 0;

    const int NUM_TILES_TO_PROCESS_PER_BATCH = 50;

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

#pragma warning disable CS0649
    [SerializeField] Tilemap _pathableTilemap;
    [SerializeField] Tilemap[] _blockingTilemaps;

#if UNITY_EDITOR
    [SerializeField] MonoBehaviour _gizmoVisualisationTarget;
#endif
#pragma warning restore CS0649

    HashSet<Vector2Int> _pathableTiles = new HashSet<Vector2Int>();
    Dictionary<IPathingTarget, PathingData> _pathingData = new Dictionary<IPathingTarget, PathingData>();

    bool _staticsFinalised;
    bool _doCalculationsImmediately;

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

    public void RemoveCoordFromPathingGrid(Vector3 tilePos)
    {
        if (_staticsFinalised)
        {
            Debug.LogError($"Static tile exception attempted after finalisation 😮 Ignoring");
            return;
        }

        var coord = WorldPosToCoord(tilePos);

        if (
            !Mathf.Approximately(coord.x, tilePos.x) ||
            !Mathf.Approximately(coord.y, tilePos.y)
        )
        {
            Debug.LogError($"Static tile exception added with non-integer tilePos (${tilePos}) 😮 Ignoring");
            return;
        }

        _pathableTiles.Remove(coord);
    }

    public void FinaliseStatics()
    {
        _staticsFinalised = true;
        CalculateInitialPathing();
    }

    public Vector3 GetNextMovePosToTarget(Vector3 curPos, IPathingTarget target)
    {
        if (!_pathingData.ContainsKey(target))
        {
            Debug.LogError("Requesting move dir to invalid target!");
            return Vector3.zero;
        }

        var coord = WorldPosToCoord(curPos);
        var data = _pathingData[target].tilePathData;

        if (!data.ContainsKey(coord))
        {
            Debug.LogError($"Requesting move dir from invalid location: {curPos}");
            return Vector3.zero;
        }

        return data[coord].nextPos;
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

            var curCoord = WorldPosToCoord(crystal.transform.position);
            if (_pathingData[crystal].lastTargetCoord != curCoord)
                RecalculatePathing(crystal);
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

    void CalculateInitialPathing()
    {
        _doCalculationsImmediately = true;

        foreach (var crystal in Refs.I.Crystals)
        {
            _pathingData[crystal] = new PathingData();
            RecalculatePathing(crystal);
        }
        foreach (var castle in Refs.I.Castles)
        {
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
        pathingData.tilePathData[coord] = new TilePathData { nextPos = obj.PathingTargetPoint }; 
        pathingData.visitedTiles.Add(coord);

        SetNeighbourTileData(pathingData, coord);
        ProcessSomeTiles(pathingData);
    }

    void SetNeighbourTileData(PathingData pathingData, Vector2Int visitedTileCoord)
    {
        var visitedPos = new Vector3(visitedTileCoord.x, visitedTileCoord.y, 0);

        foreach (var dir in _dirs)
        {
            var neighbourCoord = visitedTileCoord + dir;

            if (
                !_pathableTiles.Contains(neighbourCoord) ||
                (
                    dir.x != 0 && dir.y != 0 &&
                    !(
                        _pathableTiles.Contains(visitedTileCoord + new Vector2Int(0, dir.y)) &&
                        _pathableTiles.Contains(visitedTileCoord + new Vector2Int(dir.x, 0))
                    )
                )
            ) continue;

            var neighbourWeight = pathingData.tilePathData[visitedTileCoord].weight + dir.magnitude;

            if (
                !pathingData.visitedTiles.Contains(neighbourCoord) &&
                (
                    !pathingData.tilesToVisitSet.Contains(neighbourCoord) ||
                    pathingData.tilePathData[neighbourCoord].weight > neighbourWeight
                )
            )
            {
                pathingData.tilePathData[neighbourCoord] = new TilePathData
                {
                    weight = neighbourWeight,
                    nextPos = visitedPos
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

    Vector2Int WorldPosToCoord(Vector3 worldPos) =>
        new Vector2Int(
            Mathf.RoundToInt(worldPos.x),
            Mathf.RoundToInt(worldPos.y)
        );

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        PathingData pathingData = null;
        var target = _gizmoVisualisationTarget as IPathingTarget;
        if (
            target != null &&
            _pathingData.ContainsKey(target)
        ) pathingData = _pathingData[target];

        Gizmos.color = new Color(1f, 1f, 1f, 0.2f);

        foreach (var coord in _pathableTiles)
        {
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

            if (pathingData != null)
            {
                var tileMid = new Vector3(coord.x + 0.5f, coord.y + 0.5f, 0);
                var dir = (pathingData.tilePathData[coord].nextPos - new Vector3(coord.x, coord.y, 0)).normalized;

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
        public Vector3 nextPos;
    }
}
