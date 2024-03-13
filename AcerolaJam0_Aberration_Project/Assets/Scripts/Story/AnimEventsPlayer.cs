using Story;
using System;
using UnityEngine;

public class AnimEventsPlayer : MonoBehaviour
{
    [SerializeField] PlayerMovement _player;

    public static Action OnPuterFolded;
    public static Action OnMilkPickedUp;
    public static Action OnAnimationEnded;

    void ClosePuter() => OnPuterFolded?.Invoke();
    void PickUpMilk() => OnMilkPickedUp?.Invoke();
    void LaptopKeyPressed() => AudioManager.Instance.PlaySFXRandomPitch("KeyPress", 0.1f);
    void FootstepSound(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > 0.5f)
        {
            AudioManager.Instance.PlaySFXRandomPitch("Footstep", 0.07f);
        }
    }
    void AnimationEnded()
    {
        _player.SwitchControllerMode(ControllerMode.Free);
        OnAnimationEnded?.Invoke();
    }
    void StartFadeOut() => TransitionManager.Instance.FadeOut(Color.black);
}
