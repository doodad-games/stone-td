using System.Linq;
using MyLibrary;
using UnityEngine;

public class StoneTDPlaylist : MonoBehaviour
{
    public StoneTDPlaylist playlistToFollow;

    AudioSource[] _tracks;
    int[] _shuffleOrder;
    int _trackIndex;
    int _shuffleIndex = -1;
    AudioSource _cur;

    void OnEnable()
    {
        _tracks = GetComponentsInChildren<AudioSource>();
        _shuffleOrder = Enumerable.Range(0, _tracks.Length).Shuffle_().ToArray();
    }

    void Update()
    {
        if (_tracks.Length == 0)
            return;

        var shouldPlayNext = _cur == null || !_cur.isPlaying;
        if (!shouldPlayNext)
            return;
        _cur?.Stop();

        if (playlistToFollow == null)
        {
            _shuffleIndex = (_shuffleIndex + 1) % _tracks.Length;
            _trackIndex = _shuffleOrder[_shuffleIndex];
        }
        else _trackIndex = playlistToFollow._trackIndex;

        _cur = _tracks[_trackIndex];
        if (_cur.isActiveAndEnabled)
            _cur.Play();
        else _cur = null;
    }
}
