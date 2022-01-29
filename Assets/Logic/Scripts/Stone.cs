using System;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public static event Action<Stone> onAnyTappedChanged;
    public static event Action<Stone.Type> onFailedToUntap;

    public event Action onTappedChanged;

    [HideInInspector] public bool tapped;
    public Type type;

    public void Insp_SetTapped(bool to)
    {
        if (tapped == to)
            return;
        
        if (to == false && !Refs.I.gc.CanConstructMore(type))
        {
            onFailedToUntap?.Invoke(type);
            return;
        }

        tapped = to;

        if (tapped)
            Refs.I.tappedStones[type].Add(this);
        else Refs.I.tappedStones[type].Remove(this);

        onTappedChanged?.Invoke();
        onAnyTappedChanged?.Invoke(this);
    }

    [Serializable]
    public enum Type
    {
        None = 0,
        Wall = 1
    }
}
