using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("---MIXER GROUPS---")]
    [SerializeField] AudioMixer MainMixer;
    [SerializeField] AudioMixerGroup MusicMixerGroup;
    [SerializeField] AudioMixerGroup sfxMixerGroup;
    [Space]
    [Header("---CLIPS---")]
    [SerializeField] Sound[] musicClips;
    [SerializeField] Sound[] sfxClips;
    [SerializeField] Sound[] sfxLoopClips;
    [Space]
    [Header("---SOURCES(Read-only)---")]
    public AudioSource sfxSource;
    public List<AudioSource> musicSources, sfxLoopSource;

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
    }
    private void Start()
    {
        OnPauseSounds(false);
    }

    #region Music Func.

    public void PlayMusic(out AudioSource source, string name, float volume = 1)
    {
        Sound s = Array.Find(musicClips, x => x.name == name);
        if (s == null)
        {
            source = null;
            Debug.LogError("Sound Not Found");
        }
        else
        {
            source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = MusicMixerGroup;
            source.volume = volume;
            source.loop = true;
            source.clip = s.clip;
            source.Play();

            musicSources.Add(source);
        }
    }
    public void PlayMusicFadeIn(string name, float volume = 1, float fadeInTime = 1)
    {
        Sound s = Array.Find(musicClips, x => x.name == name);
        if (s == null)
        {
            Debug.LogError("Sound Not Found");
        }
        else
        {
            AudioSource source;
            source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = MusicMixerGroup;
            source.volume = 0;
            source.loop = true;
            source.clip = s.clip;
            source.Play();

            musicSources.Add(source);
            DOVirtual.Float(0, volume, fadeInTime, v => source.volume = v);
        }
    }
    public void IncreaseMusicVolume(float volumeIncrease, float increaseTime)
    {
        foreach (var source in musicSources)
        {
            float startVol = source.volume;
            DOVirtual.Float(startVol, startVol + volumeIncrease, increaseTime, v => source.volume = v);
        }
    }
    public void StopAllMusic()
    {
        foreach (var source in musicSources)
        {
            source.Stop();
            Destroy(source);
        }
        musicSources.Clear();
    }
    public IEnumerator StopAllMusicFade(float FadeTime = 1)
    {
        foreach (var source in musicSources)
        {
            float startVolume = source.volume;

            while (source.volume > 0)
            {
                source.volume -= startVolume * Time.deltaTime / FadeTime;

                yield return null;
            }

            source.Stop();
            Destroy(source);
        }
        musicSources.Clear();
    }
    public void ChangeMusic(string name, float volume, out AudioSource _source)
    {
        foreach (var source in musicSources)
        {
            source.Stop();
            Destroy(source);
        }
        PlayMusic(out _source, name, volume);
    }

    #endregion

    #region SFX Func.

    public void PlaySFX(string name, float volume = 1)
    {
        Sound s = Array.Find(sfxClips, x => x.name == name);
        if (s == null)
            Debug.LogError("Sound Not Found");
        else
        {
            AudioClip clip = s.clip;
            sfxSource.volume = volume;
            sfxSource.PlayOneShot(clip);
        }
    }
    public void PlaySFXRandomPitch(string name, float volume = 1, float minPitch = 0.75f, float maxPitch = 1.25f)
    {
        Sound s = Array.Find(sfxClips, x => x.name == name);
        if (s == null)
            Debug.LogError("Sound Not Found");
        else
        {
            AudioClip clip = s.clip;
            sfxSource.volume = volume;
            sfxSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            sfxSource.PlayOneShot(clip);
        }
    }

    #endregion

    #region Loop SFX Func.

    public void PlaySFXLoop(string name, float volume = 1)
    {
        Sound s = Array.Find(sfxLoopClips, x => x.name == name);
        if (s == null)
            Debug.LogError("Sound Not Found");
        else
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxMixerGroup;
            source.volume = volume;
            source.loop = true;
            source.clip = s.clip;
            source.Play();

            sfxLoopSource.Add(source);
        }
    }
    public void StopSFXLoop(string name)
    {
        foreach (var source in sfxLoopSource)
        {
            if (source.clip.name == name)
            {
                source.Stop();
                Destroy(source);
            }
        }
    }
    public void StopAllSFXLoop()
    {
        foreach (var source in sfxLoopSource)
        {
            source.Stop();
            Destroy(source);
        }
    }

    #endregion

    public void OnPauseSounds(bool pauseSounds)
    {
        switch (pauseSounds)
        {
            case true:
                AudioListener.pause = true;
                MainMixer.SetFloat("MusicCutoff", 1000);
                break;
            case false:
                AudioListener.pause = false;
                MainMixer.SetFloat("MusicCutoff", 22000);
                break;
        }
    }
    public void ToggleMuteSFX() => sfxMixerGroup.audioMixer.SetFloat("SFXVolume", -80);
    public void ToggleUnMuteSFX() => sfxMixerGroup.audioMixer.SetFloat("SFXVolume", 0);

    public void ToggleMuteMusic() => MusicMixerGroup.audioMixer.SetFloat("MusicVolume", -80);
    public void ToggleUnMuteMusic() => MusicMixerGroup.audioMixer.SetFloat("MusicVolume", 0);
}
