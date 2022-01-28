using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    public float followDistance;

#pragma warning disable CS0649
    [SerializeField] float _speed;
#pragma warning restore CS0649

    [HideInInspector] public Vector3 lastDir;
    [HideInInspector] public bool didJustMove;

    IPathingTarget _target;
    Vector2Int _nextCoord;
    Vector3 _nextPos;

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

    public void SetTarget(IPathingTarget target)
    {
        _target = target;
        _nextPos = transform.position; // This'll trigger a `SetNextPos` on the next tick
    }

    void HandleTick() => Move();
    void HandleGameOver() => didJustMove = false;

    void SetNextPos()
    {
        _nextCoord = Refs.I.ps.GetNextMoveCoordToTarget(transform.position, _target);
        _nextPos = Refs.I.ps.CoordToWorldPos(_nextCoord);
    }
    
    void Move()
    {
        if (_target == null)
            return;

        didJustMove = false;

        var curPos = transform.position;

        { // Look at the vector directly to the target to check following distance threshold only
            var targetVec = _target.PathingTargetPoint - curPos;
            if (
                followDistance != 0f &&
                Mathf.Pow(followDistance, 2) > targetVec.sqrMagnitude
            )
            {
                // So they'll face the target when it passes them close by
                lastDir = targetVec.normalized;
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
                didJustMove = true;
                continue;
            }

            var dir = vec.normalized;
            transform.position += dir * movementRemaining;
            lastDir = dir;

            movementRemaining = 0f;
            didJustMove = true;
        }
    }
}
