using Framework;
using System;
using UnityEngine;
using XLua;
using Framework.Managers;

namespace Framework
{
    public class LuaBehaviour : MonoBehaviour
    {
        private LuaEnv luaEnvironment = Manager.LuaManager.LuaEnvironment;

        // 继承的子类需要访问
        protected LuaTable scriptEnv;

        #region 各类方法

        private Action luaOnInit;

        private Action luaUpdate;

        private Action luaOnDestroy;

        #endregion 各类方法

        //为了实现调用时绑定预制体和lua脚本，舍弃unity的方法，在lua中模拟出类似的特性
        private void Awake()
        {
            scriptEnv = luaEnvironment.NewTable();
            // 为每个脚本设置单独环境，防止全局变量和方法名冲突
            LuaTable meta = luaEnvironment.NewTable();
            meta.Set("__index", luaEnvironment.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            scriptEnv.Set("self", this);
        }

        /// <summary>
        /// 实现类似Awake的方法
        /// </summary>
        /// <param name="luaScript">lua脚本名</param>
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
        /// 释放资源，同时提供子类覆写
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
}