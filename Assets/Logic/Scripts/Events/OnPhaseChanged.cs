using UnityEngine;
using UnityEngine.Events;

public class OnPhaseChanged : MonoBehaviour
{
    public UnityEvent onEnterConstructionPhase;
    public UnityEvent onEnterDefencePhase;

    void Awake()
    {
        if (Refs.I.gc != null)
            // We always start in construction phase
            onEnterConstructionPhase.Invoke();
    }

    void OnEnable() =>
        GameController.onEnterDefencePhase += onEnterDefencePhase.Invoke;
    void OnDisable() =>
        GameController.onEnterDefencePhase -= onEnterDefencePhase.Invoke;
}
