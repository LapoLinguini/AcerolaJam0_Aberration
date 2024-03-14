using UnityEngine;

[System.Serializable]
public struct Message
{
    [Header("Dialogue")]
    public Character _character;
    [TextArea(3, 10)]
    public string _message;
    public bool _instantText;
    public bool _isAutomatic;
    public float _skipTime;

    [Header("Effects")]
    public int _saturationLoss;
    public float _vignetteGain;
    public bool _glitch;
    public bool _heartBeat;
    public float _heartBeatRate;

    [Header("Volume")]
    public float _increaseMusicVol;
    public float _increaseMusicTime;
    public float _increaseSFXVol;
    public float _increaseSFXTime;
    public bool _stopMusic;
    public string _startMusic;
    public float _startVolume;
    public float _startTime;
}
