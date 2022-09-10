using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaBehaviour : MonoBehaviour
{
    private LuaEnv luaEnvironment = Manager.LuaManager.LuaEnvironment;

    // �̳е�������Ҫ����
    protected LuaTable scriptEnv;

    #region ���෽��

    private Action luaOnInit;

    //private Action luaStart;
    private Action luaUpdate;

    private Action luaOnDestroy;

    #endregion ���෽��

    //Ϊ��ʵ�ֽ���ϣ�����unity�ķ�������lua��ģ������Ƶ�����
    private void Awake()
    {
        scriptEnv = luaEnvironment.NewTable();
        // Ϊÿ���ű����õ�����������ֹȫ�ֱ����ͷ�������ͻ
        LuaTable meta = luaEnvironment.NewTable();
        meta.Set("__index", luaEnvironment.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);
    }

    /// <summary>
    /// ʵ������Awake�ķ���
    /// </summary>
    /// <param name="luaScript">lua�ű���</param>
    public virtual void Init(string luaScript)
    {
        luaEnvironment.DoString(Manager.LuaManager.GetLuaScript(luaScript), luaScript, scriptEnv);
        scriptEnv.Get("OnInit", out luaOnInit);
        scriptEnv.Get("Update", out luaUpdate);
        scriptEnv.Get("OnDestroy", out luaOnDestroy);

        luaOnInit?.Invoke();
    }

    private void Update()
    {
        luaUpdate?.Invoke();
    }

    /// <summary>
    /// �ͷ���Դ��ͬʱ�ṩ���าд
    /// </summary>
    protected virtual void Clear()
    {
        luaOnDestroy = null;
        luaOnInit = null;
        luaUpdate = null;
        scriptEnv?.Dispose();
        scriptEnv = null;
    }

    private void OnDestroy()
    {
        luaOnDestroy?.Invoke();
        Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}