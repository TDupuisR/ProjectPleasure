using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeshGenerator : MonoBehaviour
{
    [Header("MapInfo")]
    public int width;
    public int lenght;

    MeshFilter meshFilter;

    public Vector3[] vertices;
    public Vector2[] uvs;
    public int[] triangles;
    int triangleIndex;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        CreateShape();
        int vertexIndex = 0;
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (lenght - 1) / 2f;


        for (int y = 0; y < lenght; y++)
        {
            for (int x = 0; x < width; x++)
            {
                vertices[vertexIndex] = new Vector3(topLeftX + x, 0.0f, topLeftZ - y);
                uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)lenght);

                if (x < width -1 && y < lenght - 1)
                {
                    AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        Mesh mesh = CreateMesh();
        meshFilter.sharedMesh = mesh;
    }

    void CreateShape()
    {
        vertices = new Vector3[width * lenght];
        uvs = new Vector2[width * lenght];
        triangles = new int[(width - 1) * (lenght - 1) * 6];
    }

    void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    Mesh CreateMesh()
    {
        Mesh mesh = new();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        return mesh;
    }
}
