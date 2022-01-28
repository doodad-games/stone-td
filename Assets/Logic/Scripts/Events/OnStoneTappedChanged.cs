using UnityEngine;
using UnityEngine.Events;

public class OnStoneTappedChanged : MonoBehaviour
{
    public UnityEvent onTapped;
    public UnityEvent onUntapped;

    Stone _stone;

    void OnEnable()
    {
        _stone = GetComponentInParent<Stone>();
        _stone.onTappedChanged += Refresh;
        Refresh();
    }
    void OnDisable()
    {
        if (_stone != null)
            _stone.onTappedChanged -= Refresh;
    }

    void Refresh()
    {
        if (_stone.tapped)
            onTapped?.Invoke();
        else onUntapped?.Invoke();
    }
}
