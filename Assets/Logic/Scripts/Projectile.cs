using MyLibrary;
using UnityEngine;

[RequireComponent(typeof(MoveToTarget))]
[DefaultExecutionOrder(EXEC_ORDER)]
public class Projectile : MonoBehaviour
{
    public const int EXEC_ORDER = MoveToTarget.EXEC_ORDER + 1;

    public float reachRadius;

    MoveToTarget _movement;

    bool _initd;
    IEnemy _target;
    Transform _targetLocation;

    void OnEnable() => GameController.onTick += HandleTick;
    void OnDisable() => GameController.onTick -= HandleTick;

    public void Init(IEnemy target)
    {
        _initd = true;
        _target = target;
        _targetLocation = target.HitLocations.PickRandom();

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
        if (distSq < Mathf.Pow(reachRadius + _target.HitLocationRadius, 2))
        {
            var invader = _target as Invader;
            if (invader != null)
                invader.Die();
            else Debug.LogError($"Projectile's not sure what it's supposed to destroy here 🤔 {invader.gameObject}", invader.gameObject);

            Destroy(gameObject);
        }
    }
}
