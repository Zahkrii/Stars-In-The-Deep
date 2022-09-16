using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class PoolBase : MonoBehaviour
    {
        //自动释放时间/秒
        protected float releaseTime;

        //上次释放时间/毫微秒 1秒 = 10^7 = 10000000 毫微秒
        protected long lastReleaseTime = 0;

        protected List<InPoolObject> pool;

        private void Start()
        {
            lastReleaseTime = System.DateTime.Now.Ticks;
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="time"></param>
        public void Init(float time)
        {
            releaseTime = time;
            pool = new List<InPoolObject>();
        }

        /// <summary>
        /// 取出对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Object Spwan(string name)
        {
            foreach (InPoolObject po in pool)
            {
                if (po.Name == name)
                {
                    pool.Remove(po);
                    return po.Object;
                }
            }
            return null;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public virtual void Recycle(string name, Object obj)
        {
            InPoolObject po = new InPoolObject(name, obj);
            pool.Add(po);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Release()
        {
        }

        private void Update()
        {
            //现在距离最后使用时间超过了定义的释放时间，则释放资源
            if (System.DateTime.Now.Ticks - lastReleaseTime >= releaseTime * 10000000)
            {
                lastReleaseTime = System.DateTime.Now.Ticks;
                Release();
            }
        }
    }
}