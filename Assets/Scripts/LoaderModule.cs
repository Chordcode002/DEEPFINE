using System.Threading.Tasks;
using UnityEngine;
using Dummiesman;
using System;

public class LoaderModule : MonoBehaviour
{
    public Action<GameObject> OnLoadCompleted;

    // 동기 로드
    public void LoadAsset(string assetName)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            Debug.LogError("잘못된 파일 경로입니다.");
            return;
        }

        try
        {
            GameObject loadedModel = new OBJLoader().Load(assetName);

            if (loadedModel != null)
            {
                loadedModel.transform.position = Vector3.zero;
                OnLoadCompleted?.Invoke(loadedModel);
            }
            else
            {
                Debug.LogError("OBJ 로딩에 실패했습니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"OBJ 로드 중 오류 발생: {e.Message}");
        }
    }

    // 비동기 로드 (파일 주소, 배치를 위한 값, 배치 행, 배치 열, 배치 간격, 완료 시 로딩 게이지를 위한 콜백)
    public async Task<GameObject> LoadAssetAsync(string assetName, int count, int rows, int cols, float spacing, Action<float> onProgress)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            Debug.LogError("잘못된 파일 경로입니다.");
            return null;
        }

        string mtlPath = assetName.Replace(".obj", ".mtl");

        GameObject loadedModel = null;

        await Task.Run(async () =>
        {
            // 진행 상황을 명확하게 보기 위한 딜레이
            // await Task.Delay(count*100);

            // 유니티 API에 접근해야하는 부분을 메인 쓰레드에 등록
            await UnityMainThread.ExecuteInUpdate(() =>
            {

                try
                {
                    loadedModel = new OBJLoader().Load(assetName, mtlPath);

                    if (loadedModel != null)
                    {
                        // 격자 모양으로 보기 좋게 배치
                        int row = (count - 1) / cols;
                        int col = (count - 1) % cols;
                        loadedModel.transform.position = new Vector3(col * spacing, 0, row * spacing);
                    }
                    else
                    {
                        Debug.LogError("OBJ 로딩에 실패했습니다.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"OBJ 로드 중 오류 발생: {e.Message}");
                }

            });
        });

        onProgress?.Invoke(1f);

        return loadedModel;
    }
}