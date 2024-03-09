using Story;
using UnityEngine;

public class Shelve : MonoBehaviour
{
    [SerializeField] Collider _triggerColl;
    [SerializeField] M_Story_02 _storyManager2;
    [SerializeField] GameObject _milkBrick;
    bool canSpawnMilk = false;
    bool triggerEntered = false;

    [Header("DEACTIVATION")]
    [SerializeField] GameObject _ShelveScene;
    [SerializeField] Transform _sceneDisableChecker;
    [SerializeField] PlayerMovement _player;
    public static bool canDisappear = false;

    private void Start()
    {
        _milkBrick.SetActive(false);
    }
    private void OnEnable()
    {
        DialogueManager.OnDialogueFinished += SpawnMilk;
    }
    private void OnDisable()
    {
        DialogueManager.OnDialogueFinished -= SpawnMilk;
    }
    private void FixedUpdate()
    {
        if (_player.transform.position.z < transform.position.z && Mathf.Abs(_player.transform.position.x - transform.position.x) < 0.5f && canSpawnMilk)
        {
            canSpawnMilk = false;
            _milkBrick.SetActive(true);
        }

        if (canDisappear && _player.transform.position.z < _sceneDisableChecker.position.z && Vector3.Dot(_sceneDisableChecker.forward, _player.transform.forward) >= 0.6f)
        {
            _ShelveScene.SetActive(false);
        }
    }
    void SpawnMilk()
    {
        if (!triggerEntered && !canSpawnMilk) return;

        canSpawnMilk = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        _storyManager2.StartDialogue(1);
        _triggerColl.enabled = false;
        triggerEntered = true;
    }
}
