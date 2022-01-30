using System;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class UIController : MonoBehaviour
{
    public static event Action onStonePlacementModeChanged;

    public static Action onError;

    Stone.Type _stonePlacementMode;

    public Stone.Type StonePlacementMode
    {
        get => _stonePlacementMode;
        set
        {
            if (value == _stonePlacementMode)
                return;

            _stonePlacementMode = value;
            onStonePlacementModeChanged?.Invoke();
        }
    }

    void OnEnable()
    {
        Refs.I.uic = this;
        Stone.onAnyTappedChanged += HandleAnyStoneTappedChanged;
    }
    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.uic = this;
        Stone.onAnyTappedChanged -= HandleAnyStoneTappedChanged;
    }

    void HandleAnyStoneTappedChanged(Stone stone)
    {
        if (stone.isTapped)
            StonePlacementMode = stone.type;
        else if (
            StonePlacementMode == stone.type &&
            Refs.I.tappedStones[stone.type].Count == 0
        ) StonePlacementMode = Stone.Type.None;
    }
}
