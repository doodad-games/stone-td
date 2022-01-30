using UnityEngine;
using UnityEngine.Events;

public class OnTowerCountChanged : MonoBehaviour
{
    public UnityEvent onCountChangedToZero;
    public UnityEvent onCountChangedToNonZero;

    void OnEnable()
    {
        Tower.onCountChanged += HandleTowerCountChanged;
        Refresh();
    }
    void OnDisable() => Tower.onCountChanged -= HandleTowerCountChanged;

    void HandleTowerCountChanged(bool _) => Refresh();

    void Refresh()
    {
        if (Refs.I.Towers.Count == 0)
            onCountChangedToZero?.Invoke();
        else onCountChangedToNonZero?.Invoke();
    }
}
