using UnityEngine;

public class MouseInputController : MonoBehaviour
{
    MouseSupport _lastHovered;
    MouseMode _activeMouseMode;
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

        if (_activeMouseMode == MouseMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _activeMouseMode = MouseMode.LeftClick;

                if (mouseSupportComp != null)
                {
                    _activeDragSelectType = mouseSupportComp.dragSelectType;
                    mouseSupportComp.BroadcastMessage("Msg_OnLeftClick");
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
            else if (Input.GetMouseButtonDown(1))
            {
                _activeMouseMode = MouseMode.RightClick;

                if (mouseSupportComp != null)
                {
                    _activeDragSelectType = mouseSupportComp.dragSelectType;
                    mouseSupportComp.BroadcastMessage("Msg_OnRightClick");
                }
            }
        }
        else if (_activeMouseMode == MouseMode.LeftClick)
        {
            if (Input.GetMouseButtonUp(0))
            {
                _activeMouseMode = MouseMode.None;

                _activeDragSelectType = null;
                _isConstructingThings = false;
            }
            else
            {
                if (mouseSupportComp != null)
                {
                    if (
                        mouseSupportComp != _lastHovered &&
                        mouseSupportComp.dragSelectType == _activeDragSelectType
                    ) mouseSupportComp.BroadcastMessage("Msg_OnLeftClick");
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
        }
        else if (_activeMouseMode == MouseMode.RightClick)
        {
            if (Input.GetMouseButtonUp(1))
            {
                _activeMouseMode = MouseMode.None;

                _activeDragSelectType = null;
                _isConstructingThings = false;
            }
            else
            {
                if (mouseSupportComp != null)
                {
                    if (
                        mouseSupportComp != _lastHovered &&
                        mouseSupportComp.dragSelectType == _activeDragSelectType
                    ) mouseSupportComp.BroadcastMessage("Msg_OnRightClick");
                }
            }
        }

        _lastHovered = mouseSupportComp;
    }

    Vector2Int GetMouseCoord(Vector3 mousePos) =>
        Refs.I.ps.WorldPosToCoord(mousePos - new Vector3(0.5f, 0.5f, 0));
    
    enum MouseMode
    {
        None,
        LeftClick,
        RightClick
    }
}
