using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] Animator anim;

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

    void LoadScene() => GameManager.Instance.LoadScene(_sceneToTransitionIndex);
    public void FadeIn() => anim.SetTrigger("FadeIn");
    public void FadeOut() => anim.SetTrigger("FadeOut");
}
