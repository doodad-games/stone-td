using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;

public class SetTextVariableLevelNumber : MonoBehaviour
{
    void Awake()
    {
        var scene = SceneManager.GetActiveScene();
        // 0 is scene loader, 1 is menu, 2+ are ordered levels
        var levelNum = scene.buildIndex - 1;

        var locString = GetComponent<LocalizeStringEvent>();
        locString.StringReference.Remove("LevelNumber");
        locString.StringReference.Add("LevelNumber", new IntVariable { Value = levelNum });
        locString.RefreshString();

        Destroy(this);
    }
}
