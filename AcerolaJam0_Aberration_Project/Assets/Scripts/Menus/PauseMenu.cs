using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [Header("Menu")]
    [SerializeField] GameObject PauseMenuUI;

    [Header("Audio:")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider MusicSlider;
    [SerializeField] Slider SoundSlider;

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
        PauseMenuUI.SetActive(false);


    }
    private void Start()
    {
        SetMasterVolume(1);
        MasterSlider.value = 1;
        SetMusicVolume(.5f);
        MusicSlider.value = .5f;
        SetSoundVolume(.5f);
        SoundSlider.value = .5f;
    }
    public void SwitchMenuUI(bool _active) => PauseMenuUI.SetActive(_active);
    public void Resume() => GameManager.Instance.PauseGame();
    public void SetMasterVolume(float masterVolume)
    {
        float finalVol = Mathf.Log10(masterVolume) * 20;

        if (masterVolume == 0) finalVol = -80;

        audioMixer.SetFloat("MasterVol", finalVol);
    }
    public void SetMusicVolume(float musicVolume)
    {
        float finalVol = Mathf.Log10(musicVolume) * 20;

        if (musicVolume == 0) finalVol = -80;

        audioMixer.SetFloat("MusicVol", finalVol);
    }
    public void SetSoundVolume(float soundVolume)
    {
        float finalVol = Mathf.Log10(soundVolume) * 20;

        if (soundVolume == 0) finalVol = -80;

        audioMixer.SetFloat("SoundVol", finalVol);
    }
}
