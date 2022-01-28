using UnityEngine;
using UnityEngine.Events;

public class MouseSupport : MonoBehaviour
{
    public string dragSelectType
    {
        get => _dragSelectType;
        set => _dragSelectType = value;
    }

#pragma warning disable CS0649
    [SerializeField] string _dragSelectType;
#pragma warning restore CS0649

    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;
    public UnityEvent onSelect;


    void Msg_OnHoverEnter() => onHoverEnter?.Invoke();
    void Msg_OnHoverExit() => onHoverExit?.Invoke();
    void Msg_OnSelect() => onSelect?.Invoke();
}
