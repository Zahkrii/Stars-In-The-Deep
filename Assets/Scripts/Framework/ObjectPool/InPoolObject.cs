using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class InPoolObject
    {
        //�������
        public Object Object;

        //��������
        public string Name;

        //���ʹ��ʱ��
        public System.DateTime LastUsedTime;

        public InPoolObject(string name, Object obj)
        {
            Name = name;
            Object = obj;
            //���������ͬʱ�����ʹ��ʱ��
            LastUsedTime = System.DateTime.Now;
        }
    }
}