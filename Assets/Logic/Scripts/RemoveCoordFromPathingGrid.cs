using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class RemoveCoordFromPathingGrid : MonoBehaviour
{
    public const int EXEC_ORDER = PathingSystem.EXEC_ORDER + 1;

    void Awake()
    {
        Refs.I.ps.RemoveCoordFromPathingGrid(transform.position);
        Destroy(this);
    }
}
