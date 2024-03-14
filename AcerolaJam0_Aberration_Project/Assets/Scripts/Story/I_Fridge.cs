using DG.Tweening;
using Story;
using System.Collections;
using UnityEngine;

public class I_Fridge : MonoBehaviour, IInteractable
{
    MaterialPropertyBlock mpb;

    [Header("INTERACTABLE")]
    [SerializeField] Transform _fridgeDoor;
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] int _materialIndex;
    [SerializeField] Color _interactedColor = Color.yellow;
    [SerializeField] M_Story_02 storyManager2;
    [SerializeField] Transform _fridgeOpenPosT;
    public bool _isInteractable { get; set; } = true;

    [Header("DEACTIVATION")]
    [SerializeField] GameObject ShelveScene;
    [SerializeField] GameObject FridgeScene;
    [SerializeField] Transform _fridgeDisableChecker;
    [SerializeField] PlayerMovement _player;
    bool canDisappear = false;

    private void Start()
    {
        ShelveScene.SetActive(false);
    }
    private void OnEnable()
    {
        DialogueManager.OnDialogueFinished += SwitchPlayerController;
    }
    private void OnDisable()
    {
        DialogueManager.OnDialogueFinished -= SwitchPlayerController;
    }
    public void Interact()
    {
        if (!_isInteractable) return;

        OpenFridgeDoor();

        _isInteractable = false;
    }
    private void FixedUpdate()
    {
        if (_isInteractable) return;

        if (canDisappear && _player.transform.position.z < _fridgeDisableChecker.position.z && Vector3.Dot(_fridgeDisableChecker.forward, _player.transform.forward) >= 0.6f)
        {
            FridgeScene.SetActive(false);
        }
    }
    public void Interactable(bool _isInteractable)
    {
        mpb = new MaterialPropertyBlock();
        if (!this._isInteractable)
        {
            mpb.SetColor("_Color", Color.white);
            _renderer.SetPropertyBlock(mpb, _materialIndex);
            return;
        }

        if (_isInteractable)
        {
            mpb.SetColor("_Color", _interactedColor);
            _renderer.SetPropertyBlock(mpb, _materialIndex);
        }
        else
        {
            mpb.SetColor("_Color", Color.white);
            _renderer.SetPropertyBlock(mpb, _materialIndex);
        }
    }

    void OpenFridgeDoor()
    {
        ShelveScene.SetActive(true);

        _player.SwitchControllerMode(ControllerMode.Locked, -10);
        _player.transform.DOMove(_fridgeOpenPosT.position, 2).SetEase(Ease.InOutSine);
        _player.transform.DORotate(Vector3.zero, 2).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            _player.yRot = _player.transform.eulerAngles.y;
            _player.xRot = -_player._cameraPosT.localRotation.eulerAngles.x;
        });

        _fridgeDoor.DOLocalRotate(new Vector3(0, 0, -117), 2.5f).SetEase(Ease.InOutSine).OnComplete(() => StartCoroutine(StartDialogue()));
    }
    IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(.75f);

        storyManager2.StartDialogue(0);
    }
    void SwitchPlayerController()
    {
        canDisappear = true;
        _player.SwitchControllerMode(ControllerMode.Free);
    }
}
