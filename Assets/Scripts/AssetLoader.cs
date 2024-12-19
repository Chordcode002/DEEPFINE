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

    // 파일 창을 열고, OBJ를 선택해서 로드
    public void LoadOneOBJ()
    {
        string selectedAssetName = EditorUtility.OpenFilePanel("Select obj model", "", "obj");
        Load(selectedAssetName);
    }

    // 지정된 경로의 모든 OBJ 로드
    public void LoadMultipleOBJ()
    {
        List<string> selectedAssetNames = GetObjFiles("/Resources/Models");
        Load(selectedAssetNames);
    }

    // 생성되어 있는 모든 OBJ를 제거
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
        // 폴더 경로
        string resourcesPath = Application.dataPath+directory;

        // 폴더 내의 모든 obj 파일 가져오기
        string[] filePaths = Directory.GetFiles(resourcesPath, "*.obj", SearchOption.AllDirectories);

        // 크기 별 오름차순 정렬
        filePaths = filePaths.OrderBy(filePath => new FileInfo(filePath).Length).ToArray();

        return filePaths.ToList();
    }

    // 단일 OBJ를 로드
    public void Load(string assetName)
    {
        LoaderModule.OnLoadCompleted += OnLoadCompleted;
        LoaderModule.LoadAsset(assetName);
    }

    private void OnLoadCompleted(GameObject loadedAsset)
    {
        loadedAsset.transform.SetParent(transform);
    }

    // 2번 문제
    /*public async void Load(string assetName)
    {
        GameObject loadedAsset = await LoaderModule.LoadAssetAsync(assetName);
        if (loadedAsset != null)
        {
            loadedAsset.transform.SetParent(transform);
        }
    }*/

    // 폴더 내의 모든 OBJ를 비동기적으로 로드
    public async void Load(List<string> assetNames)
    {
        // 로드 작업을 위한 Task 목록 생성
        List<Task<GameObject>> loadTasks = new List<Task<GameObject>>();

        int count = 1;
        float totalAssets = assetNames.Count; 
        float currentProgress = 0f;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // 각 에셋에 대해 로드 작업 생성
        foreach (string assetName in assetNames)
        {
            var loadTask = LoaderModule.LoadAssetAsync(assetName, count, 4, 5, 300f, (progress)=>
            {
                currentProgress += 1f / totalAssets;
                loadBarCustom.imageComp.fillAmount = currentProgress;
            });
            count++;

            // 각 작업이 완료될 때까지 기다림
            GameObject loadedAsset = await loadTask;

            if (loadedAsset != null)
            {
                loadedAsset.transform.SetParent(transform);
            }
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log($"로드 시간: {stopwatch.Elapsed.TotalMilliseconds} ms");
    }
}