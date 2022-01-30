using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class GameController : MonoBehaviour
{
    public const int EXEC_ORDER = Refs.EXEC_ORDER + 1;

    public static event Action onEnterDefencePhase;
    public static event Action onTick;
    public static event Action onPlayPauseToggled;
    public static event Action<float> onWillSkipAhead;
    public static event Action onStonesAwakened;
    public static event Action<GameOverReason> onGameOver;

    static Dictionary<Stone.Type, int> _tapValueOverrides = new Dictionary<Stone.Type, int>
    {
        { Stone.Type.Wall, 4 }
    };

    public bool IsPlaying => Time.timeScale != 0;

    public bool isGameOver;
    public float time;
    public bool isDefencePhase;
    public bool haveStonesAwakened;

    void OnEnable()
    {
        Refs.I.gc = this;

        Invader.onBroughtCrystalToCastle += HandleBroughtCrystalToCastle;
        Crystal.onAnyBroken += HandleCrystalBroken;
        Enemy.onAnyDied += HandleEnemyDied;
    }
    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.gc = null;

        Invader.onBroughtCrystalToCastle -= HandleBroughtCrystalToCastle;
        Crystal.onAnyBroken -= HandleCrystalBroken;
        Enemy.onAnyDied -= HandleEnemyDied;
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

        onWillSkipAhead?.Invoke(numTicks * Time.fixedDeltaTime);

        for (var i = 0; i != numTicks; ++i)
            DoTick();
    }

    public bool CanConstructMore(Stone.Type type) =>
        Refs.I.usedTappedStones[type] < GetAmountTapped(type);
    
    public bool CanUntapStone(Stone stone)
    {
        var remainingAfterUntap = GetAmountTapped(stone.type) - GetTapValue(stone.type);
        return Refs.I.usedTappedStones[stone.type] <= remainingAfterUntap;
    }
    
    public void ConstructThing(Stone.Type type, Vector2Int coord)
    {
        if (!CanConstructMore(type))
        {
            Debug.LogError($"Attempting to construct more {type} when all tapped stones have been exhausted! Ignoring");
            return;
        }

        var resourceName = Enum.GetName(typeof(Stone.Type), type);
        Instantiate(
            Resources.Load<GameObject>(resourceName),
            Refs.I.ps.CoordToWorldPos(coord),
            Quaternion.identity
        );
    }

    public int GetAmountTapped(Stone.Type type) =>
        Refs.I.tappedStones[type].Count * GetTapValue(type);

    public int GetTapValue(Stone.Type type) =>
        _tapValueOverrides.ContainsKey(type) ? _tapValueOverrides[type] : 1;

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
        onGameOver?.Invoke(GameOverReason.InvadersStoleCrystal);
    }

    void HandleCrystalBroken()
    {
        isGameOver = true;
        onGameOver?.Invoke(GameOverReason.StonesBrokeCrystal);
    }

    void HandleEnemyDied(Enemy _)
    {
        if (
            Refs.I.Enemies.Count == 0 &&
            Refs.I.Spawners.All(_ => !_.IsStillSpawning)
        )
        {
            if (haveStonesAwakened)
            {
                isGameOver = true;
                onGameOver?.Invoke(GameOverReason.Victory);
            }
            else AwakenStones();
        }
    }

    void AwakenStones()
    {
        haveStonesAwakened = true;
        foreach (var pair in Refs.I.tappedStones)
            foreach (var stone in pair.Value)
                stone.Awaken();
        
        onStonesAwakened?.Invoke();
    }
}

public enum GameOverReason
{
    Victory,
    InvadersStoleCrystal,
    StonesBrokeCrystal
}
