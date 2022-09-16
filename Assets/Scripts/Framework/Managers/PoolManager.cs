using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Managers
{
    public class PoolManager : MonoBehaviour
    {
        // 对象池父节点
        private Transform poolParent;

        // 所有对象池的字典
        private Dictionary<string, PoolBase> pools = new Dictionary<string, PoolBase>();

        private void Awake()
        {
            //查找同级节点
            poolParent = this.transform.parent.Find("Pool");
        }

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolName"></param>
        /// <param name="releaseTime">自动释放时间，单位 second</param>
        public void Create<T>(string poolName, float releaseTime) where T : PoolBase // T 必须继承自 PoolBase
        {
            if (!pools.TryGetValue(poolName, out PoolBase pool))
            {
                GameObject go = new GameObject(poolName);
                go.transform.SetParent(poolParent);
                pool = go.AddComponent<T>();
                pool.Init(releaseTime);
                pools.Add(poolName, pool);
            }
        }

        /// <summary>
        /// 取出对象
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="objName"></param>
        /// <returns></returns>
        public Object Spwan(string poolName, string objName)
        {
            if (pools.TryGetValue(poolName, out PoolBase pool))
            {
                return pool.Spwan(objName);
            }
            return null;
        }

        /// <summary>
        /// 存入对象
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="objName"></param>
        /// <param name="obj"></param>
        public void Recycle(string poolName, string objName, Object obj)
        {
            if (pools.TryGetValue(poolName, out PoolBase pool))
            {
                pool.Recycle(objName, obj);
            }
        }
    }
}