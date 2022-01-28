using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class SetTextVariableStoneAmountTotal : MonoBehaviour
{
    public Stone.Type type;

    LocalizeStringEvent _locString;

    void OnEnable()
    {
        if (type == Stone.Type.None)
        {
            Debug.LogError("Uninitialised stone type", gameObject);
            Destroy(this);
            return;
        }

        _locString = GetComponent<LocalizeStringEvent>();
        Stone.onAnyTappedChanged += Refresh;
        Refresh();
    }
    void OnDisable() => Stone.onAnyTappedChanged -= Refresh;

    void Refresh()
    {
        _locString.StringReference.Remove("AmountTotal");
        _locString.StringReference.Add(
            "AmountTotal",
            new IntVariable { Value = Refs.I.tappedStones[type].Count }
        );
        _locString.RefreshString();
    }
}
