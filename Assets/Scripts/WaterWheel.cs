using UnityEngine;

public class WaterWheel : MonoBehaviour
{
    [Header("Input Source")]
    [Tooltip("If true, Value is derived from local rotation angle. If false, uses Manual Value.")]
    public bool deriveFromRotation = true;

    [Range(0f, 1f)]
    [Tooltip("Used when deriveFromRotation = false")]
    public float manualValue01 = 0f;

    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    [Header("Rotation Mapping (when deriveFromRotation = true)")]
    public RotationAxis axis = RotationAxis.Z;

    [Tooltip("Angle mapped to 0.0 fill")] public float minAngleDeg = 0f;
    [Tooltip("Angle mapped to 1.0 fill")] public float maxAngleDeg = 360f;

    [Tooltip("Optionally clamp the evaluated angle between min and max before normalizing")]
    public bool clampAngleBeforeNormalize = false;

    public float Value
    {
        get
        {
            if (!deriveFromRotation)
            {
                return Mathf.Clamp01(manualValue01);
            }

            float angle = GetLocalAngleOnAxis();

            if (clampAngleBeforeNormalize)
            {
                angle = Mathf.Clamp(angle, Mathf.Min(minAngleDeg, maxAngleDeg), Mathf.Max(minAngleDeg, maxAngleDeg));
            }

            float value = Mathf.InverseLerp(minAngleDeg, maxAngleDeg, angle);
            return Mathf.Clamp01(value);
        }
    }

    float GetLocalAngleOnAxis()
    {
        Vector3 euler = transform.localEulerAngles;
        switch (axis)
        {
            case RotationAxis.X: return NormalizeAngle360(euler.x);
            case RotationAxis.Y: return NormalizeAngle360(euler.y);
            default: return NormalizeAngle360(euler.z);
        }
    }

    static float NormalizeAngle360(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }
}

