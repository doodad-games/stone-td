using UnityEngine;
using UnityEngine.Events;

public class OnIsLastLevel : MonoBehaviour
{
    public UnityEvent onIsLastLevel;
    public UnityEvent onIsNotLastLevel;

    void OnEnable()
    {
        if (StoneTDConfig.I.GetThisLevelIndex() == StoneTDConfig.I.orderedLevelSceneNames.Length - 1)
            onIsLastLevel?.Invoke();
        else onIsNotLastLevel?.Invoke();

        Destroy(this);
    }
}
