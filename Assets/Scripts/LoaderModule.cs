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
            UnityEngine.Debug.LogError("�߸��� ���� ����Դϴ�.");
            return;
        }

        try
        {
            // OBJ ���� �ε�
            GameObject loadedModel = new OBJLoader().Load(assetName);

            if (loadedModel != null)
            {
                loadedModel.transform.position = Vector3.zero;
                OnLoadCompleted?.Invoke(loadedModel);
            }
            else
            {
                UnityEngine.Debug.LogError("OBJ �ε��� �����߽��ϴ�.");
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"OBJ �ε� �� ���� �߻�: {e.Message}");
        }
    }

    /*
    public async Task<GameObject> LoadAssetAsync(string assetName, int count)
    {

        //����ó��
        if (string.IsNullOrEmpty(assetName))
        {
            UnityEngine.Debug.LogError("�߸��� ���� ����Դϴ�.");
            return null;
        }

        //���׸��� ���
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
            UnityEngine.Debug.Log($"���� {assetName} �ε� �ð�: {stopwatch.ElapsedMilliseconds} ms");
        }

        return loadedModel;
    }*/

    public async Task<GameObject> LoadAssetAsync(string assetName, int count)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            UnityEngine.Debug.LogError("�߸��� ���� ����Դϴ�.");
            return null;
        }

        GameObject loadedModel = null;

        await Task.Run(async () =>
        {
            //�ε��ϴ� �κ��� 
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