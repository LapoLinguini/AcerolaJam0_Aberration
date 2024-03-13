using UnityEngine;

[System.Serializable]
public struct Message
{
    [Header("Dialogue")]
    public Character _character;
    [TextArea(3, 10)]
    public string _message;
    public bool _instantText;

    [Header("Effects")]
    public int _saturationLoss;
    public float _vignetteGain;
    public bool _glitch;

    [Header("Volume")]
    public float _increaseMusicVol;
    public float _increaseMusicTime;
    public bool _stopMusic;
    public string _startMusic;
    public float _startVolume;
}
