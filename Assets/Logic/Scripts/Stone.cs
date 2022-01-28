using System;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public static event Action onAnyTappedChanged;
    public event Action onTappedChanged;

    [HideInInspector] public bool tapped;
    public Type type;

    public void Insp_ToggleTapped()
    {
        tapped = !tapped;

        if (tapped)
            Refs.I.tappedStones[type].Add(this);
        else Refs.I.tappedStones[type].Remove(this);

        onTappedChanged?.Invoke();
        onAnyTappedChanged?.Invoke();
    }

    [Serializable]
    public enum Type
    {
        None = 0,
        Wall = 1
    }
}
