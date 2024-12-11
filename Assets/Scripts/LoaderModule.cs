using System.Threading.Tasks;
using UnityEngine;
using Dummiesman;

public class LoaderModule : MonoBehaviour
{
    public async Task<GameObject> LoadAssetAsync(string assetName)
    {
        //예외처리
        if (string.IsNullOrEmpty(assetName))
        {
            Debug.LogError("잘못된 파일 경로입니다.");
            return null;
        }

        //Debug.Log("비동기 작업 시작");
        async Task<GameObject> LoadAsync()
        {
            return new OBJLoader().Load(assetName);
        }

        GameObject loadedModel = await LoadAsync();
        //Debug.Log("대기 중");

        if (loadedModel != null)
        {
            loadedModel.transform.position = Vector3.zero;
        }

        //Debug.Log("완료");
        return loadedModel;
    }
}