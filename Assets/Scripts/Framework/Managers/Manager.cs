using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class Manager : MonoBehaviour
    {
        private static ResourceManager _resourceManager;
        private static LuaManager _luaManager;

        public static ResourceManager resourceManager
        {
            get { return _resourceManager; }
        }

        public static LuaManager luaManager
        {
            get { return _luaManager; }
        }

        private void Awake()
        {
            _resourceManager = this.gameObject.AddComponent<ResourceManager>();
            _luaManager = this.gameObject.AddComponent<LuaManager>();
        }
    }
}