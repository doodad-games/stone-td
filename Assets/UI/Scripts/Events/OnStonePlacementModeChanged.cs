using UnityEngine;
using UnityEngine.Events;

public class OnStonePlacementModeChanged : MonoBehaviour
{
    public StoneTypeParameter type;
    public UnityEvent onIsType;
    public UnityEvent onIsNotType;

    void OnEnable()
    {
        UIController.onStonePlacementModeChanged += Refresh;
        Refresh();
    }
    void OnDisable() => UIController.onStonePlacementModeChanged -= Refresh;

    void Refresh()
    {
        if (Refs.I.uic.StonePlacementMode == type.type)
            onIsType?.Invoke();
        else onIsNotType?.Invoke();
    }
}
