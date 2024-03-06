using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum DialogueType
{
    Neutral,
    Degenerate
}
public class DialogueManager : MonoBehaviour
{
    IA_Dialogue _dialogueInputs;
    InputAction IA_skipDialogue;

    [SerializeField] float _textBuildSpeed = 0.1f;

    [SerializeField] GameObject _dialogueUI;
    [SerializeField] TextMeshProUGUI _characterName;
    [SerializeField] TextMeshProUGUI _dialogueText;


    [SerializeField] Image _canSkipIcon;
    [SerializeField] Image _dialogueBox;

    [Header("Dialogue Types")]
    [SerializeField] Sprite _dialogueSpriteNormal;
    [SerializeField] Sprite _skipIconNormal;
    [SerializeField] Sprite[] _dialogueSpriteDegenerate;
    [SerializeField] Sprite[] _skipIconDegenerate;
    int _currentSprite = 0;
    Coroutine _spriteCycleCoroutine = null;

    Animation anim;

    Dialogue[] _currentDialogues;
    Message[] _currentMessages;
    string _currentMessage = "";
    bool _finishedShowingText = true;

    int _activeDialogue = 0;
    int _activeMessage = 0;

    float _dialoguesDelay = 0;

    public bool _isTalking { get; private set; } = false;
    bool _canSkip = false;

    Coroutine _currentBuildingText = null;
    public static DialogueManager Instance { get; private set; }

    public static Action OnDialogueFinished;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        _dialogueInputs = new IA_Dialogue();
    }
    private void OnEnable()
    {
        GameManager.OnGamePaused += GamePaused;

        IA_skipDialogue = _dialogueInputs.Dialogue.SkipText;
        IA_skipDialogue.performed += SkipText;
        IA_skipDialogue.Enable();
    }
    private void OnDisable()
    {
        GameManager.OnGamePaused -= GamePaused;
        IA_skipDialogue.Disable();
    }

    private void Start()
    {
        anim = GetComponent<Animation>();
        _dialogueUI.SetActive(false);
        _dialogueBox.gameObject.SetActive(false);
        _canSkipIcon.gameObject.SetActive(false);
    }
    void GamePaused(bool isPaused)
    {
        if (isPaused)
            IA_skipDialogue.Disable();
        else
            IA_skipDialogue.Enable();
    }
    public void StartDialogueSequence(Dialogue[] dialogues, float dialoguesDelay)
    {
        _currentDialogues = dialogues;
        _activeDialogue = 0;
        _activeMessage = 0;
        _dialoguesDelay = dialoguesDelay;
        _isTalking = true;
        DisplayMessage();
    }

    void DisplayMessage()
    {
        //Gets the current active message based on the current active dialogue
        Dialogue _dialogue = _currentDialogues[_activeDialogue];
        _currentMessages = _dialogue.messages;

        //Displays all the dialogue's info 
        _characterName.colorGradientPreset = _currentMessages[_activeMessage]._character._textGradient;
        _characterName.text = _currentMessages[_activeMessage]._character._name;
        _canSkipIcon.gameObject.SetActive(false);
        _currentBuildingText = StartCoroutine(ShowText());

        //Turns on the dialogue UI
        _dialogueUI.SetActive(true);
        SetTextUIType(_currentMessages[_activeMessage]._character._dialogueType);
        _dialogueBox.gameObject.SetActive(true);
    }
    IEnumerator ShowText()
    {
        _finishedShowingText = false;

        if (!_currentMessages[_activeMessage]._instantText)
        {
            for (int i = 0; i <= _currentMessages[_activeMessage]._message.Length; i++)
            {
                _currentMessage = _currentMessages[_activeMessage]._message.Substring(0, i);
                _dialogueText.text = _currentMessage;

                yield return new WaitForSeconds(_textBuildSpeed);
            }
            _finishedShowingText = true;
            _canSkip = true;
            _canSkipIcon.gameObject.SetActive(true);
            anim.Play();
        }
        else
        {
            _dialogueText.text = _currentMessages[_activeMessage]._message;
            _finishedShowingText = true;
            _canSkip = true;
            _canSkipIcon.gameObject.SetActive(true);
            anim.Play();
        }
    }
    IEnumerator NextMessage()
    {
        _activeMessage++;
        if (_activeMessage < _currentMessages.Length)
        {
            if (_currentBuildingText != null)
                StopCoroutine(_currentBuildingText);

            DisplayMessage();
        }
        else
        {
            _activeDialogue++;
            if (_activeDialogue < _currentDialogues.Length)
            {
                _dialogueBox.gameObject.SetActive(false);

                if (_currentBuildingText != null)
                    StopCoroutine(_currentBuildingText);

                yield return new WaitForSeconds(_dialoguesDelay);
                _activeMessage = 0;

                DisplayMessage();
            }
            else
            {
                _dialogueBox.gameObject.SetActive(false);

                if (_currentBuildingText != null)
                    StopCoroutine(_currentBuildingText);

                if (_spriteCycleCoroutine != null)
                    StopCoroutine(_spriteCycleCoroutine);

                yield return null;

                _isTalking = false;

                OnDialogueFinished?.Invoke();
            }
        }
    }
    void SetTextUIType(DialogueType dialogueType)
    {
        switch (dialogueType)
        {
            case DialogueType.Neutral:
                _dialogueBox.sprite = _dialogueSpriteNormal;
                _canSkipIcon.sprite = _skipIconNormal;

                if (_spriteCycleCoroutine != null)
                    StopCoroutine(_spriteCycleCoroutine);
                break;
            case DialogueType.Degenerate:

                _dialogueBox.sprite = _dialogueSpriteDegenerate[_currentSprite];
                _canSkipIcon.sprite = _skipIconDegenerate[_currentSprite];

                _spriteCycleCoroutine = StartCoroutine(SpriteCycle());
                break;
            default:
                break;
        }
    }
    IEnumerator SpriteCycle()
    {
        _currentSprite++;

        if (_currentSprite >= _dialogueSpriteDegenerate.Length)
            _currentSprite = 0;

        _dialogueBox.sprite = _dialogueSpriteDegenerate[_currentSprite];
        _canSkipIcon.sprite = _skipIconDegenerate[_currentSprite];

        yield return new WaitForSeconds(.15f);

        _spriteCycleCoroutine = StartCoroutine(SpriteCycle());
    }
    void SkipText(InputAction.CallbackContext context)
    {
        if (!_isTalking) return;

        if (_canSkip)
        {
            _canSkip = false;
            _canSkipIcon.gameObject.SetActive(false);
            anim.Stop();
            StartCoroutine(NextMessage());
            return;
        }

        if (!_finishedShowingText)
        {
            StopCoroutine(_currentBuildingText);
            _dialogueText.text = _currentMessages[_activeMessage]._message;
            _finishedShowingText = true;
            _canSkip = true;
            _canSkipIcon.gameObject.SetActive(true);
            anim.Play();
        }
    }
}
