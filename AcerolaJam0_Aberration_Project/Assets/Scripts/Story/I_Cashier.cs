using DG.Tweening;
using Story;
using UnityEngine;

public class I_Cashier : MonoBehaviour
{
    [SerializeField] Collider _triggerColl;
    [SerializeField] M_Story_02 _storyManager2;
    bool _triggered = false;
    [SerializeField] PlayerMovement _player;
    [SerializeField] Transform CashierTalkPosT;


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

        TransitionManager.Instance.FadeOut();
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
            _player.SwitchControllerMode(ControllerMode.LookOnly);
            _storyManager2.StartDialogue(3);
        });
    }
}
