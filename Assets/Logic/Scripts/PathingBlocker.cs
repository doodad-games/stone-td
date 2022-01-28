using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class PathingBlocker : MonoBehaviour
{
    public const int EXEC_ORDER = PathingSystem.EXEC_ORDER + 1;

    public bool isStatic;
    public bool isDestructible;

    Vector3 _position;

    void OnEnable()
    {
        _position = transform.position;

        if (isStatic)
        {
            Refs.I.ps.StaticallyBlockCoord(transform.position);
            Destroy(this);
        }
        else Refs.I.ps.BlockCoord(this);
    }

    void Update()
    {
        if (transform.position != _position)
            Debug.LogError("A pathing blocker moved! This isn't supported");
    }

    void OnDisable()
    {
        if (isStatic)
            return;

        if (Refs.I?.ps != null)
            Refs.I.ps.UnblockCoord(this);
    }
}
