using UnityEngine;

public class MouseInputController : MonoBehaviour
{
    MouseSupport _lastHovered;
    string _activeDragSelectType;

    void Update() => CheckConstructionInput();

    void CheckConstructionInput()
    {
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
        }
        else if (Input.GetMouseButton(0))
        {
            if (
                mouseSupportComp != null &&
                mouseSupportComp != _lastHovered &&
                mouseSupportComp.dragSelectType == _activeDragSelectType
            ) mouseSupportComp.BroadcastMessage("Msg_OnSelect");
        }
        if (Input.GetMouseButtonUp(0))
            _activeDragSelectType = null;

        _lastHovered = mouseSupportComp;
    }
}
