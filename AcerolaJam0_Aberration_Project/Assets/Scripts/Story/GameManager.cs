using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Framerate Debugger")]
    [SerializeField] bool _capFrameRate = false;
    [SerializeField] int _targetFrameRate = 60;

    public static Action<bool> OnGamePaused;
    bool _gameIsPaused = false;
    float _currentGameTime = 1;

    private void Start()
    {
        CapFrameRate();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _gameIsPaused = !_gameIsPaused;

            Time.timeScale = _gameIsPaused ? 0 : _currentGameTime;

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
