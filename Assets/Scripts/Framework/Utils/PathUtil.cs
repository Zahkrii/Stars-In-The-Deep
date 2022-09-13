using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utils
{
    public class PathUtil
    {
        // Assets 目录
        public static readonly string AssetsPath = Application.dataPath;

        // 需要打包的资源目录
        public static readonly string BuildResourcesPath = AssetsPath + "/BundleResources";

        // 打包资源输出目录
        public static readonly string BundleOutputPath = Application.streamingAssetsPath;

        // 资源只读目录
        public static readonly string ReadOnlyPath = Application.streamingAssetsPath;

        // 资源可读写目录
        public static readonly string ReadAndWritePath = Application.persistentDataPath;

        // lua 脚本目录
        public static readonly string LuaPath = AssetsPath + "/BundleResources/LuaScripts";

        // 打包资源目录（如从远程地址或本地地址获取打包好的资源）
        public static string BundleResourcesPath
        {
            get
            {
                // 如果是热更模式就从 persistentDataPath 加载（资源 Bundle 都会下载到这个目录）
                if (Constant.AssetsLoadMode == AssetsLoadMode.Hotfix)
                    return ReadAndWritePath;
                // Package Bundle 模式，从自己包里的 streamingAssetsPath（只读）加载
                return ReadOnlyPath;
            }
        }

        /// <summary>
        /// 从 Assets 绝对路径获取 Assets 相对路径
        /// </summary>
        /// <param name="path">Assets 绝对路径</param>
        /// <returns>Assets 相对路径</returns>
        public static string GetUnityRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            // 从 Assets 开始截取到结束，即相对路径
            return path.Substring(path.IndexOf("Assets"));
        }

        /// <summary>
        /// 规范化路径，去除前后空格及反斜杠化为斜杠
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>规范化的路径</returns>
        public static string FormatPathToStandard(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            return path.Trim().Replace("\\", "/");
        }

        public static string GetLuaPath(string name)
        {
            return $"Assets/BundleResources/LuaScripts/{name}.bytes";
        }

        public static string GetUIPath(string name)
        {
            return $"Assets/BundleResources/UI/Prefabs/{name}.prefab";
        }

        public static string GetMusicPath(string name, string extension)
        {
            return $"Assets/BundleResources/Audio/Music/{name}.{extension}";
        }

        public static string GetSoundPath(string name, string extension)
        {
            return $"Assets/BundleResources/Audio/Sounds/{name}.{extension}";
        }

        public static string GetEffectPath(string name)
        {
            return $"Assets/BundleResources/Effect/Prefabs/{name}.prefab";
        }

        public static string GetSpritePath(string name, string extension)
        {
            return $"Assets/BundleResources/Sprites/{name}.{extension}";
        }

        public static string GetScenePath(string name)
        {
            return $"Assets/BundleResources/Scenes/{name}.unity";
        }

        public static string GetModelPath(string name)
        {
            return $"Assets/BundleResources/Models/Prefabs/{name}.prefab";
        }

        public static string GetPrefabPath(string name)
        {
            return $"Assets/BundleResources/Prefabs/{name}.prefab";
        }
    }
}