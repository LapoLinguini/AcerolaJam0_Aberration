using DG.Tweening;
using UnityEngine;

public class PuterFolder : MonoBehaviour
{
    [SerializeField] Transform _monitor;
    [SerializeField] GameObject _screenLight;

    private void OnEnable()
    {
        AnimEventsPlayer.OnPuterFolded += ClosePuter;
    }
    private void OnDisable()
    {
        AnimEventsPlayer.OnPuterFolded -= ClosePuter;
    }
    void ClosePuter()
    {
        _monitor.transform.DOLocalRotate(new Vector3(-180, 0, 0), 0.5f).SetEase(Ease.InSine).OnComplete(() => { _screenLight.SetActive(false); });
    }
}
