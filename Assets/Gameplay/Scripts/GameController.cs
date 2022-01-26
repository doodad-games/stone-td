using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static event Action onTick;

    void OnEnable() => Refs.I.gc = this;
    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.gc = null;
    }

    void FixedUpdate() =>
        onTick?.Invoke();
    
    public void TogglePause() =>
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    
    public void SkipAhead(int numTicks)
    {
        if (numTicks < 0)
        {
            Debug.LogException(new InvalidOperationException());
            return;
        }

        for (var i = 0; i != numTicks; ++i)
            onTick?.Invoke();
    }
}
