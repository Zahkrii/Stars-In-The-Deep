using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace Framework.Managers
{
    public class EntityManager : MonoBehaviour
    {
        //缓存所有entity
        private Dictionary<string, GameObject> entities = new Dictionary<string, GameObject>();

        //组管理
        private Dictionary<string, Transform> groupsCache = new Dictionary<string, Transform>();

        //entity父节点
        private Transform entityParent;

        private void Awake()
        {
            entityParent = this.transform.parent.Find("Entity");
        }

        /// <summary>
        /// 设置entity分组
        /// </summary>
        /// <param name="groups"></param>
        public void SetEntityGroup(List<string> groups)
        {
            foreach (string group in groups)
            {
                GameObject go = new GameObject("Group - " + group);
                go.transform.SetParent(entityParent, false);
                groupsCache[group] = go.transform;
            }
        }

        private Transform GetEntityGroup(string group)
        {
            if (!groupsCache.ContainsKey(group))
            {
                Debug.LogError($"Group：{group} 不存在");
                return entityParent;
            }
            return groupsCache[group];
        }

        public void ShowEntity(string name, string group, string lua)
        {
            GameObject entity = null;
            if (entities.TryGetValue(name, out entity))
            {
                EntityBehaviour entityBehaviour = entity.GetComponent<EntityBehaviour>();
                //如果存在现有对象，直接执行OnStart，跳过了Init阶段
                entityBehaviour.OnStart();
                return;
            }
            //如果不存在，加载资源并绑定脚本，调用Init，相当于Awake
            Manager.ResourceManager.LoadAsset(name, AssetType.Prefab, (UnityEngine.Object obj) =>
            {
                entity = Instantiate(obj) as GameObject;

                // 设置组别
                Transform parent = GetEntityGroup(group);
                entity.transform.SetParent(parent, false);

                entities.Add(name, entity);

                EntityBehaviour entityBehaviour = entity.AddComponent<EntityBehaviour>();
                //调用init
                entityBehaviour.Init(lua);
                entityBehaviour.OnStart();
            });
        }
    }
}