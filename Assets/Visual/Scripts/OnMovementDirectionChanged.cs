using UnityEngine;
using UnityEngine.Events;

public class OnMovementDirectionChanged : MonoBehaviour
{
    public UnityEvent onGoingLeft;
    public UnityEvent onGoingRight;

    IMovement _movement;

    float _nextRefreshTime;
    Vector3 _lastDir;

    void OnEnable() => _movement = GetComponentInParent<IMovement>();

    void Update() => Refresh();

    void Refresh()
    {
        if (_movement == null)
            return;

        var rotation = Vector2.Angle(Vector2.right, _movement.LastMovementDir);
        if (rotation <= 90f || rotation > 270f)
            onGoingRight?.Invoke();
        else onGoingLeft?.Invoke();
    }
}
