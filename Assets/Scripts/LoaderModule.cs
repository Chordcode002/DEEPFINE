using System.Threading.Tasks;
using UnityEngine;
using Dummiesman;
using System;
using static UnityEngine.Rendering.DebugUI.Table;
using System.Diagnostics;

public class LoaderModule : MonoBehaviour
{
    public Action<GameObject> OnLoadCompleted;

    public void LoadAsset(string assetName)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            UnityEngine.Debug.LogError("잘못된 파일 경로입니다.");
            return;
        }

        try
        {
            // OBJ 파일 로드
            GameObject loadedModel = new OBJLoader().Load(assetName);

            if (loadedModel != null)
            {
                loadedModel.transform.position = Vector3.zero;
                OnLoadCompleted?.Invoke(loadedModel);
            }
            else
            {
                UnityEngine.Debug.LogError("OBJ 로딩에 실패했습니다.");
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"OBJ 로드 중 오류 발생: {e.Message}");
        }
    }

    /*
    public async Task<GameObject> LoadAssetAsync(string assetName, int count)
    {

        //예외처리
        if (string.IsNullOrEmpty(assetName))
        {
            UnityEngine.Debug.LogError("잘못된 파일 경로입니다.");
            return null;
        }

        //머테리얼 경로
        string mtlPath = assetName.Replace(".obj", ".mtl");

        Stopwatch stopwatch = Stopwatch.StartNew();

        async Task<GameObject> LoadAsync()
        {
            //await Task.Delay(count * 500);

            return new OBJLoader().Load(assetName, mtlPath);
            //return await Task.Run(() => new OBJLoader().Load(assetName, mtlPath));
            //return new OBJParser().Parse(assetName);
        }

        GameObject loadedModel = await LoadAsync();

        stopwatch.Stop();

        if (loadedModel != null)
        {
            DateTime now = DateTime.Now;
            loadedModel.transform.position = new Vector3(0, 0, count * 200);
            UnityEngine.Debug.Log($"에셋 {assetName} 로드 시간: {stopwatch.ElapsedMilliseconds} ms");
        }

        return loadedModel;
    }*/

    public async Task<GameObject> LoadAssetAsync(string assetName, int count)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            UnityEngine.Debug.LogError("잘못된 파일 경로입니다.");
            return null;
        }

        GameObject loadedModel = null;

        await Task.Run(async () =>
        {
            //로드하는 부분을 
            await UnityMainThread.ExecuteInUpdate(() =>
            {
                loadedModel = new OBJLoader().Load(assetName);

                if (loadedModel != null)
                {
                    loadedModel.transform.position = new Vector3(0, 0, count * 200);
                }
            });

            
        });

        return loadedModel;
    }
}