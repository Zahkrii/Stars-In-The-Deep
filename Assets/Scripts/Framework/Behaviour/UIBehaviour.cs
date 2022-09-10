using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBehaviour : LuaBehaviour
{
    //UI´ò¿ª
    private Action luaOnOpen;

    //UI¹Ø±Õ
    private Action luaOnClose;

    public override void Init(string luaScript)
    {
        base.Init(luaScript);
        scriptEnv.Get("OnOpen", out luaOnOpen);
        scriptEnv.Get("OnClose", out luaOnClose);
    }

    public void OnOpen()
    {
        luaOnOpen?.Invoke();
    }

    public void OnClose()
    {
        luaOnClose?.Invoke();
    }

    protected override void Clear()
    {
        base.Clear();
        luaOnOpen = null;
        luaOnClose = null;
    }
}