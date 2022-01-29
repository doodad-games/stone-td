using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
[RequireComponent(typeof(PathingBlocker))]
public class Wall : MonoBehaviour
{
    public const int EXEC_ORDER = PathingBlocker.EXEC_ORDER + 1;

#pragma warning disable CS0649
    [SerializeField] GameObject _coreVisuals;
    [SerializeField] GameObject _horizontalConnectorVisuals;
    [SerializeField] GameObject _verticalConnectorVisuals;
#pragma warning restore CS0649

    Wall[] _neighbours = new Wall[4]; // 0 = t, 1 = r, 2 = b, 3 = l
    Vector2Int _coord;

    void Awake() => _coreVisuals.SetActive(false);
    void OnEnable()
    {
        _coord = Refs.I.ps.WorldPosToCoord(transform.position);

        ReconnectVisuals();
        ReconnectNeighbourVisuals();
    }

    void OnDisable()
    {
        if (Refs.I != null && Refs.I.ps != null)
            ReconnectNeighbourVisuals();
    }

    public void Insp_SelfDestruct() =>
        Destroy(gameObject);

    void RefreshNeighbours()
    {
        _neighbours[0] = Refs.I.ps.GetBlocker(_coord + Vector2Int.up)?.GetComponent<Wall>();
        _neighbours[1] = Refs.I.ps.GetBlocker(_coord + Vector2Int.right)?.GetComponent<Wall>();
        _neighbours[2] = Refs.I.ps.GetBlocker(_coord + Vector2Int.down)?.GetComponent<Wall>();
        _neighbours[3] = Refs.I.ps.GetBlocker(_coord + Vector2Int.left)?.GetComponent<Wall>();
    }

    void ReconnectVisuals()
    {
        RefreshNeighbours();

        _horizontalConnectorVisuals.SetActive(_neighbours[1] != null);
        _verticalConnectorVisuals.SetActive(_neighbours[0] != null);

        var hasVerticalConnection = _neighbours[0] != null || _neighbours[2] != null;
        var hasHorizontalConnection = _neighbours[1] != null || _neighbours[3] != null;
        var fullyHorizontallyConnected = _neighbours[0] != null && _neighbours[2] != null;
        var fullyVerticallyConnected = _neighbours[1] != null && _neighbours[3] != null;

        if (
            // In defence phase core walls won't disable themselves if a connection gets destroyed
            !Refs.I.gc.isDefencePhase ||
            !_coreVisuals.activeSelf
        ) _coreVisuals.SetActive(
            (hasHorizontalConnection && hasVerticalConnection) ||
            !(fullyHorizontallyConnected || fullyVerticallyConnected)
        );
    }

    void ReconnectNeighbourVisuals()
    {
        foreach (var neighbour in _neighbours)
            if (neighbour != null)
                neighbour.ReconnectVisuals();
    }
}
