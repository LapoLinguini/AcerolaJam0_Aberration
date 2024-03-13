using UnityEngine;

public class HeightBall : MonoBehaviour
{
    [Header("Oscillation Parameters:")]
    [SerializeField] Transform ParticleHolder;
    [SerializeField] float _amplitude;
    [SerializeField] float _frequence;
    float Ypos;
    void Start()
    {
        Ypos = ParticleHolder.localPosition.y;
    }
    void Update()
    {
        ParticleHolder.localPosition = new Vector3(ParticleHolder.localPosition.x, Ypos + Mathf.Sin(Time.time * _frequence) * _amplitude, ParticleHolder.localPosition.z);
    }
}
