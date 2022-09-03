using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework.Utils;

public class BuildTool : Editor
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
        // 从目录获取需要打包的资源文件名
        string[] files = Directory.GetFiles(PathUtil.BundleResourcesPath, "*", SearchOption.AllDirectories);
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
            // 打包后的 bundle 名称，包含相对路径
            string bundleName = fileName.Replace(PathUtil.BundleResourcesPath, "").ToLower();
            assetBundle.assetBundleName = bundleName + ".bundle";
            // 添加到资源列表
            assetBundleBuilds.Add(assetBundle);
        }
        // 清空目录
        if (Directory.Exists(PathUtil.BundleOutputPath))
            Directory.Delete(PathUtil.BundleOutputPath, true);
        Directory.CreateDirectory(PathUtil.BundleOutputPath);
        // 打包
        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutputPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, buildTarget);
    }
}