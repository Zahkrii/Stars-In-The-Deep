using Framework.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class AssetPool : PoolBase
    {
        public override Object Spwan(string name)
        {
            return base.Spwan(name);
        }

        public override void Recycle(string name, Object obj)
        {
            base.Recycle(name, obj);
        }

        public override void Release()
        {
            base.Release();
            foreach (InPoolObject po in pool)
            {
                if (System.DateTime.Now.Ticks - po.LastUsedTime.Ticks >= releaseTime * 10000000)
                {
                    Debug.Log($"AssetPool: Bundle:{po.Name} Release Time: {System.DateTime.Now}");
                    Manager.ResourceManager.UnloadBundle(po.Object);
                    pool.Remove(po);
                    //递归调用，pool 中对象被移除后 foreach 循环出错，因此递归调用解决
                    Release();
                    return;
                }
            }
        }
    }
}