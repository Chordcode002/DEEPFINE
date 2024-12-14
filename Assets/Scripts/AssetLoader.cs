using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AssetLoader : MonoBehaviour
{
    public loadingbar loadBarCustom;

    [field: SerializeField]
    public LoaderModule LoaderModule { get; set; }

    private void Start()
    {
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
        float totalAssets = assetNames.Count; 
        float currentProgress = 0f; 

        // 각 에셋에 대해 로드 작업 생성
        foreach (string assetName in assetNames)
        {
            loadTasks.Add(LoaderModule.LoadAssetAsync(assetName, count, 4, 5, 500f, (progress)=>
            {
                currentProgress += 1f / totalAssets;
                loadBarCustom.imageComp.fillAmount = currentProgress;
            }
            ));
            count++;
        }

        // 모든 작업이 완료되었는지 확인
        GameObject[] loadedAssets = await Task.WhenAll(loadTasks);

        //만들어진 모든 어셋을 자식오브젝트로 설정
        foreach (GameObject loadedAsset in loadedAssets)
        {
            if (loadedAsset != null)
            {
                loadedAsset.transform.SetParent(transform);
            }
        }
    }
}