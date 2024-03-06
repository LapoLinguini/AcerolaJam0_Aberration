using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] Dialogue[] _DialogueSequence;

    [SerializeField] bool _delayBetweenDialogues;
    [SerializeField] float _dialoguesDelay = 0;

    public void TriggerDialogue()
    {
        float dialogueDelay = _delayBetweenDialogues ? _dialoguesDelay : 0;

        DialogueManager.Instance.StartDialogueSequence(_DialogueSequence, dialogueDelay);
    }
}
