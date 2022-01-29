using System;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static event Action onCountChanged;

    void OnEnable()
    {
        Refs.I.Towers.Add(this);
        onCountChanged?.Invoke();
    }
    void OnDisable()
    {
        if (Refs.I != null)
        {
            Refs.I.Towers.Remove(this);
            onCountChanged?.Invoke();
        }
    }
}
