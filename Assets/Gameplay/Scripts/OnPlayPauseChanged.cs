using UnityEngine;
using UnityEngine.Events;

public class OnPlayPauseChanged : MonoBehaviour
{
    public UnityEvent onPlay;
    public UnityEvent onPause;

    void OnEnable()
    {
        GameController.onPlayPauseToggled += HandlePlayPauseToggled;
        HandlePlayPauseToggled();
    }
    
    void OnDisable() =>
        GameController.onPlayPauseToggled -= HandlePlayPauseToggled;
    
    void HandlePlayPauseToggled()
    {
        if (Refs.I.gc.IsPlaying)
            onPlay?.Invoke();
        else onPause?.Invoke();
    }
}
