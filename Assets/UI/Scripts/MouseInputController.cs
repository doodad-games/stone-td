using UnityEngine;

public class MouseInputController : MonoBehaviour
{
    MouseSupport _lastHovered;
    string _activeDragSelectType;
    bool _isConstructingThings;

    void Update() => CheckConstructionInput();

    void CheckConstructionInput()
    {
        if (Refs.I.gc.isDefencePhase)
            return;

        var mousePos = Refs.I.cam.ScreenToWorldPoint(Input.mousePosition);
        var raycast = Physics2D.Raycast(mousePos, Vector2.zero);

        var mouseSupportComp = raycast.collider?.GetComponentInParent<MouseSupport>();

        if (mouseSupportComp != _lastHovered)
        {
            if (_lastHovered != null)
                _lastHovered.BroadcastMessage("Msg_OnHoverExit");
            
            if (mouseSupportComp != null)
                mouseSupportComp.BroadcastMessage("Msg_OnHoverEnter");
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (mouseSupportComp != null)
            {
                _activeDragSelectType = mouseSupportComp.dragSelectType;
                mouseSupportComp.BroadcastMessage("Msg_OnSelect");
            }
            else if (Refs.I.uic.StonePlacementMode != Stone.Type.None)
            {
                var coord = GetMouseCoord(mousePos);
                if (Refs.I.ps.IsPathable(coord))
                {
                    _isConstructingThings = true;
                    Refs.I.gc.ConstructThing(Refs.I.uic.StonePlacementMode, coord);
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (mouseSupportComp != null)
            {
                if (
                    mouseSupportComp != _lastHovered &&
                    mouseSupportComp.dragSelectType == _activeDragSelectType
                ) mouseSupportComp.BroadcastMessage("Msg_OnSelect");
            }
            else if (_isConstructingThings)
            {
                var coord = GetMouseCoord(mousePos);
                if (Refs.I.ps.IsPathable(coord))
                {
                    _isConstructingThings = true;
                    Refs.I.gc.ConstructThing(Refs.I.uic.StonePlacementMode, coord);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _activeDragSelectType = null;
            _isConstructingThings = false;
        }

        _lastHovered = mouseSupportComp;
    }

    Vector2Int GetMouseCoord(Vector3 mousePos) =>
        Refs.I.ps.WorldPosToCoord(mousePos - new Vector3(0.5f, 0.5f, 0));
}
