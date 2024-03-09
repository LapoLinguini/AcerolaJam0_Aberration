using DG.Tweening;
using Story;
using UnityEngine;

public class I_MilkBrick : MonoBehaviour, IInteractable
{
    MaterialPropertyBlock mpb;

    [Header("INTERACTABLE")]
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] int _materialIndex;
    [SerializeField] Color _interactedColor = Color.yellow;
    [SerializeField] M_Story_02 storyManager2;
    [SerializeField] PlayerMovement _player;
    [SerializeField] Transform _milkPickUpT;
    [SerializeField] GameObject _cashier;

    public bool _isInteractable { get; set; } = true;

    private void OnEnable()
    {
        AnimEventsPlayer.OnMilkPickedUp += ReparentMilk;
        AnimEventsPlayer.OnAnimationEnded += MilkDialogue;
    }
    private void OnDisable()
    {
        AnimEventsPlayer.OnMilkPickedUp -= ReparentMilk;
        AnimEventsPlayer.OnAnimationEnded -= MilkDialogue;
    }
    void MilkDialogue()
    {
        storyManager2.StartDialogue(2);
        _cashier.SetActive(true);
    }
    void ReparentMilk()
    {
        transform.parent = _player._rightHandT;
        Shelve.canDisappear = true;
    }
    public void Interact()
    {
        if (!_isInteractable) return;

        _player.SwitchControllerMode(ControllerMode.Locked);
        _player.transform.DOMove(_milkPickUpT.position, 1).SetEase(Ease.InOutSine);
        _player.transform.DORotate(new Vector3(0, 180, 0), 1).SetEase(Ease.InOutSine);
        Invoke(nameof(StartPickUpAnim), 1);

        _isInteractable = false;
    }
    void StartPickUpAnim()
    {
        _player.anim.SetTrigger("PickUp");
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
}
