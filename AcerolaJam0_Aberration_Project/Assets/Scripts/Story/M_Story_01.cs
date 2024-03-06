using DG.Tweening;
using Story;
using System.Collections;
using UnityEngine;

public class M_Story_01 : MonoBehaviour
{
    [SerializeField] GameObject MainMenuCanvas;
    [SerializeField] PlayerMovement player;
    [SerializeField] Transform chairPos;

    [Header("---CAMERAS---")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera menuCamera;

    [Header("---DiALOGUES---")]
    [SerializeField] DialogueTrigger[] dialogueTriggers;

    private void Start()
    {
        MainMenuCanvas.SetActive(!GameManager.Instance._gameStarted);

        Cursor.lockState = !GameManager.Instance._gameStarted ? CursorLockMode.None : CursorLockMode.Locked;

        CheckLocationChangeCount(GameManager.Instance._locationChangeCount);
    }
    private void OnEnable()
    {
        DialogueManager.OnDialogueFinished += DialogueFinished;
    }
    private void OnDisable()
    {
        DialogueManager.OnDialogueFinished -= DialogueFinished;
    }
    void DialogueFinished()
    {
        if (GameManager.Instance._locationChangeCount == 0)
            GameManager.Instance.LoadScene(1);
    }
    void CheckLocationChangeCount(int i)
    {
        switch (i)
        {
            case 0:
                player.SwitchControllerMode(ControllerMode.Locked);
                player.transform.position = chairPos.position;
                player.anim.SetBool("isPutering", true);
                menuCamera.enabled = true;
                playerCamera.enabled = false;
                break;
            case 3:
                player.SwitchControllerMode(ControllerMode.SoftLocked);
                break;
            case 6:
                player.SwitchControllerMode(ControllerMode.SoftLocked);
                break;
            default:
                break;
        }
    }

    public void StartCameraTransition(float transitionTime) => StartCoroutine(CameraTransition(transitionTime));
    IEnumerator CameraTransition(float transitionTime)
    {
        yield return new WaitForSeconds(0.5f);

        menuCamera.transform.DOMove(playerCamera.transform.position, transitionTime).SetEase(Ease.InOutQuad)
        .OnComplete(() => StartCoroutine(CameraTransitionFinished()));

        menuCamera.transform.DORotate(playerCamera.transform.eulerAngles, transitionTime).SetEase(Ease.InOutQuad);
    }
    IEnumerator CameraTransitionFinished()
    {
        playerCamera.transform.position = menuCamera.transform.position;
        playerCamera.transform.rotation = menuCamera.transform.rotation;

        playerCamera.enabled = true;
        menuCamera.enabled = false;

        player.SwitchControllerMode(ControllerMode.SoftLocked);

        yield return new WaitForSeconds(5f);

        dialogueTriggers[0].TriggerDialogue();
    }
}
