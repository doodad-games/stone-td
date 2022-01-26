using UnityEngine;

public class StartingScene : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] string _sceneToLoadInto;
#pragma warning restore CS0649

    void Awake() => SceneSwitcherSystem.I.Load(_sceneToLoadInto);
}
