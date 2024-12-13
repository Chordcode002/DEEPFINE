using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class AssetLoader : MonoBehaviour
{
    public Slider LoadingBar;

    [field: SerializeField]
    public LoaderModule LoaderModule { get; set; }

    private void Start()
    {
        List<string> selectedAssetNames = GetObjFiles("/Resources/Models");
        Load(selectedAssetNames);
    }

    private List<string> GetObjFiles(string directory)
    {
        // ���� ���
        string resourcesPath = Application.dataPath+directory;

        // ���� ���� ��� obj ���� ��������
        string[] filePaths = Directory.GetFiles(resourcesPath, "*.obj", SearchOption.AllDirectories);

        // ũ�� �� �������� ����
        filePaths = filePaths.OrderBy(filePath => new FileInfo(filePath).Length).ToArray();

        return filePaths.ToList();
    }

    // 2�� ����
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
        // �ε� �۾��� ���� Task ��� ����
        List<Task<GameObject>> loadTasks = new List<Task<GameObject>>();

        int count = 1;
        float totalAssets = assetNames.Count; // ��ü ���� ����
        float currentProgress = 0f; // ��������� �� ���� ��Ȳ

        // �� ���¿� ���� �ε� �۾� ����
        foreach (string assetName in assetNames)
        {
            loadTasks.Add(LoaderModule.LoadAssetAsync(assetName, count, 4, 5, 500f, (progress)=>
            {
                // LoadAssetAsync �Լ����� ���޵� progress ���� ������� ����
                currentProgress += 1f / totalAssets; // ��ü �½�ũ ������ ������ŭ ����
                LoadingBar.value = currentProgress;
            }
            ));
            count++;
        }

        // ��� �۾��� �Ϸ�Ǿ����� Ȯ��
        GameObject[] loadedAssets = await Task.WhenAll(loadTasks);

        //������� ��� ����� �ڽĿ�����Ʈ�� ����
        foreach (GameObject loadedAsset in loadedAssets)
        {
            if (loadedAsset != null)
            {
                loadedAsset.transform.SetParent(transform);
            }
        }
    }
}