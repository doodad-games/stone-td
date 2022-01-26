using System;
using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class GameController : MonoBehaviour
{
    public const int EXEC_ORDER = Refs.EXEC_ORDER + 1;

    public static event Action onTick;
    public static event Action onPlayPauseToggled;
    public static event Action onGameOver;

    public bool IsPlaying => Time.timeScale != 0;

    public bool isGameOver;

    void OnEnable()
    {
        Refs.I.gc = this;

        Invader.onBroughtCrystalToCastle += HandleBroughtCrystalToCastle;
    }
    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.gc = null;

        Invader.onBroughtCrystalToCastle -= HandleBroughtCrystalToCastle;
    }

    void FixedUpdate() =>
        DoTick();
    
    public void TogglePause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        onPlayPauseToggled?.Invoke();
    }
    
    public void SkipAhead(int numTicks)
    {
        if (numTicks < 0)
        {
            Debug.LogException(new InvalidOperationException());
            return;
        }

        for (var i = 0; i != numTicks; ++i)
            DoTick();
    }

    void DoTick()
    {
        if (!isGameOver)
            onTick?.Invoke();
    }

    void HandleBroughtCrystalToCastle()
    {
        isGameOver = true;
        onGameOver?.Invoke();
    }
}