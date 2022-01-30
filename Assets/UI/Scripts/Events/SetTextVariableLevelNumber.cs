using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;

public class SetTextVariableLevelNumber : MonoBehaviour
{
    void Awake()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        var thisLevelIndex = StoneTDConfig.I.GetThisLevelIndex();
        var levelNum = thisLevelIndex + 1;

        var locString = GetComponent<LocalizeStringEvent>();
        locString.StringReference.Remove("LevelNumber");
        locString.StringReference.Add("LevelNumber", new IntVariable { Value = levelNum });
        locString.RefreshString();

        Destroy(this);
    }
}
