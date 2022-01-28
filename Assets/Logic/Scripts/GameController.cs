using System;
using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class GameController : MonoBehaviour
{
    public const int EXEC_ORDER = Refs.EXEC_ORDER + 1;

    public static event Action onEnterDefencePhase;
    public static event Action onTick;
    public static event Action onPlayPauseToggled;
    public static event Action onGameOver;

    public bool IsPlaying => Time.timeScale != 0;

    public bool isGameOver;
    public float time;
    public bool isDefencePhase;

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

    void FixedUpdate()
    {
        if (isDefencePhase)
            DoTick();
    }
    
    public void StartDefencePhase()
    {
        if (isDefencePhase)
        {
            Debug.LogError("Tried to start defence phase while in defence phase 🤔");
            return;
        }

        isDefencePhase = true;
        onEnterDefencePhase?.Invoke();
    }
    
    public void TogglePause()
    {
        if (!isDefencePhase)
        {
            Debug.LogError("Tried to toggle pause while in construction phase 🤔");
            return;
        }

        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        onPlayPauseToggled?.Invoke();
    }
    
    public void SkipAhead(int numTicks)
    {
        if (!isDefencePhase)
        {
            Debug.LogError("Tried to skip ahead while in construction phase 🤔");
            return;
        }

        if (numTicks < 0)
        {
            Debug.LogException(new InvalidOperationException());
            return;
        }

        for (var i = 0; i != numTicks; ++i)
            DoTick();
    }

    public void Retry() =>
        SceneSwitcherSystem.I.ReloadCurrentScene();

    void DoTick()
    {
        if (isGameOver)
            return;

        time += Time.fixedDeltaTime;
        foreach (var del in onTick.GetInvocationList())
        {
            del.DynamicInvoke();
            if (isGameOver)
                return;
        }
    }

    void HandleBroughtCrystalToCastle()
    {
        isGameOver = true;
        onGameOver?.Invoke();
    }
}
