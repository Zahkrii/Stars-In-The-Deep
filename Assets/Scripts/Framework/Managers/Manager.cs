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
        private static AudioManager _audioManager;
        private static EventManager _eventManager;
        private static PoolManager _poolManager;

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

        public static AudioManager AudioManager
        {
            get { return _audioManager; }
        }

        public static EventManager EventManager
        {
            get { return _eventManager; }
        }

        public static PoolManager PoolManager
        {
            get { return _poolManager; }
        }

        private void Awake()
        {
            _resourceManager = this.gameObject.AddComponent<ResourceManager>();
            _luaManager = this.gameObject.AddComponent<LuaManager>();
            _uiManager = this.gameObject.AddComponent<UIManager>();
            _entityManager = this.gameObject.AddComponent<EntityManager>();
            _sceneManager = this.gameObject.AddComponent<SceneManager>();
            _audioManager = this.gameObject.AddComponent<AudioManager>();
            _eventManager = this.gameObject.AddComponent<EventManager>();
            _poolManager = this.gameObject.AddComponent<PoolManager>();
        }
    }
}