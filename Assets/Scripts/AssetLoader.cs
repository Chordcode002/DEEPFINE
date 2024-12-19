using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AssetLoader : MonoBehaviour
{
    public loadingbar loadBarCustom;
    public Button loadOneOBJButton;
    public Button loadMultipleOBJButton;
    public Button clearOBJButton;

    [field: SerializeField]
    public LoaderModule LoaderModule { get; set; }

    private void Start()
    {
        //List<string> selectedAssetNames = GetObjFiles("/Resources/Models");
        //Load(selectedAssetNames);
    }

    // ���� â�� ����, OBJ�� �����ؼ� �ε�
    public void LoadOneOBJ()
    {
        string selectedAssetName = EditorUtility.OpenFilePanel("Select obj model", "", "obj");
        Load(selectedAssetName);
    }

    // ������ ����� ��� OBJ �ε�
    public void LoadMultipleOBJ()
    {
        List<string> selectedAssetNames = GetObjFiles("/Resources/Models");
        Load(selectedAssetNames);
    }

    // �����Ǿ� �ִ� ��� OBJ�� ����
    public void ClearOBJ()
    {
        loadBarCustom.imageComp.fillAmount = 0f;

        Transform[] children = GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child != transform)
            {
                Destroy(child.gameObject);
            }
        }
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

    // ���� OBJ�� �ε�
    public void Load(string assetName)
    {
        LoaderModule.OnLoadCompleted += OnLoadCompleted;
        LoaderModule.LoadAsset(assetName);
    }

    private void OnLoadCompleted(GameObject loadedAsset)
    {
        loadedAsset.transform.SetParent(transform);
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

    // ���� ���� ��� OBJ�� �񵿱������� �ε�
    public async void Load(List<string> assetNames)
    {
        // �ε� �۾��� ���� Task ��� ����
        List<Task<GameObject>> loadTasks = new List<Task<GameObject>>();

        int count = 1;
        float totalAssets = assetNames.Count; 
        float currentProgress = 0f;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // �� ���¿� ���� �ε� �۾� ����
        foreach (string assetName in assetNames)
        {
            var loadTask = LoaderModule.LoadAssetAsync(assetName, count, 4, 5, 300f, (progress)=>
            {
                currentProgress += 1f / totalAssets;
                loadBarCustom.imageComp.fillAmount = currentProgress;
            });
            count++;

            // �� �۾��� �Ϸ�� ������ ��ٸ�
            GameObject loadedAsset = await loadTask;

            if (loadedAsset != null)
            {
                loadedAsset.transform.SetParent(transform);
            }
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log($"�ε� �ð�: {stopwatch.Elapsed.TotalMilliseconds} ms");
    }
}