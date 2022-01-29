using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class SetTextVariableStoneAmountUsed : MonoBehaviour
{
    public StoneTypeParams type;
    public string variableKey = "AmountUsed";

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
        Refs.onUsedTappedStonesChanged += HandleUsedTappedStonesChanged;
        Refresh();
    }
    void OnDisable() => Refs.onUsedTappedStonesChanged -= HandleUsedTappedStonesChanged;

    void HandleUsedTappedStonesChanged(Stone.Type changedType)
    {
        if (changedType == type.type)
            Refresh();
    }

    void Refresh()
    {
        _locString.StringReference.Remove(variableKey);
        _locString.StringReference.Add(
            variableKey,
            new IntVariable { Value = Refs.I.usedTappedStones[type.type] }
        );
        _locString.RefreshString();
    }
}
