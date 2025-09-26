using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Initialized a component field if it is not properly referenced.
    /// </summary>
    /// <typeparam name="T">Any Componentr</typeparam>
    /// <param name="owner">The component calling this utility (usually 'this')</param>
    /// <param name="mesh">Reference to the mesh to be set or retrieved</param>
    public static void InitComponent<T>(Component owner, ref T field) where T : Component
    {
        if (field != null) return;

        field = owner.GetComponent<T>();
        if (field == null) Debug.LogError($"{owner.name} has no {typeof(T)} component.");
    }
}
