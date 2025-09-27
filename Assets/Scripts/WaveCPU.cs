using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaveCPU : MonoBehaviour
{
    [Header("Component Refs")]
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;

    [field: Header("Mesh Generation Settings")]
    [field: SerializeField, Range(1, 200)] public int MeshWidth { get; private set; } = 20;
    [field: SerializeField, Range(1, 200)] public int MeshHeight { get; private set; } = 20;
    [field: SerializeField, Range(1f, 20f)] public float MeshSize { get; private set; } = 1f;

    [field: Header("Wave Settings")]
    [field: SerializeField, Range(0f, 10f)] public float Speed { get; private set; } = 1f;
    [field: SerializeField] public List<Wave> Waves { get; private set; } = new List<Wave>();

    [Header("Gizmos Draw Settings")]
    [SerializeField] Color gizmosColor = Color.black;
    [SerializeField] float gizmosVertexSize = 0.02f;

    // Cached Inspector Settings
    int previousMeshWidth;
    int previousMeshHeight;
    float previousMeshSize;
    float previousSpeed;

    // Cached Mesh Properties
    Mesh mesh;
    Vector3[] baseVertices;
    Vector3[] displacedVertices;

    float time = 0f;

    void AnimateWave()
    {
        time += Time.deltaTime;

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 position = baseVertices[i];
            Vector2 XZ = new Vector2(position.x, position.z);

            float height = 0f;
            foreach (Wave wave in Waves)
                height += wave.GetHeight(XZ, time);

            displacedVertices[i] = new Vector3(position.x, height, position.z);
        }

        mesh.vertices = displacedVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void UpdateMesh()
    {
        if (previousMeshWidth != MeshWidth || previousMeshHeight != MeshHeight || previousMeshSize != MeshSize)
        {
            InitializeMesh();
        }
    }

    void UpdateSpeed()
    {
        if (previousSpeed != Speed)
        {
            previousSpeed = Speed;
            InitializeWaves();
        }
    }

    void RefreshInspectorChanges()
    {
        UpdateMesh();
        UpdateSpeed();
    }

    void InitializeMesh()
    {
        previousMeshWidth = MeshWidth;
        previousMeshHeight = MeshHeight;
        previousMeshSize = MeshSize;

        mesh = MeshGenerator.GeneratePlane(MeshWidth, MeshHeight, MeshSize);
        meshFilter.mesh = mesh;

        if (mesh != null)
        {
            baseVertices = mesh.vertices.Clone() as Vector3[];
            displacedVertices = new Vector3[baseVertices.Length];
        }
    }

    void InitializeWaves()
    {
        for (int i = 0; i < Waves.Count; ++i)
        {
            Wave wave = Waves[i];
            wave.Initialize(Speed);
            Waves[i] = wave;
        }
    }

    [ExecuteInEditMode]
    void OnDrawGizmos()
    {
        if (meshFilter == null || meshFilter.sharedMesh == null) return;

        Vector3[] verts = meshFilter.sharedMesh.vertices;
        foreach (Vector3 v in verts)
        {
            Gizmos.DrawSphere(transform.TransformPoint(v), gizmosVertexSize);
        }
    }

    void Awake()
    {
        Utils.InitComponent(this, ref meshFilter);
        Utils.InitComponent(this, ref meshRenderer);
        InitializeMesh();
        InitializeWaves();
        Gizmos.color = gizmosColor;
    }

    void Update()
    {
        AnimateWave();
        RefreshInspectorChanges();
    }
}
