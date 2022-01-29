using MyLibrary;
using UnityEngine;

[RequireComponent(typeof(MoveToTarget))]
[DefaultExecutionOrder(EXEC_ORDER)]
public class Projectile : MonoBehaviour
{
    public const int EXEC_ORDER = MoveToTarget.EXEC_ORDER + 1;

    public float reachRadius;
    public int damageDealt;

    MoveToTarget _movement;

    bool _initd;
    Enemy _target;
    Transform _targetLocation;

    void OnEnable() => GameController.onTick += HandleTick;
    void OnDisable() => GameController.onTick -= HandleTick;

    public void Init(Enemy target)
    {
        _initd = true;
        _target = target;
        _targetLocation = target.hitLocations.PickRandom();

        GetComponent<MoveToTarget>().target = _targetLocation;
    }

    void HandleTick()
    {
        if (_targetLocation == null)
        {
            if (!_initd)
                Debug.LogError("Projectile ticked without having been initialised 😮");

            Destroy(gameObject);
            return;
        }

        var distSq = (transform.position - _targetLocation.position).sqrMagnitude;
        if (distSq < Mathf.Pow(reachRadius + _target.hitLocationRadius, 2))
        {
            _target.TakeDamage(damageDealt);
            Destroy(gameObject);
        }
    }
}
