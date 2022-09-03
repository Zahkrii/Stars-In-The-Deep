using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static LuaManager luaManager;

    public static LuaManager LuaManager
    {
        get { return luaManager; }
    }

    private void Awake()
    {
        luaManager = this.gameObject.AddComponent<LuaManager>();
    }
}