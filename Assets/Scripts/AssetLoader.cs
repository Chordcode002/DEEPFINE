using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class AssetLoader : MonoBehaviour
{
    [field: SerializeField]
    public LoaderModule LoaderModule { get; set; }

    private void Start()
    {
        //string selectedAssetName = EditorUtility.OpenFilePanel("Select obj model", "", "obj");
        List<string> selectedAssetNames = GetObjFiles("/Resources/Models");
        //selectedAssetNames.ForEach(item => Debug.Log(item));
        Load(selectedAssetNames);
    }

    private List<string> GetObjFiles(string directory)
    {
        // Resources ���� ���� ��� ���� �������� (���� ũ�� ���� �������� ����)
        string[] filePaths = Directory.GetFiles(directory, "*.obj", SearchOption.AllDirectories)
            .OrderBy(filePath => new FileInfo(filePath).Length)
            .ToArray();

        // ���� �̸��� �����Ͽ� ����Ʈ�� ����
        List<string> objFiles = filePaths.ToList();

        /*
        foreach (var filePath in objFiles)
        {
            string fullPath = Path.Combine(Application.dataPath, directory, filePath);
            Debug.Log($"���� �̸�: {filePath}, ũ��: {new FileInfo(fullPath).Length} bytes");
        }
        */

        return objFiles;
    }

    // �񵿱� ������� obj ������ �ε��ϴ� �޼ҵ�
    /*public async void Load(string assetName)
    {
        GameObject loadedAsset = await LoaderModule.LoadAssetAsync(assetName);
        if (loadedAsset != null)
        {
            loadedAsset.transform.SetParent(transform);
        }
    }*/

    public async void Load(List<string> assetNames)
    {
        // �ε� �۾��� ���� Task ��� ����
        List<Task<GameObject>> loadTasks = new List<Task<GameObject>>();

        int count = 1;

        // �� ���¿� ���� �ε� �۾� ����
        foreach (string assetName in assetNames)
        {
            loadTasks.Add(LoaderModule.LoadAssetAsync(assetName, count));
            count++;
        }

        // ��� �ε� �۾��� �Ϸ�� ������ ��ٸ�
        GameObject[] loadedAssets = await Task.WhenAll(loadTasks);

        // �ε�� ���µ��� �θ� ��ü�� �߰�
        foreach (GameObject loadedAsset in loadedAssets)
        {
            if (loadedAsset != null)
            {
                loadedAsset.transform.SetParent(transform);
            }
        }
    }
}