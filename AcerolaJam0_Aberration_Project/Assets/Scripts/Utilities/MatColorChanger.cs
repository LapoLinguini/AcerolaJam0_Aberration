using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MatColorChanger : MonoBehaviour
{
    MaterialPropertyBlock mpb;
    MeshRenderer mr;

    [SerializeField] Color color = Color.white;
    [SerializeField] int materialIndex = 1;

    private void OnValidate()
    {
        mr = GetComponent<MeshRenderer>();
        mpb = new MaterialPropertyBlock();
        mpb.SetColor("_Color", color);
        mr.SetPropertyBlock(mpb, materialIndex);
    }
    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        mpb = new MaterialPropertyBlock();
        mpb.SetColor("_Color", color);
        mr.SetPropertyBlock(mpb, materialIndex);
    }
}
