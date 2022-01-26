using UnityEngine;
using UnityEngine.Events;

public class OnGameOver : MonoBehaviour
{
    public UnityEvent onGameOver;

    void OnEnable() =>
        GameController.onGameOver += HandlePlayPauseToggled;
    
    void OnDisable() =>
        GameController.onGameOver -= HandlePlayPauseToggled;
    
    void HandlePlayPauseToggled() =>
        onGameOver?.Invoke();
}
