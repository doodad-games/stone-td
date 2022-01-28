using System;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public event Action onTappedChanged;

    [HideInInspector] public bool tapped;

    public void Insp_ToggleTapped()
    {
        tapped = !tapped;
        onTappedChanged?.Invoke();
    }
}
