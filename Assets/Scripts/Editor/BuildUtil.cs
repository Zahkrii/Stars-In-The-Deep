using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework.Utils;
using System.Linq;
using Framework;
using System;
using Sirenix.Serialization;

namespace Framework
{
    public class BuildUtil : Editor
    {
        [MenuItem("Tools/Build Bundle for Windows")]
        public static void BundleBuildWindows()
        {
            Build(BuildTarget.StandaloneWindows);
        }

        [MenuItem("Tools/Build Bundle for Android")]
        public static void BundleBuildAndroid()
        {
            Build(BuildTarget.Android);
        }

        [MenuItem("Tools/Build Bundle for iOS")]
        public static void BundleBuildiOS()
        {
            Build(BuildTarget.iOS);
        }

        private static void Build(BuildTarget buildTarget)
        {
            // 需要打包的资源列表
            List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
            // 文件信息列表，存储路径名称依赖文件等
            List<BundleInfo> bundleInfos = new List<BundleInfo>();
            // 从目录获取需要打包的资源文件名
            string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                // 排除 meta 文件
                if (file.EndsWith(".meta"))
                    continue;
                // 建立资源
                AssetBundleBuild assetBundle = new AssetBundleBuild();
                string fileName = PathUtil.FormatPathToStandard(file);
                // 需要打包的资源名称，包含相对路径 https://docs.unity.cn/cn/2021.3/ScriptReference/AssetBundleBuild-assetNames.html
                string assetName = PathUtil.GetUnityRelativePath(fileName);
                assetBundle.assetNames = new string[] { assetName };
#if UNITY_EDITOR
                Debug.Log($"正在打包：{assetName}");
#endif
                // 打包后的 bundle 名称，包含相对路径
                string bundleName = fileName.Replace(PathUtil.BuildResourcesPath, "").ToLower().Substring(1);
                assetBundle.assetBundleName = bundleName + ".bundle";
                // 添加到资源列表
                assetBundleBuilds.Add(assetBundle);

                // 获取依赖文件信息
                List<string> dependencies = GetDependency(assetName);
                BundleInfo bundleInfo = new BundleInfo(assetName, bundleName + ".bundle", dependencies);
                bundleInfos.Add(bundleInfo);
            }
            // 清空目录
            if (Directory.Exists(PathUtil.BundleOutputPath))
                Directory.Delete(PathUtil.BundleOutputPath, true);
            Directory.CreateDirectory(PathUtil.BundleOutputPath);
            // 打包
            BuildPipeline.BuildAssetBundles(PathUtil.BundleOutputPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, buildTarget);

            SaveToFile(bundleInfos);

            // 刷新资源
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 获取资源依赖文件
        /// </summary>
        /// <param name="curFile">资源文件</param>
        /// <returns>依赖文件</returns>
        private static List<string> GetDependency(string curFile)
        {
            List<string> dependencies = new List<string>();
            string[] files = AssetDatabase.GetDependencies(curFile);
            // 去除脚本文件以及资源本身，剩下的才是依赖文件，如纹理，Sprite等
            dependencies = files.Where(file => !file.EndsWith(".cs") && !file.Equals(curFile)).ToList();
            return dependencies;
        }

        private static void SaveToFile(object data)
        {
            string path = Path.Combine(PathUtil.BundleOutputPath, Constant.FileListName);
            byte[] bytes = Sirenix.Serialization.SerializationUtility.SerializeValue(data, DataFormat.Binary);

            try
            {
                File.WriteAllBytes(path, bytes);
#if UNITY_EDITOR
                Debug.Log($"Save data succeed! \n{Application.persistentDataPath}");
#endif
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError($"Save data failed!\nPath:{Application.persistentDataPath}\n{e}");
#endif
            }
        }
    }
}