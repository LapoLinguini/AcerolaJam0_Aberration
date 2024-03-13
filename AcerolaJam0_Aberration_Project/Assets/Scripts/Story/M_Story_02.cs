using UnityEngine;

public class M_Story_02 : MonoBehaviour
{
    [Header("---DIALOGUES---")]
    [SerializeField] DialogueTrigger[] dialogueTriggers;

    [Header("---All Scenes---")]
    [SerializeField] GameObject FridgeScene;
    [SerializeField] GameObject ShelvesScene;
    [SerializeField] GameObject CashierScene;

    void Start()
    {
        FridgeScene.SetActive(true);
        ShelvesScene.SetActive(false);
        CashierScene.SetActive(false);

        TransitionManager.Instance.FadeIn();
        TransitionManager.Instance._sceneToTransitionIndex = 2;

        if (GameManager.Instance._locationChangeCount == 1)
            AudioManager.Instance.PlayMusicFadeIn("DarkAmbient", 0.2f, 10);
    }

    public void StartDialogue(int dialogueIndex)
    {
        dialogueTriggers[dialogueIndex].TriggerDialogue();
    }
}
