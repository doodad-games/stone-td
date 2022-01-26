using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SetAnimRotationToMovementDirection : MonoBehaviour
{
    public string propertyName = "Rotation";

    Animator _anim;
    MoveToTarget _movement;

    Vector3 _lastDir;

    void OnEnable()
    {
        _anim = GetComponent<Animator>();
        _movement = GetComponentInParent<MoveToTarget>();
        Refresh();
    }

    void Update() => Refresh();

    void Refresh()
    {
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