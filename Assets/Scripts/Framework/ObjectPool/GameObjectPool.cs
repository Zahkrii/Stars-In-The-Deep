using Framework.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class GameObjectPool : PoolBase
    {
        /// <summary>
        /// 生成 GameObject
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
                    //递归调用，pool 中对象被移除后 foreach 循环出错，因此递归调用解决
                    Release();
                    return;
                }
            }
        }
    }
}