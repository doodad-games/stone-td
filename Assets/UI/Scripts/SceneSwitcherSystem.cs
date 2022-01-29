using System;
using MyLibrary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcherSystem : MonoBehaviour
{
    public static event Action onSwitchedScene;

    public static SceneSwitcherSystem I { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() =>
        DontDestroyOnLoad(
            Instantiate(
                Resources.Load<GameObject>("Scene Switcher System")
            )
        );

    Animator _anim;
    string _sceneToLoad;

    void Awake() => _anim = GetComponent<Animator>();
    void OnEnable() => I = this;
    void OnDisable() => I = null;

    public void ReloadCurrentScene() =>
        Load(SceneManager.GetActiveScene().name);
    public void Load(string sceneName)
    {
        _sceneToLoad = sceneName;
        _anim.SetBool("Switching Scene", true);
    }

    public void Insp_OnCoveredScreen()
    {
        _anim.SetBool("Switching Scene", false);
        SceneManager.LoadScene(_sceneToLoad);
        
        new Async(this).Next(2).Then(onSwitchedScene.Invoke);
    }
}
