using DG.Tweening;
using UnityEngine;

public class M_Story_02 : MonoBehaviour
{
    [Header("---DIALOGUES---")]
    [SerializeField] DialogueTrigger[] dialogueTriggers;

    [Header("---All Scenes---")]
    [SerializeField] GameObject FridgeScene;
    [SerializeField] GameObject ShelvesScene;
    [SerializeField] GameObject CashierScene;

    AudioSource _darkAmbient;

    void Start()
    {
        FridgeScene.SetActive(true);
        ShelvesScene.SetActive(false);
        CashierScene.SetActive(false);

        TransitionManager.Instance.FadeIn();
        TransitionManager.Instance._sceneToTransitionIndex = 2;

        AudioManager.Instance.PlayMusic(out _darkAmbient, "DarkAmbient", 0);
        DOVirtual.Float(_darkAmbient.volume, 0.2f, 10f, v => _darkAmbient.volume = v);
    }

    public void StartDialogue(int dialogueIndex)
    {
        dialogueTriggers[dialogueIndex].TriggerDialogue();
    }
}
