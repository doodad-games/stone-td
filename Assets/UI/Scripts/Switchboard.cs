using UnityEngine;

public class Switchboard : MonoBehaviour
{
    public void Insp_StartDefencePhase() =>
        Refs.I.gc.StartDefencePhase();

    public void Insp_TogglePause() =>
        Refs.I.gc.TogglePause();

    public void Insp_SkipAhead(float numSeconds) =>
        Refs.I.gc.SkipAhead((int)(numSeconds / Time.fixedDeltaTime));
    
    public void Insp_Retry() =>
        Refs.I.gc.Retry();
    
    public void Insp_ToggleStonePlacementMode(StoneTypeParams type) =>
        Refs.I.uic.StonePlacementMode = Refs.I.uic.StonePlacementMode == type.type
            ? Stone.Type.None
            : type.type;
}
