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
        // Resources 폴더 내의 모든 파일 가져오기 (파일 크기 기준 오름차순 정렬)
        string[] filePaths = Directory.GetFiles(directory, "*.obj", SearchOption.AllDirectories)
            .OrderBy(filePath => new FileInfo(filePath).Length)
            .ToArray();

        // 파일 이름만 추출하여 리스트에 저장
        List<string> objFiles = filePaths.ToList();

        /*
        foreach (var filePath in objFiles)
        {
            string fullPath = Path.Combine(Application.dataPath, directory, filePath);
            Debug.Log($"파일 이름: {filePath}, 크기: {new FileInfo(fullPath).Length} bytes");
        }
        */

        return objFiles;
    }

    // 비동기 방식으로 obj 파일을 로드하는 메소드
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

        // 로드된 에셋들을 부모 객체에 추가
        foreach (GameObject loadedAsset in loadedAssets)
        {
            if (loadedAsset != null)
            {
                loadedAsset.transform.SetParent(transform);
            }
        }
    }
}