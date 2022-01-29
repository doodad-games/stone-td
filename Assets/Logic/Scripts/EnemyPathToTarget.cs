using UnityEngine;

public class EnemyPathToTarget : MonoBehaviour, IMovement
{
    public float followDistance;

#pragma warning disable CS0649
    [SerializeField] float _speed;
#pragma warning restore CS0649

    IPathingTarget _target;
    Vector2Int _nextCoord;
    Vector3 _nextPos;
    bool _didJustMove;
    Vector3 _lastDir;

    public Vector3 LastMovementDir => _lastDir;
    public bool DidJustMove => _didJustMove;
    public IPathingTarget Target
    {
        get => _target;
        set
        {
            _target = value;
            _nextPos = transform.position; // This'll trigger a `SetNextPos` on the next tick
        }
    }

    void OnEnable()
    {
        GameController.onTick += HandleTick;
        GameController.onGameOver += HandleGameOver;
    }
    void OnDisable()
    {
        GameController.onTick -= HandleTick;
        GameController.onGameOver -= HandleGameOver;
    }

    void HandleTick() => Move();
    void HandleGameOver(GameOverReason _) => _didJustMove = false;

    void SetNextPos()
    {
        _nextCoord = Refs.I.ps.GetNextMoveCoordToTarget(transform.position, _target);
        _nextPos = Refs.I.ps.CoordToWorldPos(_nextCoord);
    }
    
    void Move()
    {
        if (_target == null)
            return;

        _didJustMove = false;

        var curPos = transform.position;

        { // Look at the vector directly to the target to check following distance threshold only
            var targetVec = _target.PathingTargetPoint - curPos;
            if (
                followDistance != 0f &&
                Mathf.Pow(followDistance, 2) > targetVec.sqrMagnitude
            )
            {
                // So they'll face the target when it passes them close by
                _lastDir = targetVec.normalized;
                return;
            }
        }

        // Otherwise continue moving one tile at a time

        var movementRemaining = _speed * Time.fixedDeltaTime;
        while (movementRemaining != 0f)
        {
            var blocker = Refs.I.ps.GetBlocker(_nextCoord);
            if (blocker?.isDestructible == true)
                // TODO: Animations
                Destroy(blocker.gameObject);

            var vec = _nextPos - curPos;
            var dist = vec.magnitude;

            if (dist < movementRemaining)
            {
                transform.position = curPos = _nextPos;
                SetNextPos();

                if (_nextPos == curPos)
                    return;

                movementRemaining -= dist;
                _didJustMove = true;
                continue;
            }

            var dir = vec.normalized;
            transform.position += dir * movementRemaining;
            _lastDir = dir;

            movementRemaining = 0f;
            _didJustMove = true;
        }
    }
}
