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

        Manager.resourceManager.ParseVersionFile();
        Manager.luaManager.Init(() =>
        {
            Manager.luaManager.ExecuteLua("test");
        });
    }
}