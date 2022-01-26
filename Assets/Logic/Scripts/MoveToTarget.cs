using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class MoveToTarget : MonoBehaviour
{
    public const int EXEC_ORDER = Invader.EXEC_ORDER;

    public float followDistance;

#pragma warning disable CS0649
    [SerializeField] float _speed;
#pragma warning restore CS0649

    [HideInInspector] public Vector3 lastDir;

    Transform _target;
    Vector3 _targetPos;

    void OnEnable() => GameController.onTick += HandleTick;
    void OnDisable() => GameController.onTick -= HandleTick;

    public void SetTarget(Transform tfm)
    {
        _target = tfm;
        RefreshTargetPos();
    }

    void HandleTick() => Move();
    
    void Move()
    {
        RefreshTargetPos();

        var vec = _targetPos - transform.position;
        if (vec == Vector3.zero)
            return;

        var distSq = vec.sqrMagnitude;
        var dir = vec.normalized;

        if (followDistance != 0f)
        {
            var actualTargetPos = _targetPos - dir * followDistance;
            vec = actualTargetPos - transform.position;
            if (vec == Vector3.zero)
                return;

            // Set it to the original direction because we don't want to see them continuously flipping
            lastDir = dir;

            distSq = vec.sqrMagnitude;
            dir = vec.normalized;
        }
        else lastDir = dir;


        var thisTickSpeed = _speed * Time.fixedDeltaTime;
        var amountToMove = Mathf.Pow(thisTickSpeed, 2) > distSq
            ? Mathf.Sqrt(distSq)
            : thisTickSpeed;
        
        transform.position += dir * thisTickSpeed;
    }

    void RefreshTargetPos()
    {
        if (_target != null)
            _targetPos = _target.position;
    }
}
