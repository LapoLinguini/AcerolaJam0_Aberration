using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] Animator anim;
    [SerializeField] Image _transitionPanel;

    public int _sceneToTransitionIndex = 0;

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
    }

    void LoadScene()
    {
        print("NEW SCENE LOADED: " + _sceneToTransitionIndex);
        GameManager.Instance.LoadScene(_sceneToTransitionIndex);
    }
    public void FadeIn()
    {
        anim.SetTrigger("FadeIn");
    }
    public void FadeOut(Color color)
    {
        _transitionPanel.color = color;

        anim.SetTrigger("FadeOut");
    }
}
