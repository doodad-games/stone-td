using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInputController : MonoBehaviour
{
    public Vector2Int hoveredCoord;

    MouseSupport _lastHovered;
    MouseMode _activeMouseMode;
    string _activeDragSelectType;
    bool _isConstructingThings;

    public bool IsDraggingSomething => _activeMouseMode != MouseMode.None;
    public bool IsMousingOverSomething => _lastHovered != null;

    void OnEnable()
    {
        Stone.onFailedToUntap += HandleFailedToUntapStone;
        Refs.I.mouseC = this;
    }
    void Update() => CheckConstructionInput();
    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.mouseC = null;

        Stone.onFailedToUntap -= HandleFailedToUntapStone;
    }

    void HandleFailedToUntapStone(Stone.Type type) => _activeDragSelectType = null;

    void CheckConstructionInput()
    {
        if (Refs.I.gc.isDefencePhase)
        {
            if (_lastHovered != null)
            {
                _lastHovered.BroadcastMessage("Msg_OnHoverExit");
                _lastHovered = null;
            }

            return;
        }

        var mousePos = Refs.I.cam.ScreenToWorldPoint(Input.mousePosition);
        hoveredCoord = GetMouseCoord(mousePos);
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
            if (EventSystem.current.IsPointerOverGameObject())
                return;

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
                    if (
                        Refs.I.gc.CanConstructMore(Refs.I.uic.StonePlacementMode) &&
                        Refs.I.ps.IsPathable(hoveredCoord)
                    )
                    {
                        _isConstructingThings = true;
                        Refs.I.gc.ConstructThing(Refs.I.uic.StonePlacementMode, hoveredCoord);
                    }
                    else UIController.onError?.Invoke();
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
                else Refs.I.uic.StonePlacementMode = Stone.Type.None;
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
                    if (
                        Refs.I.gc.CanConstructMore(Refs.I.uic.StonePlacementMode) &&
                        Refs.I.ps.IsPathable(hoveredCoord)
                    ) Refs.I.gc.ConstructThing(Refs.I.uic.StonePlacementMode, hoveredCoord);
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
