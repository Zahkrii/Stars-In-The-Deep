using Framework.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

namespace Framework.Managers
{
    public class LuaManager : MonoBehaviour
    {
        // 所有lua脚本文件路径名, 待加载列表
        public List<string> LuaNames = new List<string>();

        // 已加载的脚本列表
        private Dictionary<string, byte[]> luaScriptCache;

        // lua 虚拟机，全局唯一
        public LuaEnv LuaEnvironment;

        // 初始化完成，回调
        //private Action initComplete;

        /// <summary>
        /// 初始化，加载脚本
        /// </summary>
        public void Init()//public void Init(Action complete)
        {
            //initComplete += complete;
            luaScriptCache = new Dictionary<string, byte[]>();

            LuaEnvironment = new LuaEnv();
            //LuaEnvironment.DoString("util =  require('xlua.util')");
            LuaEnvironment.AddLoader(LuaLoader);

            if (Constant.AssetsLoadMode == AssetsLoadMode.PackageBundle || Constant.AssetsLoadMode == AssetsLoadMode.Hotfix)
            {
                LoadLuaScript();
            }
#if UNITY_EDITOR
            else
            {
                EditorLoadLuaScript();
            }
#endif
        }

        private void Update()
        {
            if (LuaEnvironment != null)
            {
                // 资源释放
                LuaEnvironment.Tick();
            }
        }

        private void OnDestroy()
        {
            if (LuaEnvironment != null)
            {
                // 销毁
                LuaEnvironment.Dispose();
                LuaEnvironment = null;
            }
        }

        /// <summary>
        /// 加载lua脚本
        /// </summary>
        /// <param name="luaName">lua文件名</param>
        public void RequireLua(string luaName)
        {
            LuaEnvironment.DoString($"require '{luaName}'");
        }

        private byte[] LuaLoader(ref string name)
        {
            return GetLuaScript(name);
        }

        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public byte[] GetLuaScript(string name)
        {
            // require ui.register => ui/register
            name = name.Replace(".", "/");
            string path = PathUtil.GetLuaPath(name);

            byte[] luaScript = null;
            // 查找字典里是否存在脚本
            if (!luaScriptCache.TryGetValue(path, out luaScript))
                Debug.LogError("lua script does not exist: " + path);
            //Debug.Log("lua script: " + path);
            return luaScript;
        }

        /// <summary>
        /// 加载所有脚本
        /// </summary>
        private void LoadLuaScript()
        {
            // 循环待加载列表
            foreach (string name in LuaNames)
            {
                Manager.ResourceManager.LoadAsset(name, AssetType.Lua, (UnityEngine.Object obj) =>
                {
                    AddLuaScript(name, (obj as TextAsset).bytes);
                    if (luaScriptCache.Count >= LuaNames.Count) //加载完成
                    {
                        Manager.EventManager.Execute(10000);
                        //initComplete?.Invoke();
                        LuaNames.Clear();
                        LuaNames = null;
                    }
                });
            }
        }

        private void AddLuaScript(string assetName, byte[] script)
        {
            // 不用 luaScriptCache.Add(assetName, script); 防止重复操作报错
            luaScriptCache[assetName] = script;
        }

#if UNITY_EDITOR

        /// <summary>
        /// 编辑器模式加载所有脚本
        /// </summary>
        private void EditorLoadLuaScript()
        {
            string[] luaFiles = Directory.GetFiles(PathUtil.LuaPath, "*.bytes", SearchOption.AllDirectories);
            foreach (string luaFile in luaFiles)
            {
                string fileName = PathUtil.FormatPathToStandard(luaFile);
                byte[] file = File.ReadAllBytes(fileName);
                AddLuaScript(PathUtil.GetUnityRelativePath(fileName), file);
            }
            //initComplete?.Invoke();
            Manager.EventManager.Execute(10000);
        }

#endif
    }
}