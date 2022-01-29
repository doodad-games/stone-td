using UnityEngine;
using UnityEngine.Events;

public class OnAnyStonesTappedChanged : MonoBehaviour
{
    public StoneTypeParams type;
    public UnityEvent onTapCountChangedToZero;
    public UnityEvent onTapCountChangedToNonZero;

    void OnEnable()
    {
        Stone.onAnyTappedChanged += HandleAnyStoneTappedChanged;
        Refresh();
    }
    void OnDisable() => Stone.onAnyTappedChanged -= HandleAnyStoneTappedChanged;

    void HandleAnyStoneTappedChanged(Stone _) => Refresh();

    void Refresh()
    {
        var numTapped = Refs.I.tappedStones[type.type].Count;

        if (numTapped == 0)
            onTapCountChangedToZero?.Invoke();
        else onTapCountChangedToNonZero?.Invoke();
    }
}
