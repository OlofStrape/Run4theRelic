using UnityEngine;

public class FillGauge : MonoBehaviour
{
    [Header("Bar Parts")]
    [Tooltip("The transform that visually scales to represent fill (e.g., a quad)")]
    public Transform barTransform;

    [Tooltip("Optional: clamp incoming values to 0..1")]
    public bool clampInput = true;

    [Header("Axis Mapping")]
    [Tooltip("Axis to scale along")]
    public ScaleAxis scaleAxis = ScaleAxis.Y;
    
    public enum ScaleAxis { X, Y, Z }

    [Tooltip("Minimum local scale along axis when fill = 0")]
    public float minScale = 0f;

    [Tooltip("Maximum local scale along axis when fill = 1")]
    public float maxScale = 1f;

    [Tooltip("If true, scale pivot is assumed at bottom/left; we offset position to keep base fixed.")]
    public bool keepBaseFixed = true;

    [Tooltip("Local space direction used for positional offset when keeping base fixed.")]
    public Vector3 baseDirectionLocal = Vector3.up;

    float currentFill01 = 0f;
    Vector3 initialLocalScale;
    Vector3 initialLocalPosition;

    void Awake()
    {
        if (barTransform == null) barTransform = transform;
        initialLocalScale = barTransform.localScale;
        initialLocalPosition = barTransform.localPosition;
        ApplyScaleAndOffset(0f);
    }

    public void Set01(float fill01)
    {
        if (clampInput) fill01 = Mathf.Clamp01(fill01);
        currentFill01 = fill01;
        ApplyScaleAndOffset(fill01);
    }

    void ApplyScaleAndOffset(float t)
    {
        if (barTransform == null) return;

        float targetScale = Mathf.Lerp(minScale, maxScale, t);
        Vector3 scale = initialLocalScale;
        switch (scaleAxis)
        {
            case ScaleAxis.X: scale.x = targetScale; break;
            case ScaleAxis.Y: scale.y = targetScale; break;
            case ScaleAxis.Z: scale.z = targetScale; break;
        }
        barTransform.localScale = scale;

        if (keepBaseFixed)
        {
            float delta = targetScale - GetAxis(initialLocalScale, scaleAxis);
            Vector3 direction = baseDirectionLocal.normalized;
            barTransform.localPosition = initialLocalPosition + direction * (delta * 0.5f);
        }
    }

    static float GetAxis(Vector3 v, ScaleAxis axis)
    {
        switch (axis)
        {
            case ScaleAxis.X: return v.x;
            case ScaleAxis.Y: return v.y;
            default: return v.z;
        }
    }
}

