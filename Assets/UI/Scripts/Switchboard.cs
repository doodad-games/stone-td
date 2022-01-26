using UnityEngine;

public class Switchboard : MonoBehaviour
{
    public void Insp_TogglePause() =>
        Refs.I.gc.TogglePause();

    public void Insp_SkipAhead(float numSeconds) =>
        Refs.I.gc.SkipAhead((int)(numSeconds / Time.fixedDeltaTime));
    
    public void Insp_Retry() =>
        SceneSwitcherSystem.I.ReloadCurrentScene();
}
