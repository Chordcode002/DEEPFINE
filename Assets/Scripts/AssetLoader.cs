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
        // 폴더 경로
        string resourcesPath = Application.dataPath+directory;

        // 폴더 내의 모든 obj 파일 가져오기
        string[] filePaths = Directory.GetFiles(resourcesPath, "*.obj", SearchOption.AllDirectories);

        // 크기 별 오름차순 정렬
        filePaths = filePaths.OrderBy(filePath => new FileInfo(filePath).Length).ToArray();

        return filePaths.ToList();
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

    public async void Load(List<string> assetNames)
    {
        // 로드 작업을 위한 Task 목록 생성
        List<Task<GameObject>> loadTasks = new List<Task<GameObject>>();

        int count = 1;

        // 각 에셋에 대해 로드 작업 생성
        foreach (string assetName in assetNames)
        {
            loadTasks.Add(LoaderModule.LoadAssetAsync(assetName, count));
            count++;
        }

        // 모든 로드 작업이 완료될 때까지 기다림
        GameObject[] loadedAssets = await Task.WhenAll(loadTasks);
        //Task<GameObject> loadedAssets = await Task.WhenAny(loadTasks);

        // 로드된 에셋들을 부모 객체에 추가
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

        // 각 에셋에 대해 로드 작업 생성 및 실행
        foreach (string assetName in assetNames)
        {
            // LoadAssetAsync의 결과를 처리하는 익명 함수 정의
            Action<GameObject> handleLoadedObject = (loadedAsset) =>
            {
                if (loadedAsset != null)
                {
                    loadedAsset.transform.SetParent(transform);
                }
            };

            // LoadAssetAsync 실행 및 결과 처리
            _ = LoaderModule.LoadAssetAsync(assetName, count)
                .ContinueWith(task => handleLoadedObject(task.Result));

            count++;
        }
    }*/
}