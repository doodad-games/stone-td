using UnityEngine;
using UnityEngine.Events;

public class OnAnyStonesTappedChanged : MonoBehaviour
{
    public StoneTypeParameter type;
    public UnityEvent onTapCountChangedToZero;
    public UnityEvent onTapCountChangedToNonZero;

    void OnEnable()
    {
        Stone.onTappedJustChanged += HandleStoneTappedJustChanged;
        Refresh();
    }
    void OnDisable() => Stone.onTappedJustChanged -= HandleStoneTappedJustChanged;

    void HandleStoneTappedJustChanged(Stone _) => Refresh();

    void Refresh()
    {
        var numTapped = Refs.I.tappedStones[type.type].Count;

        if (numTapped == 0)
            onTapCountChangedToZero?.Invoke();
        else onTapCountChangedToNonZero?.Invoke();
    }
}
