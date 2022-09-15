using Framework.Managers;
using UnityEngine;

namespace Framework
{
    public class AssetLoadStart : MonoBehaviour
    {
        public AssetsLoadMode loadMode;

        private void Awake()
        {
            Manager.EventManager.Subscribe(10000, OnLuaInit);

            Constant.AssetsLoadMode = loadMode;
            DontDestroyOnLoad(this);

            Manager.ResourceManager.ParseVersionFile();
            Manager.LuaManager.Init();
        }

        private void OnLuaInit(object args)
        {
            Manager.LuaManager.RequireLua("test");
            XLua.LuaFunction function = Manager.LuaManager.LuaEnvironment.Global.Get<XLua.LuaFunction>("Test");
            function.Call();
        }

        private void OnApplicationQuit()
        {
            Manager.EventManager.Unsubscribe(10000, OnLuaInit);
        }
    }
}