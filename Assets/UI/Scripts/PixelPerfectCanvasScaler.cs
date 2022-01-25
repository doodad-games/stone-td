using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

/**
 * - Assumes the attached canvas uses a camera with a URP PixelPerfectCamera component
 * - Only the ConstantPixelSize scale mode is supported
 *     - This is enforced in the inspector using OnValidate, but not enforced by code
 * - This will act weird if the component is moved around / reparented for some reason. Don't do it 😛 
 */
// Execute after PixelPerfectCamera (will break if this Awakes before that)
[DefaultExecutionOrder(1)]
public class PixelPerfectCanvasScaler : CanvasScaler
{
    PixelPerfectCamera _canvasPixelPerfectCamera;

    protected override void OnEnable()
    {
        _canvasPixelPerfectCamera = GetComponent<Canvas>()?.worldCamera?.GetComponent<PixelPerfectCamera>();
        base.OnEnable();
    }

#if UNITY_EDITOR
    // For some reason `HandleConstantPixelSize` stops being executed after entering then exiting play mode
    // (along with the other `Handle*Size` methods), so let's directly execute it here.
    void Update() => Handle();
#endif

    protected override void HandleConstantPixelSize()
    {
        if (_canvasPixelPerfectCamera == null)
            return;

        // Only calling `SetReferencePixelsPerUnit` doesn't seem to update the value in the inspector 🤔 
        m_ReferencePixelsPerUnit = _canvasPixelPerfectCamera.assetsPPU;
        SetReferencePixelsPerUnit(_canvasPixelPerfectCamera.assetsPPU);

        // Only calling `SetScaleFactor` doesn't seem to update the value in the inspector 🤔 
        m_ScaleFactor = _canvasPixelPerfectCamera.pixelRatio;
        SetScaleFactor(_canvasPixelPerfectCamera.pixelRatio);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        uiScaleMode = ScaleMode.ConstantPixelSize;
        base.OnValidate();
    }
#endif
}
