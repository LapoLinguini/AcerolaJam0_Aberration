using DG.Tweening;
using Story;
using UnityEngine;
using UnityEngine.UI;

public class I_Cashier : MonoBehaviour
{
    [SerializeField] Collider _triggerColl;
    [SerializeField] M_Story_02 _storyManager2;
    bool _triggered = false;
    [SerializeField] PlayerMovement _player;
    [SerializeField] Transform CashierTalkPosT;
    [SerializeField] GameObject _shelves;
    [SerializeField] Image _fadeOutPanel;

    private void OnEnable()
    {
        DialogueManager.OnDialogueFinished += ChangeScene;
    }
    private void OnDisable()
    {
        DialogueManager.OnDialogueFinished -= ChangeScene;
    }
    void ChangeScene()
    {
        if (!_triggered) return;

        if (GameManager.Instance._locationChangeCount == 1)
        {
            TransitionManager.Instance.FadeOut(Color.black);
        }
        else
        {
            if(_storyManager2._dialogueIndex == 4)
            {
                AudioManager.Instance.StopAllMusicFade(1f);
                Invoke(nameof(FinalTalk), 2f);
            }
            if (_storyManager2._dialogueIndex == 5)
            {
                Color newColor = Color.black;

                DOVirtual.Float(0, 1, 2f, a =>
                {
                    newColor.a = a;
                    _fadeOutPanel.color = newColor;
                });
                AudioManager.Instance.StopAllSFXLoopLerp(2f);
                Invoke(nameof(OutroTalk), 5f);
            }
            if (_storyManager2._dialogueIndex == 6)
            {
                Invoke(nameof(Credits), 2f);
            }
        }
    }
    void FinalTalk()
    {
        _storyManager2.StartDialogue(4);
    }
    void OutroTalk()
    {
        _storyManager2.StartDialogue(5);
    }
    void Credits()
    {
        TransitionManager.Instance._sceneToTransitionIndex = 5;
        TransitionManager.Instance.FadeOut(Color.black);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (DialogueManager.Instance._isTalking) return;

        _triggerColl.enabled = false;
        _triggered = true;

        _player.SwitchControllerMode(ControllerMode.Locked);
        _player.transform.DOMove(CashierTalkPosT.position, 1).SetEase(Ease.InOutSine);
        _player.transform.DORotate(CashierTalkPosT.eulerAngles, 1).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            _player.xRot = -_player._cameraPosT.localRotation.eulerAngles.x;
            _player.yRot = transform.eulerAngles.y;
            _player.SwitchControllerMode(ControllerMode.SoftLocked);
            _storyManager2.StartDialogue(3);
            _shelves.SetActive(false);
        });
    }
}
