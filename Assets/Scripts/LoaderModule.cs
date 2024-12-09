using System;
using System.IO;
using UnityEngine;

public class LoaderModule : MonoBehaviour
{
    public Action<GameObject> OnLoadCompleted;

    public void LoadAsset(string assetName)
    {
        try
        {
            // .obj 파일을 문자열로 읽어오기
            string objData = File.ReadAllText(assetName);

            // Mesh 생성
            GameObject loadedObject = new GameObject("LoadedOBJ");
            MeshFilter meshFilter = loadedObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = loadedObject.AddComponent<MeshRenderer>();

            // 간단한 재질 추가 (머티리얼 추가 필요 없음)
            meshRenderer.material = new Material(Shader.Find("Standard"));

            // OBJ 파싱 및 Mesh 설정 (간단 구현)
            Mesh mesh = ObjImporter.ImportMesh(objData);
            meshFilter.mesh = mesh;

            // 로드 완료 알림
            OnLoadCompleted?.Invoke(loadedObject);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading asset: {e.Message}");
        }
    }
}

public static class ObjImporter
{
    public static Mesh ImportMesh(string objData)
    {
        Mesh mesh = new Mesh();

        // 간단한 OBJ 파서 구현 (Vertex 및 Face 데이터만)
        var vertices = new System.Collections.Generic.List<Vector3>();
        var triangles = new System.Collections.Generic.List<int>();

        using (StringReader reader = new StringReader(objData))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("v ")) // Vertex
                {
                    string[] parts = line.Split(' ');
                    vertices.Add(new Vector3(
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])
                    ));
                }
                else if (line.StartsWith("f ")) // Face
                {
                    string[] parts = line.Split(' ');
                    triangles.Add(int.Parse(parts[1]) - 1);
                    triangles.Add(int.Parse(parts[2]) - 1);
                    triangles.Add(int.Parse(parts[3]) - 1);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }
}
