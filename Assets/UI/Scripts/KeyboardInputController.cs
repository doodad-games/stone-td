using UnityEngine;

public class KeyboardInputController : MonoBehaviour
{
    void Update()
    {
        if (Refs.I.gc != null)
            CheckForGameplayInput();
    }

    void CheckForGameplayInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Refs.I.gc.SkipAhead((int)(1 / Time.fixedDeltaTime));
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Refs.I.gc.SkipAhead((int)(2 / Time.fixedDeltaTime));
        if (Input.GetKeyDown(KeyCode.Alpha3))
            Refs.I.gc.SkipAhead((int)(3 / Time.fixedDeltaTime));
    }
}
