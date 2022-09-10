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
        private static UIManager _uiManager;

        public static ResourceManager ResourceManager
        {
            get { return _resourceManager; }
        }

        public static LuaManager LuaManager
        {
            get { return _luaManager; }
        }

        public static UIManager UIManager
        {
            get { return _uiManager; }
        }

        private void Awake()
        {
            _resourceManager = this.gameObject.AddComponent<ResourceManager>();
            _luaManager = this.gameObject.AddComponent<LuaManager>();
            _uiManager = this.gameObject.AddComponent<UIManager>();
        }
    }
}