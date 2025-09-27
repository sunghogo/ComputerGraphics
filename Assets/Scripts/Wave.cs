using UnityEngine;

[System.Serializable]
public struct Wave
{
    [Tooltip("Height of the wave crest above the water plane")]
    public float amplitude;

    [Tooltip("Number of full cycles per world unit")]
    public float frequency;

    [Tooltip("Speed of the wave crest (world units per second)")]
    public float speed;

    [Tooltip("(Denormalized) Direction of travel on the XZ plane")]
    public Vector2 direction;

    // Derived values (computed once for efficiency)
    float s;    // scaled speed
    float k;    // radians per unit
    float w;    // angular velocity = k * S
    Vector2 d;  // normalized direction


    /// <summary>Call once to normalized direction and cache frequency and direction values.</summary>
    public void Initialize(float speedUp = 1f)
    {
        s = speed * speedUp;
        d = direction.normalized;
        k = 2f * Mathf.PI * frequency;
        w = k * s;
    }

    /// <summary>Calculate height at given horizontal point & time.</summary>
    public float GetHeight(Vector2 positionXZ, float time)
    {
        return amplitude * Mathf.Sin(Vector2.Dot(d, positionXZ) * k + time * w);
    }
}
