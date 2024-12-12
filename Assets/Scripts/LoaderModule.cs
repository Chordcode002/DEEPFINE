using System.Threading.Tasks;
using UnityEngine;
using Dummiesman;
using System;
using static UnityEngine.Rendering.DebugUI.Table;
using System.Diagnostics;

public class LoaderModule : MonoBehaviour
{
    public Action<GameObject> OnLoadCompleted;

    public async Task<GameObject> LoadAssetAsync(string assetName, int count)
    {

        //예외처리
        if (string.IsNullOrEmpty(assetName))
        {
            UnityEngine.Debug.LogError("잘못된 파일 경로입니다.");
            return null;
        }

        //Debug.Log("에셋 명 : "+ assetName + " 고유번호 : " + count);

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
            //Debug.Log("에셋 명 : " + assetName + " 출력 완료, 시간 : " + now.Millisecond);
            UnityEngine.Debug.Log($"에셋 {assetName} 로드 시간: {stopwatch.ElapsedMilliseconds} ms");
        }

        return loadedModel;
    }
}