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
        SceneSwitcherSystem.I.ReloadCurrentScene();
    
    public void Insp_NextLevel() =>
        SceneSwitcherSystem.I.LoadNextLevel();
    
    public void Insp_SetStonePlacementModeIfUnset(StoneTypeParams type)
    {
        if (Refs.I.uic.StonePlacementMode == Stone.Type.None)
            Refs.I.uic.StonePlacementMode = type.type;
    }
    
    public void Insp_ToggleStonePlacementMode(StoneTypeParams type) =>
        Refs.I.uic.StonePlacementMode = Refs.I.uic.StonePlacementMode == type.type
            ? Stone.Type.None
            : type.type;
}
