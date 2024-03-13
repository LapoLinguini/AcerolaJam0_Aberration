using DG.Tweening;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Framerate Debugger")]
    [SerializeField] bool _capFrameRate = false;
    [SerializeField] int _targetFrameRate = 60;

    [Header("Game Progression Variables")]
    public bool _gameStarted = false;
    public int _locationChangeCount = 0;

    [Header("Post Processing")]
    [SerializeField] PostProcessProfile _storyPostProcess;
    ColorGrading _storyColorGrading;
    float _currentSaturationValue = 0;
    Tweener _saturationTween;
    Vignette _storyVignette;
    float _currentVignetteValue = 0;
    Tweener _vignetteTween;

    [SerializeField] PostProcessProfile _duneGameplayPostProcess;

    [Header("Scene Loader")]
    public int _sceneToLoadIndex = 0;

    bool _gameIsPaused = false;
    float _currentGameTime = 1;
    public static GameManager Instance { get; private set; }

    public static Action<bool> OnGamePaused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        CapFrameRate();

        _storyPostProcess.TryGetSettings(out _storyColorGrading);
        _storyPostProcess.TryGetSettings(out _storyVignette);
        RestoreAtmoshpere();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _gameStarted)
            PauseGame();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Time.timeScale = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Time.timeScale = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            Time.timeScale = 4;
        if (Input.GetKeyDown(KeyCode.Alpha6))
            Time.timeScale = 6;
    }
    public void DarkenAtmosphereLerp(int saturationLoss, float vignetteGain)
    {
        float currentSaturation = _storyColorGrading.saturation.value;
        float currentVignette = _storyVignette.intensity.value;

        _currentSaturationValue -= Mathf.Abs(saturationLoss);
        _currentVignetteValue += Mathf.Abs(vignetteGain);

        if (_saturationTween != null)
            _saturationTween.Kill();

        if (_vignetteTween != null)
            _vignetteTween.Kill();

        _saturationTween = DOVirtual.Float(currentSaturation, _currentSaturationValue, 2.5f, s => _storyColorGrading.saturation.value = s);
        _vignetteTween = DOVirtual.Float(currentVignette, _currentVignetteValue, 2.5f, v => _storyVignette.intensity.value = v);
    }
    public void DarkenAtmosphere(int saturation = -70, float vignette = 0.5f)
    {
        _storyColorGrading.saturation.value = saturation;
        _storyVignette.intensity.value = vignette;
    }
    public void RestoreAtmoshpere()
    {
        _storyColorGrading.saturation.value = 0;
        _currentSaturationValue = 0;
        _storyVignette.intensity.value = 0.25f;
        _currentVignetteValue = 0.25f;
    }
    public void LoadScene(int sceneIndex) => StartCoroutine(LoadSceneAsync(sceneIndex));
    IEnumerator LoadSceneAsync(int index)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);

        _locationChangeCount++;
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    public void PauseGame()
    {
        _gameIsPaused = !_gameIsPaused;

        Time.timeScale = _gameIsPaused ? 0 : _currentGameTime;

        Cursor.lockState = _gameIsPaused ? CursorLockMode.None : CursorLockMode.Locked;

        OnGamePaused?.Invoke(_gameIsPaused);

        PauseMenu.Instance.SwitchMenuUI(_gameIsPaused);
    }
    void CapFrameRate()
    {
        if (!_capFrameRate)
        {
            QualitySettings.vSyncCount = 1;
            return;
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _targetFrameRate;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
class GameManagerInspector : Editor
{
    GameManager gm;
    void OnEnable() => gm = target as GameManager;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Load Scene"))
        {
            gm._gameStarted = true;
            TransitionManager.Instance._sceneToTransitionIndex = gm._sceneToLoadIndex;
            TransitionManager.Instance.FadeOut(Color.black);
        }

        if (GUI.changed)
            EditorUtility.SetDirty(gm);
    }
}
#endif
