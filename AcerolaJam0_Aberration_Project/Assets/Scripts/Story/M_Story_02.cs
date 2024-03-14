using UnityEngine;

public class M_Story_02 : MonoBehaviour
{
    [Header("---DIALOGUES---")]
    [SerializeField] DialogueTrigger[] dialogueTriggers;

    [Header("---All Scenes---")]
    [SerializeField] GameObject FridgeScene;
    [SerializeField] GameObject ShelvesScene;
    [SerializeField] GameObject CashierScene;

    public int _dialogueIndex = 0;

    void Start()
    {
        FridgeScene.SetActive(true);
        ShelvesScene.SetActive(false);
        CashierScene.SetActive(false);

        TransitionManager.Instance.FadeIn();

        if (GameManager.Instance._locationChangeCount == 1)
        {
            TransitionManager.Instance._sceneToTransitionIndex = 2;
            AudioManager.Instance.PlayMusicFadeIn("DarkAmbient", 0.2f, 10);
        }
    }

    public void StartDialogue(int dialogueIndex)
    {
        _dialogueIndex++;
        dialogueTriggers[dialogueIndex].TriggerDialogue();
    }
}
