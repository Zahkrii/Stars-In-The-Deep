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
        public static readonly string BundleResourcesPath = AssetsPath + "/BundleResources";

        // 打包资源输出目录
        public static readonly string BundleOutputPath = Application.streamingAssetsPath;

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
    }
}