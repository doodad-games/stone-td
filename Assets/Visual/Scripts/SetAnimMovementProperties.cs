using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SetAnimMovementProperties : MonoBehaviour
{
    const float REFRESH_INTERVAL = 0.2f;

    public string rotationPropertyName = "Rotation";
    public string didJustMovePropertyName = "Is Moving";

    Animator _anim;
    IMovement _movement;

    Vector3 _lastDir;
    float _nextRefreshTime;

    void OnEnable()
    {
        _anim = GetComponent<Animator>();
        _movement = GetComponentInParent<IMovement>();
    }

    void Start() => Refresh(true);
    void Update() => Refresh(false);

    void Refresh(bool force)
    {
        if (!force)
        {
            if (Time.time < _nextRefreshTime)
                return;
            _nextRefreshTime = Time.time + REFRESH_INTERVAL;
        }

        if (_movement == null)
            return;

        _anim.SetBool(didJustMovePropertyName, _movement.DidJustMove);

        var newDir = _movement.LastMovementDir;
        if (!force && _lastDir == newDir)
            return;
        _lastDir = newDir;

        var rotation = Vector2.Angle(Vector2.right, newDir);
        _anim.SetFloat(rotationPropertyName, rotation);
    }
}
