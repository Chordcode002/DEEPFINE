using System;
using UnityEngine;
using Dummiesman; 

public class LoaderModule : MonoBehaviour
{
    public Action<GameObject> OnLoadCompleted;

    public void LoadAsset(string assetName)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            Debug.LogError("잘못된 파일 경로입니다.");
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
                Debug.LogError("OBJ 로딩에 실패했습니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"OBJ 로드 중 오류 발생: {e.Message}");
        }
    }
}
