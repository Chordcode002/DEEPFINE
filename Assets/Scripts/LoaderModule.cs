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
            Debug.LogError("�߸��� ���� ����Դϴ�.");
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
                Debug.LogError("OBJ �ε��� �����߽��ϴ�.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"OBJ �ε� �� ���� �߻�: {e.Message}");
        }
    }
}
