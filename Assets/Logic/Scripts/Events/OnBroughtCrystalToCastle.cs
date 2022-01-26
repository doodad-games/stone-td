using UnityEngine;
using UnityEngine.Events;

public class OnBroughtCrystalToCastle : MonoBehaviour
{
    public UnityEvent onBroughtCrystalToCastle;

    void Msg_OnBroughtCrystalToCastle() =>
        onBroughtCrystalToCastle?.Invoke();
}
