using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class PathingSystemFinaliser : MonoBehaviour
{
    public const int EXEC_ORDER = 100; // After all Crystals, Castles, what not

    void Awake()
    {
        Refs.I.ps.FinaliseStatics();
        Destroy(this);
    }
}
