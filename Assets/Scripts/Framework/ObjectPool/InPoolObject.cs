using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class InPoolObject
    {
        //具体对象
        public Object Object;

        //对象名称
        public string Name;

        //最后使用时间
        public System.DateTime LastUsedTime;

        public InPoolObject(string name, Object obj)
        {
            Name = name;
            Object = obj;
            //创建对象的同时即最后使用时间
            LastUsedTime = System.DateTime.Now;
        }
    }
}