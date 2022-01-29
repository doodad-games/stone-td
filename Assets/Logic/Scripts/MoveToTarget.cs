using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class MoveToTarget : MonoBehaviour, IMovement
{
    public const int EXEC_ORDER = 0;

    [HideInInspector] public Transform target;
    public float speed;

    Vector3 _lastDir;
    bool _didJustMove;

    public Vector3 LastMovementDir => _lastDir;
    public bool DidJustMove => _didJustMove;

    void OnEnable() => GameController.onTick += HandleTick;
    void OnDisable() => GameController.onTick -= HandleTick;

    void HandleTick() => Move();

    void Move()
    {
        if (target == null)
            return;

        var curPos = transform.position;
        var targetPos = target.position;
        var vec = targetPos - curPos;

        var distSq = vec.sqrMagnitude;
        var distToMove = speed * Time.fixedDeltaTime;

        _didJustMove = !Mathf.Approximately(distSq, 0);
        _lastDir = vec.normalized;

        if (distSq < Mathf.Pow(distToMove, 2))
        {
            transform.position = targetPos;
            return;
        }

        transform.position = curPos + _lastDir * distToMove;
    }
}
