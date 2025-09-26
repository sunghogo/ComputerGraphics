using UnityEngine;

public static class MeshGenerator
{
    public static Mesh GeneratePlane(int widthSegments, int heightSegments, float size = 1f)
    {
        Mesh mesh = new Mesh();
        int vertCountX = widthSegments + 1;
        int vertCountY = heightSegments + 1;

        Vector3[] vertices = new Vector3[vertCountX * vertCountY];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[widthSegments * heightSegments * 6];

        for (int y = 0; y < vertCountY; y++)
        {
            for (int x = 0; x < vertCountX; x++)
            {
                int i = y * vertCountX + x;
                float u = (float)x / widthSegments;
                float v = (float)y / heightSegments;
                vertices[i] = new Vector3(
                    ((float)x / widthSegments - 0.5f) * size,
                    0,
                    ((float)y / heightSegments - 0.5f) * size
                );
                uvs[i] = new Vector2(u, v);
            }
        }

        int t = 0;
        for (int y = 0; y < heightSegments; y++)
        {
            for (int x = 0; x < widthSegments; x++)
            {
                int i = y * vertCountX + x;
                triangles[t++] = i;
                triangles[t++] = i + vertCountX;
                triangles[t++] = i + 1;
                triangles[t++] = i + 1;
                triangles[t++] = i + vertCountX;
                triangles[t++] = i + vertCountX + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
