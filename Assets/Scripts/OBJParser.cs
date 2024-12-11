using System.Collections.Generic;
using UnityEngine;

public class OBJParser
{
    public GameObject Parse(string objData)
    {
        GameObject newObj = new GameObject("LoadedOBJ");

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        string[] lines = objData.Split('\n');
        foreach (string line in lines)
        {
            string[] parts = line.Trim().Split(' ');
            if (parts.Length < 1) continue;

            switch (parts[0])
            {
                case "v": // Vertex
                    vertices.Add(new Vector3(
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])));
                    break;

                case "f": // Face
                    triangles.Add(int.Parse(parts[1]) - 1);
                    triangles.Add(int.Parse(parts[2]) - 1);
                    triangles.Add(int.Parse(parts[3]) - 1);
                    break;
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        MeshFilter filter = newObj.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        newObj.AddComponent<MeshRenderer>();

        return newObj;
    }
}
