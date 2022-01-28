using UnityEngine;
using UnityEngine.Events;

public class OnAnyStonesTappedChanged : MonoBehaviour
{
    public StoneTypeParameter type;
    public UnityEvent onTapCountChangedToZero;
    public UnityEvent onTapCountChangedToNonZero;

    void OnEnable()
    {
        Stone.onAnyTappedChanged += Refresh;
        Refresh();
    }
    void OnDisable() => Stone.onAnyTappedChanged -= Refresh;

    void Refresh()
    {
        var numTapped = Refs.I.tappedStones[type.type].Count;

        if (numTapped == 0)
            onTapCountChangedToZero?.Invoke();
        else onTapCountChangedToNonZero?.Invoke();
    }
}
