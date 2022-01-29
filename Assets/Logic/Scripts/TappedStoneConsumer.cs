using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
[RequireComponent(typeof(PathingBlocker))]
public class TappedStoneConsumer : MonoBehaviour
{
    public const int EXEC_ORDER = PathingBlocker.EXEC_ORDER + 1;

    public StoneTypeParams type;

    void OnEnable() =>
        Refs.I.UseTappedStone(type.type);

    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.ReturnTappedStone(type.type);
    }

    public void Insp_SelfDestruct() =>
        Destroy(gameObject);
}
