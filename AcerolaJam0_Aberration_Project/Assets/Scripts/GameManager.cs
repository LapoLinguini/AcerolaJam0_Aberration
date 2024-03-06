using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Framerate Debugger")]
    [SerializeField] bool _capFrameRate = false;
    [SerializeField] int _targetFrameRate = 60;

    [Header("Game Progression Variables")]
    public bool _gameStarted = false;
    public int _locationChangeCount = 0;

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
    }
    private void Update()
    {
        PauseGame();
    }
    public void LoadScene(int sceneIndex) => StartCoroutine(LoadSceneAsync(sceneIndex));
    IEnumerator LoadSceneAsync(int index)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        _locationChangeCount++;
    }
    void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _gameStarted)
        {
            _gameIsPaused = !_gameIsPaused;

            Time.timeScale = _gameIsPaused ? 0 : _currentGameTime;

            Cursor.lockState = _gameIsPaused ? CursorLockMode.None : CursorLockMode.Locked;

            OnGamePaused?.Invoke(_gameIsPaused);
        }
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
            gm.LoadScene(gm._sceneToLoadIndex);

        if (GUI.changed)
            EditorUtility.SetDirty(gm);
    }
}
#endif
