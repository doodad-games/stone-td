using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SetAnimRotationToMovementDirection : MonoBehaviour
{
    const float REFRESH_INTERVAL = 0.4f;

    public string propertyName = "Rotation";

    Animator _anim;
    MoveToTarget _movement;

    Vector3 _lastDir;
    float _nextRefreshTime;

    void OnEnable()
    {
        _anim = GetComponent<Animator>();
        _movement = GetComponentInParent<MoveToTarget>();
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

        var newDir = _movement.lastDir;
        if (_lastDir == newDir)
            return;
        _lastDir = newDir;

        var rotation = Vector2.Angle(Vector2.right, newDir);
        _anim.SetFloat(propertyName, rotation);
    }
}
