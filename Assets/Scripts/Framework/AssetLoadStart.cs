using Framework.Managers;
using UnityEngine;

namespace Framework
{
    public class AssetLoadStart : MonoBehaviour
    {
        public AssetsLoadMode loadMode;

        private void Awake()
        {
            Constant.AssetsLoadMode = loadMode;
            DontDestroyOnLoad(this);

            Manager.ResourceManager.ParseVersionFile();
            Manager.LuaManager.Init(() =>
            {
                Manager.LuaManager.RequireLua("test");
                XLua.LuaFunction function = Manager.LuaManager.LuaEnvironment.Global.Get<XLua.LuaFunction>("Test");
                function.Call();
            });
        }
    }
}