using UnityEngine;

public class Credits : MonoBehaviour
{
    private void Start()
    {
        TransitionManager.Instance.FadeIn();
        GameManager.Instance._finished = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void Quit()
    {
        Application.Quit();
    }
}
