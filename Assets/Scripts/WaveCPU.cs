using UnityEngine;
using System.Collections.Generic;

public class WaveCPU: MonoBehaviour
{
    [Header("Component Refs")]
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;

    [field: Header("Mesh Generation Settings")]
    [field: SerializeField, Range(1, 200)] public int MeshWidth { get; private set; } = 20;
    [field: SerializeField, Range(1, 200)] public int MeshHeight { get; private set; } = 20;
    [field: SerializeField, Range(1f, 10f)] public float MeshSize { get; private set; } = 1f;

    int previousMeshWidth;
    int previousMeshHeight;
    float previousMeshSize;

    [field: Header("Wave Settings")]
    [field: SerializeField, Range(0f, 10f)] public float Speed { get; private set; } = 1f;
    [field: SerializeField] public List<float> Amplitudes { get; private set; } = new List<float> { 1f };
    [field: SerializeField] public List<float> Frequencies { get; private set; } = new List<float> { 1f };

    [Header("Gizmos Draw Settings")]
    [SerializeField] Color gizmosColor = Color.black;
    [SerializeField] float gizmosVertexSize = 0.02f;

    Mesh mesh;
    Vector3[] baseVertices;
    Vector3[] displacedVertices;

    float waveTimer = 0f;

    void AnimateWave()
    {
        waveTimer += Time.deltaTime;

        for (int i = 0; i < baseVertices.Length; ++i)
        {
            Vector3 pos = baseVertices[i];

            float wave = 0f;
            int numWaves = Mathf.Min(Amplitudes.Count, Frequencies.Count);

            float halfWidth = MeshSize * 0.5f;
            float normX = (pos.x + halfWidth) / MeshSize;

            for (int j = 0; j < numWaves; ++j)
            {
                float f = Frequencies[j] * 2f * Mathf.PI;
                float a = Amplitudes[j];
                float p = Speed * f;
                float t = waveTimer;
                wave += a * Mathf.Sin(normX * f + t * p);
            }

            displacedVertices[i] = new Vector3(pos.x, pos.y + wave, pos.z);
        }

        mesh.vertices = displacedVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void UpdateMesh() {
        if (previousMeshWidth != MeshWidth || previousMeshHeight != MeshHeight || previousMeshSize != MeshSize) {
            InitMesh();
        }
    } 

    void RefreshInspectorChanges() {
        UpdateMesh();
    }

    void InitMesh()
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
        InitMesh();

        Gizmos.color = gizmosColor;
    }

    void Update()
    {
        AnimateWave();
        RefreshInspectorChanges();
    }
}
