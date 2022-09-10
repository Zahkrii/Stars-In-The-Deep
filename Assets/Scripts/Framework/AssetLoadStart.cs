using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Manager.LuaManager.ExecuteLua("test");
            XLua.LuaFunction function = Manager.LuaManager.LuaEnvironment.Global.Get<XLua.LuaFunction>("Test");
            function.Call();
        });
    }
}