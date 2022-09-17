using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class PoolBase : MonoBehaviour
    {
        //�Զ��ͷ�ʱ��/��
        protected float releaseTime;

        //�ϴ��ͷ�ʱ��/��΢�� 1�� = 10^7 = 10000000 ��΢��
        protected long lastReleaseTime = 0;

        protected List<InPoolObject> pool;

        private void Start()
        {
            lastReleaseTime = System.DateTime.Now.Ticks;
        }

        /// <summary>
        /// ��ʼ�������
        /// </summary>
        /// <param name="time"></param>
        public void Init(float time)
        {
            releaseTime = time;
            pool = new List<InPoolObject>();
        }

        /// <summary>
        /// ȡ������
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
        /// ���ն���
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public virtual void Recycle(string name, Object obj)
        {
            InPoolObject po = new InPoolObject(name, obj);
            pool.Add(po);
        }

        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        public virtual void Release()
        {
        }

        private void Update()
        {
            //���ھ������ʹ��ʱ�䳬���˶�����ͷ�ʱ�䣬���ͷ���Դ
            if (System.DateTime.Now.Ticks - lastReleaseTime >= releaseTime * 10000000)
            {
                lastReleaseTime = System.DateTime.Now.Ticks;
                Release();
            }
        }
    }
}