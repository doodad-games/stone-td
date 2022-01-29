using UnityEngine;
using UnityEngine.Events;

public class OnFiredProjectile : MonoBehaviour
{
    public UnityEvent onFire;

    FireProjectilesAtEnemies _firer;

    void OnEnable()
    {
        _firer = GetComponentInParent<FireProjectilesAtEnemies>();
        _firer.onDidFire += onFire.Invoke;
    }
    void OnDisable()
    {
        if (_firer != null)
            _firer.onDidFire -= onFire.Invoke;
    }
}
