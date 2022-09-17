using Framework.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class GameObjectPool : PoolBase
    {
        /// <summary>
        /// ���� GameObject
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Object Spwan(string name)
        {
            Object obj = base.Spwan(name);
            if (obj == null)
                return null;
            GameObject go = obj as GameObject;
            go.SetActive(true);
            return obj;
        }

        public override void Recycle(string name, Object obj)
        {
            GameObject go = obj as GameObject;
            go.SetActive(false);
            go.transform.SetParent(this.transform, false);
            base.Recycle(name, obj);
        }

        public override void Release()
        {
            base.Release();
            foreach (InPoolObject po in pool)
            {
                if (System.DateTime.Now.Ticks - po.LastUsedTime.Ticks >= releaseTime * 10000000)
                {
                    Debug.Log("GameObjectPool Release Time: " + System.DateTime.Now);
                    Destroy(po.Object);
                    Manager.ResourceManager.MinusBundleRefCount(po.Name);
                    pool.Remove(po);
                    //�ݹ���ã�pool �ж����Ƴ��� foreach ѭ����������˵ݹ���ý��
                    Release();
                    return;
                }
            }
        }
    }
}