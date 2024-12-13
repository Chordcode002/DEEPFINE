using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
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
        Load(selectedAssetNames);
    }

    private List<string> GetObjFiles(string directory)
    {
        // ���� ���
        string resourcesPath = Application.dataPath+directory;

        // ���� ���� ��� obj ���� ��������
        string[] filePaths = Directory.GetFiles(resourcesPath, "*.obj", SearchOption.AllDirectories);

        // ũ�� �� �������� ����
        filePaths = filePaths.OrderBy(filePath => new FileInfo(filePath).Length).ToArray();

        return filePaths.ToList();
    }

    // 2�� ����
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
        //Task<GameObject> loadedAssets = await Task.WhenAny(loadTasks);

        // �ε�� ���µ��� �θ� ��ü�� �߰�
        foreach (GameObject loadedAsset in loadedAssets)
        {
            if (loadedAsset != null)
            {
                loadedAsset.transform.SetParent(transform);
            }
        }
    }

    /*
    public void Load(List<string> assetNames)
    {
        int count = 1;

        // �� ���¿� ���� �ε� �۾� ���� �� ����
        foreach (string assetName in assetNames)
        {
            // LoadAssetAsync�� ����� ó���ϴ� �͸� �Լ� ����
            Action<GameObject> handleLoadedObject = (loadedAsset) =>
            {
                if (loadedAsset != null)
                {
                    loadedAsset.transform.SetParent(transform);
                }
            };

            // LoadAssetAsync ���� �� ��� ó��
            _ = LoaderModule.LoadAssetAsync(assetName, count)
                .ContinueWith(task => handleLoadedObject(task.Result));

            count++;
        }
    }*/
}