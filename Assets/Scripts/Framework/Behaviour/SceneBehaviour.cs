using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBehaviour : LuaBehaviour
{
    private string sceneName;

    private Action luaOnActive;
    private Action luaInactive;
    private Action luaLoad;
    private Action luaUnload;

    public string SceneName { get => sceneName; set => sceneName = value; }

    public override void Init(string luaScript)
    {
        base.Init(luaScript);
        scriptEnv.Get("OnActive", out luaOnActive);
        scriptEnv.Get("Inactive", out luaInactive);
        scriptEnv.Get("Load", out luaLoad);
        scriptEnv.Get("Unload", out luaUnload);
    }

    public void OnActive()
    {
        luaOnActive?.Invoke();
    }

    public void Inactive()
    {
        luaInactive?.Invoke();
    }

    public void Load()
    {
        luaLoad?.Invoke();
    }

    public void Unload()
    {
        luaUnload?.Invoke();
    }

    protected override void Clear()
    {
        base.Clear();
        luaInactive = null;
        luaOnActive = null;
        luaLoad = null;
        luaUnload = null;
    }
}