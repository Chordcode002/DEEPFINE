using System.Threading.Tasks;
using UnityEngine;
using Dummiesman;

public class LoaderModule : MonoBehaviour
{
    public async Task<GameObject> LoadAssetAsync(string assetName)
    {
        //����ó��
        if (string.IsNullOrEmpty(assetName))
        {
            Debug.LogError("�߸��� ���� ����Դϴ�.");
            return null;
        }

        //Debug.Log("�񵿱� �۾� ����");
        async Task<GameObject> LoadAsync()
        {
            return new OBJLoader().Load(assetName);
        }

        GameObject loadedModel = await LoadAsync();
        //Debug.Log("��� ��");

        if (loadedModel != null)
        {
            loadedModel.transform.position = Vector3.zero;
        }

        //Debug.Log("�Ϸ�");
        return loadedModel;
    }
}