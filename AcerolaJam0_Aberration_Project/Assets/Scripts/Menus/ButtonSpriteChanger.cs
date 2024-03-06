using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSpriteChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image _imageTarget;
    [SerializeField] Sprite[] _allChangingSprites;
    [SerializeField] float _swapInterval = 0.5f;

    int _currentSprite = 0;

    Coroutine _cycleCoroutine = null;

    private void Start()
    {
        _imageTarget.sprite = _allChangingSprites[0];
        _imageTarget.preserveAspect = true;
    }

    #region SpriteCycle

    public void OnPointerEnter(PointerEventData eventData)
    {
        _cycleCoroutine = StartCoroutine(SpriteCycle(_swapInterval));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_cycleCoroutine != null)
            StopCoroutine(_cycleCoroutine);
    }
    IEnumerator SpriteCycle(float time)
    {
        _currentSprite++;

        if (_currentSprite >= _allChangingSprites.Length)
            _currentSprite = 0;

        _imageTarget.sprite = _allChangingSprites[_currentSprite];

        yield return new WaitForSeconds(time);

        _cycleCoroutine = StartCoroutine(SpriteCycle(time));
    } 
    #endregion

    public void StartGame()
    {
        GameManager.Instance._gameStarted = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Options()
    {

    }
    public void Credits()
    {
        print("Credits");
    }
}
