using MyLibrary;
using UnityEngine;
using UnityEngine.EventSystems;

public class SFXButtons : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool hoverOnly;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!hoverOnly)
            SoundController.Play("Click On");
    }
    
    public void OnPointerEnter(PointerEventData eventData) =>
        SoundController.Play("Hover On");

    public void OnPointerExit(PointerEventData eventData) =>
        SoundController.Play("Hover Off");

    public void Insp_ClickOff() =>
        SoundController.Play("Click Off");
}
