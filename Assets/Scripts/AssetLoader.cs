using UnityEditor;
using UnityEngine;

public class AssetLoader : MonoBehaviour
{
    [field: SerializeField]
    public LoaderModule LoaderModule { get; set; }

    private void Start()
    {
        string selectedAssetName = EditorUtility.OpenFilePanel("Select obj model", "", "obj");
        Load(selectedAssetName);
    }

    // 비동기 방식으로 obj 파일을 로드하는 메소드
    public async void Load(string assetName)
    {
        GameObject loadedAsset = await LoaderModule.LoadAssetAsync(assetName);
        if (loadedAsset != null)
        {
            loadedAsset.transform.SetParent(transform);
        }
    }
}