using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Managers
{
    public class PoolManager : MonoBehaviour
    {
        // ����ظ��ڵ�
        private Transform poolParent;

        // ���ж���ص��ֵ�
        private Dictionary<string, PoolBase> pools = new Dictionary<string, PoolBase>();

        private void Awake()
        {
            //����ͬ���ڵ�
            poolParent = this.transform.parent.Find("Pool");
        }

        /// <summary>
        /// ���������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolName"></param>
        /// <param name="releaseTime">�Զ��ͷ�ʱ�䣬��λ second</param>
        public void Create<T>(string poolName, float releaseTime) where T : PoolBase // T ����̳��� PoolBase
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
        /// ȡ������
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
        /// �������
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