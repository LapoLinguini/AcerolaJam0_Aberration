using System.Collections;
using UnityEditor;
using UnityEngine;

public enum LightFlickerModes { Position, Intensity, Both }
public class LightFlickerer : MonoBehaviour
{

    public LightFlickerModes _lightFlickerMode = LightFlickerModes.Both;

    public float _intensityIncrease = 0.2f;
    public float _intensityDecrease = 0.2f;
    public float _positionIncrease = 0.2f;
    public float _positionDecrease = 0.2f;
    public float _flickerRate = 0.1f;

    bool _canFlicker = true;

    public Light _lightSource;
    private float _baseIntensity = 1f;

    Vector3 baseLightPos;
    Vector3 newLightPos;

    public void Reset()
    {
        _intensityDecrease = 0.2f;
        _intensityIncrease = 0.2f;
        _flickerRate = 0.1f;
    }

    public void Start()
    {
        if (_lightSource == null) return;

        _lightSource = GetComponentInChildren<Light>();

        _baseIntensity = _lightSource.intensity;
        baseLightPos = _lightSource.transform.position;
        StartCoroutine(DoFlicker());
    }

    [ContextMenu("DO FLICKER")]
    private IEnumerator DoFlicker()
    {
        if (!_canFlicker || _lightSource == null) yield break;

        switch (_lightFlickerMode)
        {
            case LightFlickerModes.Position:
                PositionChange();
                break;
            case LightFlickerModes.Intensity:
                IntensityChange();
                break;
            case LightFlickerModes.Both:
                IntensityChange();
                PositionChange();
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(_flickerRate);

        StartCoroutine(DoFlicker());
    }
    void IntensityChange() => _lightSource.intensity = Random.Range(_baseIntensity - _intensityDecrease, _baseIntensity + _intensityIncrease);
    void PositionChange()
    {
        newLightPos = baseLightPos;
        newLightPos.y += Random.Range(-_intensityDecrease, _intensityIncrease);
        _lightSource.transform.position = newLightPos;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(LightFlickerer)), CanEditMultipleObjects]
public class LightFlickererEditor : Editor
{
    LightFlickerer lf;

    private void OnEnable() => lf = target as LightFlickerer;

    public override void OnInspectorGUI()
    {
        lf._lightSource = (Light)EditorGUILayout.ObjectField("Light Source" ,lf._lightSource, typeof(Light), true);

        EditorGUILayout.Space(10);

        if (lf._lightSource != null)
        {
            lf._lightFlickerMode = (LightFlickerModes)EditorGUILayout.EnumPopup("Light Flickerer Mode", lf._lightFlickerMode);

            EditorGUILayout.Space(10);

            switch (lf._lightFlickerMode)
            {
                case LightFlickerModes.Position:
                    DrawPosition();
                    break;
                case LightFlickerModes.Intensity:
                    DrawIntensity();
                    break;
                case LightFlickerModes.Both:
                    DrawPosition();

                    EditorGUILayout.Space(10);

                    DrawIntensity();
                    break;
                default:
                    break;
            }

            EditorGUILayout.Space(10);

            lf._flickerRate = EditorGUILayout.FloatField("Flicker Rate", lf._flickerRate);
        }

        if (GUI.changed)
            EditorUtility.SetDirty(lf);
    }
    void DrawIntensity()
    {
        lf._intensityIncrease = EditorGUILayout.FloatField("Intensity Increase", lf._intensityIncrease);
        lf._intensityDecrease = EditorGUILayout.FloatField("Intensity Decrease", lf._intensityDecrease);
    }
    void DrawPosition()
    {
        lf._positionIncrease = EditorGUILayout.FloatField("Position Increase", lf._positionIncrease);
        lf._positionDecrease = EditorGUILayout.FloatField("Position Decrease", lf._positionDecrease);
    }
}
#endif