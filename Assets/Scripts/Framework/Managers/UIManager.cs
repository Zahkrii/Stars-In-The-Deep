using Framework.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Managers
{
    public class UIManager : MonoBehaviour
    {
        // 存储管理所有UI，键为名称
        //private Dictionary<string, GameObject> UICache = new Dictionary<string, GameObject>();

        //UI分组
        private Dictionary<string, Transform> UIGroups = new Dictionary<string, Transform>();

        // UI与Manager的根节点
        private Transform root;

        private void Awake()
        {
            //赋值
            root = this.transform.parent.Find("UI");
        }

        /// <summary>
        /// 设置UI层级
        /// </summary>
        /// <param name="group"></param>
        public void SetUIGroup(List<string> group)
        {
            foreach (string item in group)
            {
                GameObject go = new GameObject("Group - " + item);
                go.transform.SetParent(root, true);
                UIGroups.Add(item, go.transform);
            }
        }

        private Transform GetUIGroup(string groupName)
        {
            if (!UIGroups.ContainsKey(groupName))
            {
                Debug.LogError("找不到 " + groupName);
                return root;
            }
            return UIGroups[groupName];
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="luaName"></param>
        public void OpenUI(string uiName, string groupName, string luaName)
        {
            GameObject ui = null;

            // 设置UI层级
            Transform parent = GetUIGroup(groupName);

            string uiPath = PathUtil.GetUIPath(uiName);

            Object obj = Manager.PoolManager.Spwan("UI", uiPath);
            if (obj != null)
            {
                ui = obj as GameObject;
                UIBehaviour uiBehaviour = ui.GetComponent<UIBehaviour>();
                ui.transform.SetParent(parent, false);
                //如果存在现有对象，直接执行OnOpen，跳过了Init阶段，相当于Start
                uiBehaviour.OnOpen();
                return;
            }

            //if (UICache.TryGetValue(uiName, out ui))
            //{
            //    UIBehaviour uiBehaviour = ui.GetComponent<UIBehaviour>();
            //    //如果存在现有对象，直接执行OnOpen，跳过了Init阶段，相当于Start
            //    uiBehaviour.OnOpen();
            //    return;
            //}

            //如果不存在，加载资源并绑定脚本，调用Init，相当于Awake
            Manager.ResourceManager.LoadAsset(uiName, AssetType.UI, (UnityEngine.Object obj) =>
            {
                ui = Instantiate(obj) as GameObject;

                ui.transform.SetParent(parent, false);

                //UICache.Add(uiName, ui);

                UIBehaviour uiBehaviour = ui.AddComponent<UIBehaviour>();
                // 存储全路径，进入池子后方便查找
                uiBehaviour.AssetName = PathUtil.GetUIPath(uiName);
                //调用init
                uiBehaviour.Init(luaName);
                uiBehaviour.OnOpen();
            });
        }
    }
}