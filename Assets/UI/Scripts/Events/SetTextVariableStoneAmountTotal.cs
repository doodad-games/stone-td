using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class SetTextVariableStoneAmountTotal : MonoBehaviour
{
    public StoneTypeParameter type;
    public string variableKey = "AmountTotal";

    LocalizeStringEvent _locString;

    void OnEnable()
    {
        if (type.type == Stone.Type.None)
        {
            Debug.LogError("Uninitialised stone type", gameObject);
            Destroy(this);
            return;
        }

        _locString = GetComponent<LocalizeStringEvent>();
        Stone.onAnyTappedChanged += HandleAnyStoneTappedChanged;
        Refresh();
    }
    void OnDisable() => Stone.onAnyTappedChanged -= HandleAnyStoneTappedChanged;

    void HandleAnyStoneTappedChanged(Stone _) => Refresh();

    void Refresh()
    {
        _locString.StringReference.Remove(variableKey);
        _locString.StringReference.Add(
            variableKey,
            new IntVariable { Value = Refs.I.tappedStones[type.type].Count }
        );
        _locString.RefreshString();
    }
}
