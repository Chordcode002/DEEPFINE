using System.Threading.Tasks;
using UnityEngine;
using Dummiesman;
using System;
using static UnityEngine.Rendering.DebugUI.Table;

public class LoaderModule : MonoBehaviour
{
    public Action<GameObject> OnLoadCompleted;

    public async Task<GameObject> LoadAssetAsync(string assetName, int count)
    {

        //����ó��
        if (string.IsNullOrEmpty(assetName))
        {
            Debug.LogError("�߸��� ���� ����Դϴ�.");
            return null;
        }

        //Debug.Log("���� �� : "+ assetName + " ������ȣ : " + count);

        //���׸��� ���
        string mtlPath = assetName.Replace(".obj", ".mtl"); 
        //Debug.Log(mtlPath);

        //Debug.Log("�񵿱� �۾� ����");
        async Task<GameObject> LoadAsync()
        {
            return new OBJLoader().Load(assetName, mtlPath);
            //return await Task.Run(() => new OBJLoader().Load(assetName, mtlPath));
            //return new OBJParser().Parse(assetName);
        }

        GameObject loadedModel = await LoadAsync();

        if (loadedModel != null)
        {
            DateTime now = DateTime.Now;
            loadedModel.transform.position = new Vector3(0, 0, count * 200);
            //Debug.Log("���� �� : " + assetName + " ��� �Ϸ�, �ð� : " + now.Millisecond);
        }

        return loadedModel;
    }
}