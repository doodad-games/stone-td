using MyLibrary;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Config/StoneTDConfig", fileName = "StoneTDConfig")]
public class StoneTDConfig : ScriptableObject
{
    static StoneTDConfig _i;
    public static StoneTDConfig I =>
        _i ??= Resources.Load<StoneTDConfig>("StoneTDConfig");
    
    public string[] orderedLevelSceneNames;

    public int GetThisLevelIndex() => orderedLevelSceneNames.IndexOf(
        SceneManager.GetActiveScene().name
    );
}
