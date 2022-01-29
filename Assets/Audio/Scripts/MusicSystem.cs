using MyLibrary;
using UnityEngine;

public class MusicSystem : MonoBehaviour
{
    const float LERP_DURATION = 1f;
    const string NORMAL_VOL_KEY = "Music Normal";
    const string AWAKENED_VOL_KEY = "Music Stones Awakened";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init() =>
        DontDestroyOnLoad(Instantiate(Resources.Load<GameObject>("Music")));

    bool _usingAwakenedTracks;

    void OnEnable()
    {
        GameController.onStonesAwakened += RefreshState;
        SceneSwitcherSystem.onSwitchedScene += RefreshState;
    }
    void Start() => SetInitialVolume();
    void OnDisable()
    {
        GameController.onStonesAwakened -= RefreshState;
        SceneSwitcherSystem.onSwitchedScene -= RefreshState;
    }

    void SetInitialVolume()
    {
        _usingAwakenedTracks = ShouldUseAwakenedTracks();
        var gaining = _usingAwakenedTracks ? AWAKENED_VOL_KEY : NORMAL_VOL_KEY;
        var decreasing = _usingAwakenedTracks ? NORMAL_VOL_KEY : AWAKENED_VOL_KEY;
        VolumeController.SetDynamicVolume(gaining, 1);
        VolumeController.SetDynamicVolume(decreasing, 0);
    }

    void RefreshState()
    {
        var shouldUseAwakenedTracks = ShouldUseAwakenedTracks();
        if (shouldUseAwakenedTracks == _usingAwakenedTracks)
            return;
        _usingAwakenedTracks = shouldUseAwakenedTracks;
        
        new Async(this)
            .Lerp(
                0, 1, LERP_DURATION,
                (step) =>
                {
                    var gaining = shouldUseAwakenedTracks ? AWAKENED_VOL_KEY : NORMAL_VOL_KEY;
                    var decreasing = shouldUseAwakenedTracks ? NORMAL_VOL_KEY : AWAKENED_VOL_KEY;

                    VolumeController.SetDynamicVolume(decreasing, 1f - step);
                    VolumeController.SetDynamicVolume(gaining, step);
                },
                TimeMode.Unscaled
            );
    }
    
    bool ShouldUseAwakenedTracks() => Refs.I != null && Refs.I.gc != null && Refs.I.gc.haveStonesAwakened;
}
