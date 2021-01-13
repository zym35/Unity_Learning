using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int LOD) 
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        //let the mesh centered at the origin
        float topleftX = (width - 1) / -2f;
        float topleftZ = (height - 1) / 2f;

        int simplificationIncrement = (LOD == 0) ? 1 : LOD * 2;
        int verticesPerLine = (width - 1) / simplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += simplificationIncrement) {
            for (int x = 0; x < width; x += simplificationIncrement) 
            {
                //add vertex
                meshData.vertices[vertexIndex] = new Vector3(x + topleftX, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topleftZ - y);
                meshData.uv[vertexIndex] = new Vector2(x / (float) width, y / (float) height);

                //add 2 triangles at each vertex in CW order (exclude right and bottom lines)
                if (x < width - 1 && y < height - 1) {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }
        
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uv;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshHeight * meshWidth];
        triangles = new int[(meshHeight - 1) * (meshWidth - 1) * 6];
        uv = new Vector2[meshWidth * meshHeight];
    }

    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex++] = a;
        triangles[triangleIndex++] = b;
        triangles[triangleIndex++] = c;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        return mesh;
    }
}