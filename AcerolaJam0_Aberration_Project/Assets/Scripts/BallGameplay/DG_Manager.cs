using DG.Tweening;
using DunesGameplay;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DG_Manager : MonoBehaviour
{
    [Header("---DIALOGUES---")]
    [SerializeField] DialogueTrigger[] dialogueTriggers;

    [Header("STUFF")]
    [SerializeField] GameObject _spawnParticleEffect;
    [SerializeField] BallMovement _ballMovement;

    [Header("Gameplay")]
    [SerializeField] Transform[] _heightTriggers;
    [SerializeField] GameObject _heightOrb;
    [SerializeField] LineWaves _lineTrack;
    [SerializeField] Image _glowPanel;



    bool _canFollow = false;
    bool _gameplayStarted = false;
    bool _canGoHeigher = false;
    int heightIndex = 0;
    bool _finished = false;

    private void Start()
    {
        _heightOrb.SetActive(false);
        StartDialogue(0);
        Color newColor = _glowPanel.color;
        newColor.a = 0;
        _glowPanel.color = newColor;
    }
    private void OnEnable()
    {
        DialogueManager.OnDialogueFinished += DialogueFinished;
    }
    private void OnDisable()
    {
        DialogueManager.OnDialogueFinished -= DialogueFinished;
    }
    private void FixedUpdate()
    {
        if (_canFollow)
            _heightOrb.transform.position = new Vector3(_ballMovement.transform.position.x, _heightOrb.transform.position.y);

        if (heightIndex < _heightTriggers.Length)
        {
            if (_canGoHeigher && _ballMovement.transform.position.y >= _heightTriggers[heightIndex].position.y)
            {
                _canGoHeigher = false;

                heightIndex++;
                if (heightIndex < _heightTriggers.Length)
                {
                    HeightOrbRepositionY(_heightTriggers[heightIndex].position.y, 1.5f);
                    switch (heightIndex)
                    {
                        default:
                            break;
                        case 1:
                            GameManager.Instance._heartbeatInterval = .7f;
                            AdjustTrackStats(4, 1.5f, .75f);
                            break;
                        case 2:
                            GameManager.Instance._heartbeatInterval = .9f;
                            AdjustTrackStats(6, 1f, .75f);
                            break;
                    }
                }
            }
        }
        else
        {
            if (!_finished)
                Finished();
        }
    }
    void AdjustTrackStats(float amplitude, float frequency, float time)
    {
        bool isHalf = false;

        Color newColor = _glowPanel.color;
        newColor.a = 0;

        var tween = DOVirtual.Float(0, 1, time, a =>
        {
            newColor.a = a;
            _glowPanel.color = newColor;

        }).SetEase(Ease.InOutQuart).SetLoops(2, LoopType.Yoyo).OnStepComplete(() =>
        {
            if (!isHalf)
            {
                isHalf = true;
                _lineTrack._amplitude = amplitude;
                _lineTrack._frequency = frequency;
                StartDialogue(heightIndex);
            }
        });
    }
    void Finished()
    {
        GameManager.Instance._heartbeatInterval = 1.25f;

        _finished = true;

        _ballMovement._canMove = false;

        _heightOrb.GetComponentInChildren<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);

        _ballMovement.LerpColor();
    }
    void HeightOrbRepositionY(float Ycoord, float time)
    {
        _heightOrb.transform.DOMoveY(Ycoord, time).SetEase(Ease.InOutSine).OnComplete(() => { _canGoHeigher = true; _canFollow = true; });
    }
    public void StartDialogue(int dialogueIndex)
    {
        dialogueTriggers[dialogueIndex].TriggerDialogue();
    }
    void DialogueFinished()
    {
        if (!_gameplayStarted)
        {
            TransitionManager.Instance.FadeIn();
            _gameplayStarted = true;
            StartCoroutine(SpawnParticleEffect());
            return;
        }
    }

    IEnumerator SpawnParticleEffect()
    {
        yield return new WaitForSeconds(3);

        GameObject spawnParticleEffect = Instantiate(_spawnParticleEffect, _ballMovement.transform.position, Quaternion.Euler(-180, 0, 0));

        AudioManager.Instance.StopAllMusicFade(12f);

        yield return new WaitForSeconds(3);

        _heightOrb.SetActive(true);

        yield return new WaitForSeconds(5);

        GameManager.Instance._heartbeatLoop = StartCoroutine(GameManager.Instance.Heartbeat());

        HeightOrbRepositionY(_heightTriggers[heightIndex].position.y, 3);

        _ballMovement.Init(4f);

        spawnParticleEffect.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmitting);
    }
}
