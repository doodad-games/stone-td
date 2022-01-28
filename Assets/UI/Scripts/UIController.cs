using System;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class UIController : MonoBehaviour
{
    public static event Action onStonePlacementModeChanged;

    Stone.Type _stonePlacementMode;

    public Stone.Type StonePlacementMode
    {
        get => _stonePlacementMode;
        set
        {
            _stonePlacementMode = value;
            onStonePlacementModeChanged?.Invoke();
        }
    }

    void OnEnable()
    {
        Refs.I.uic = this;
        Stone.onAnyTappedChanged += HandleStoneTapsChanged;
    }
    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.uic = this;
        Stone.onAnyTappedChanged -= HandleStoneTapsChanged;
    }

    void HandleStoneTapsChanged()
    {
        if (_stonePlacementMode == Stone.Type.None)
            return;

        if (Refs.I.tappedStones[_stonePlacementMode].Count == 0)
            StonePlacementMode = Stone.Type.None;
    }
}
