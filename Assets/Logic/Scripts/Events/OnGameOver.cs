using UnityEngine;
using UnityEngine.Events;

public class OnGameOver : MonoBehaviour
{
    public UnityEvent onInvadersStoleCrystal;
    public UnityEvent onStonesBrokeCrystal;
    public UnityEvent onVictory;

    void OnEnable() => GameController.onGameOver += HandleGameOver;
    void OnDisable() => GameController.onGameOver -= HandleGameOver;

    void HandleGameOver(GameOverReason reason)
    {
        if (reason == GameOverReason.InvadersStoleCrystal)
            onInvadersStoleCrystal?.Invoke();
        else if (reason == GameOverReason.StonesBrokeCrystal)
            onStonesBrokeCrystal?.Invoke();
        else if (reason == GameOverReason.Victory)
            onVictory?.Invoke();
    }
}