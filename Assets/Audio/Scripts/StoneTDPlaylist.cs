using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

public class StoneTDPlaylist : MonoBehaviour
{
    AudioSource[] _tracks;
    int _trackIndex = -1;
    AudioSource _cur;

    void OnEnable() =>
        _tracks = GetComponentsInChildren<AudioSource>();

    void Update()
    {
        if (_tracks.Length == 0)
            return;

        var shouldPlayNext = _cur == null || !_cur.isPlaying;
        if (!shouldPlayNext)
            return;

        _trackIndex = (_trackIndex + 1) % _tracks.Length;

        _cur = _tracks[_trackIndex];
        if (_cur.isActiveAndEnabled)
            _cur.Play();
        else _cur = null;
    }
}
