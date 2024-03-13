using System.Collections;
using UnityEngine;

public class Glitcher : MonoBehaviour
{
    [SerializeField] GameObject _objectsToGlitch;
    [SerializeField] bool _glitchSetActive = false;
    [SerializeField] GameObject _optionalInverseObjects;

    [SerializeField] float _startingDelay = 0;
    [Header("X is for glitch duration / Y is for interval")]
    [SerializeField] GlitchSequence[] _glitchSequences;
    [SerializeField] Glitcher _nextGlitcher;
    int glitchIndex = 0;
    bool _finished = false;

    private void OnEnable()
    {
        DialogueManager.OnGlitchedEffect += StartGlitch;
    }
    private void OnDisable()
    {
        DialogueManager.OnGlitchedEffect -= StartGlitch;
    }
    void StartGlitch()
    {
        StartCoroutine(GlitchEffect());
    }
    IEnumerator GlitchEffect()
    {
        if (_finished) yield break;

        yield return new WaitForSeconds(_startingDelay);

        for (int i = 0; i < _glitchSequences[glitchIndex].glitchSequence.Length; i++)
        {
            GameManager.Instance.DarkenAtmosphere();
            _objectsToGlitch.SetActive(_glitchSetActive);
            if (_optionalInverseObjects != null)
                _optionalInverseObjects.SetActive(!_glitchSetActive);
            AudioManager.Instance.PlaySFX("GlitchEffect", 0.25f);

            yield return new WaitForSeconds(_glitchSequences[glitchIndex].glitchSequence[i]._duration);

            _objectsToGlitch.SetActive(!_glitchSetActive);
            if (_optionalInverseObjects != null)
                _optionalInverseObjects.SetActive(_glitchSetActive);
            GameManager.Instance.RestoreAtmoshpere();
            AudioManager.Instance.StopSFX();

            yield return new WaitForSeconds(_glitchSequences[glitchIndex].glitchSequence[i]._interval);
        }
        glitchIndex++;

        if (glitchIndex == _glitchSequences.Length)
        {
            _finished = true;
            if (_nextGlitcher != null)
                _nextGlitcher.enabled = true;
        }
    }
}
