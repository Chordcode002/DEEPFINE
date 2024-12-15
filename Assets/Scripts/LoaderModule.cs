using System.Threading.Tasks;
using UnityEngine;
using Dummiesman;
using System;

public class LoaderModule : MonoBehaviour
{
    public Action<GameObject> OnLoadCompleted;

    // ���� �ε�
    public void LoadAsset(string assetName)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            Debug.LogError("�߸��� ���� ����Դϴ�.");
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
                Debug.LogError("OBJ �ε��� �����߽��ϴ�.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"OBJ �ε� �� ���� �߻�: {e.Message}");
        }
    }

    // �񵿱� �ε� (���� �ּ�, ��ġ�� ���� ��, ��ġ ��, ��ġ ��, ��ġ ����, �Ϸ� �� �ε� �������� ���� �ݹ�)
    public async Task<GameObject> LoadAssetAsync(string assetName, int count, int rows, int cols, float spacing, Action<float> onProgress)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            Debug.LogError("�߸��� ���� ����Դϴ�.");
            return null;
        }

        string mtlPath = assetName.Replace(".obj", ".mtl");

        GameObject loadedModel = null;

        await Task.Run(async () =>
        {
            // ���� ��Ȳ�� ��Ȯ�ϰ� ���� ���� ������
            // await Task.Delay(count*100);

            // ����Ƽ API�� �����ؾ��ϴ� �κ��� ���� �����忡 ���
            await UnityMainThread.ExecuteInUpdate(() =>
            {

                try
                {
                    loadedModel = new OBJLoader().Load(assetName, mtlPath);

                    if (loadedModel != null)
                    {
                        // ���� ������� ���� ���� ��ġ
                        int row = (count - 1) / cols;
                        int col = (count - 1) % cols;
                        loadedModel.transform.position = new Vector3(col * spacing, 0, row * spacing);
                    }
                    else
                    {
                        Debug.LogError("OBJ �ε��� �����߽��ϴ�.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"OBJ �ε� �� ���� �߻�: {e.Message}");
                }

            });
        });

        onProgress?.Invoke(1f);

        return loadedModel;
    }
}