using UnityEngine;
using UnityEngine.Events;

public class OnGameOverOrVictory : MonoBehaviour
{
    public UnityEvent onGameOver;
    public UnityEvent onVictory;

    void OnEnable() =>
        GameController.onGameOver += HandleGameOver;
    
    void OnDisable() =>
        GameController.onGameOver -= HandleGameOver;
    
    void HandleGameOver(bool victory)
    {
        if (victory)
            onVictory?.Invoke();
        else onGameOver?.Invoke();
    }
}
