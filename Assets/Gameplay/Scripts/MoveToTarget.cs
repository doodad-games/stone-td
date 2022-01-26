using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class MoveToTarget : MonoBehaviour
{
    public const int EXEC_ORDER = Invader.EXEC_ORDER;

#pragma warning disable CS0649
    [SerializeField] float _speed;
#pragma warning restore CS0649

    Transform _target;
    Vector3 _targetPos;

    void FixedUpdate() => Move();

    public void SetTarget(Transform tfm)
    {
        _target = tfm;
        RefreshTargetPos();
    }
    
    void Move()
    {
        RefreshTargetPos();

        var vec = _targetPos - transform.position;
        if (vec == Vector3.zero)
            return;

        var distSq = vec.sqrMagnitude;
        var dir = vec.normalized;

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
