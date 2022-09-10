using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // �洢��������UI����Ϊ����
    private Dictionary<string, GameObject> UICache = new Dictionary<string, GameObject>();

    //UI����
    private Dictionary<string, Transform> UIGroups = new Dictionary<string, Transform>();

    // UI��Manager�ĸ��ڵ�
    private Transform root;

    private void Awake()
    {
        //��ֵ
        root = this.transform.parent.Find("UI");
    }

    /// <summary>
    /// ����UI�㼶
    /// </summary>
    /// <param name="group"></param>
    public void SetUIGroup(List<string> group)
    {
        foreach (string item in group)
        {
            GameObject go = new GameObject("Group - " + item);
            go.transform.SetParent(root, false);
            UIGroups.Add(item, go.transform);
        }
    }

    public Transform GetUIGroup(string groupName)
    {
        if (!UIGroups.ContainsKey(groupName))
        {
            Debug.LogError("�Ҳ��� " + groupName);
            return null;
        }
        return UIGroups[groupName];
    }

    /// <summary>
    /// ��UI
    /// </summary>
    /// <param name="uiName"></param>
    /// <param name="luaName"></param>
    public void OpenUI(string uiName, string groupName, string luaName)
    {
        GameObject ui = null;
        if (UICache.TryGetValue(uiName, out ui))
        {
            UIBehaviour uiBehaviour = ui.GetComponent<UIBehaviour>();
            //����������ж���ֱ��ִ��OnOpen��������Init�׶Σ��൱��Start
            uiBehaviour.OnOpen();
            return;
        }
        //��������ڣ�������Դ���󶨽ű�������Init���൱��Awake
        Manager.ResourceManager.LoadAsset(uiName, AssetType.UI, (UnityEngine.Object obj) =>
        {
            ui = Instantiate(obj) as GameObject;
            UICache.Add(uiName, ui);

            // ����UI�㼶
            Transform parent = GetUIGroup(groupName);
            ui.transform.SetParent(parent, false);

            UIBehaviour uiBehaviour = ui.AddComponent<UIBehaviour>();
            //����init
            uiBehaviour.Init(luaName);
            uiBehaviour.OnOpen();
        });
    }
}