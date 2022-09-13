using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Managers
{
    public class Manager : MonoBehaviour
    {
        private static ResourceManager _resourceManager;
        private static LuaManager _luaManager;
        private static UIManager _uiManager;
        private static EntityManager _entityManager;
        private static SceneManager _sceneManager;

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

        public static EntityManager EntityManager
        {
            get { return _entityManager; }
        }

        public static SceneManager SceneManager
        {
            get { return _sceneManager; }
        }

        private void Awake()
        {
            _resourceManager = this.gameObject.AddComponent<ResourceManager>();
            _luaManager = this.gameObject.AddComponent<LuaManager>();
            _uiManager = this.gameObject.AddComponent<UIManager>();
            _entityManager = this.gameObject.AddComponent<EntityManager>();
            _sceneManager = this.gameObject.AddComponent<SceneManager>();
        }
    }
}