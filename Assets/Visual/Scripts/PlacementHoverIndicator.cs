using System;
using UnityEngine;

public class PlacementHoverIndicator : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] OutlineVisuals[] _visuals;
#pragma warning restore CS0649

    bool _allDisabled;

    void Update()
    {
        if (
            Refs.I.gc.isDefencePhase ||
            Refs.I.uic.StonePlacementMode == Stone.Type.None ||
            !Refs.I.ps.IsPathable(Refs.I.mouseC.hoveredCoord)
        )
        {
            if (!_allDisabled)
            {
                _allDisabled = true;
                DisableAll();
            }
            return;
        }

        _allDisabled = false;

        transform.position = Refs.I.ps.CoordToWorldPos(Refs.I.mouseC.hoveredCoord);

        foreach (var visual in _visuals)
            visual.obj.SetActive(
                Refs.I.uic.StonePlacementMode == visual.type
            );
    }

    void DisableAll()
    {
        foreach (var visual in _visuals)
            visual.obj.SetActive(false);
    }

    [Serializable]
    struct OutlineVisuals
    {
        public Stone.Type type;
        public GameObject obj;
    }
}
