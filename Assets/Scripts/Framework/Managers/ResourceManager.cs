using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Framework.Utils;
using System;
using Sirenix.Serialization;
using UObject = UnityEngine.Object;
using Framework;

public class ResourceManager : MonoBehaviour
{
    // 资源名与包信息一一对应的字典
    private Dictionary<string, BundleInfo> bundleInfoDic = new Dictionary<string, BundleInfo>();

    /// <summary>
    /// 解析版本文件（文件清单）
    /// </summary>
    public void ParseVersionFile()
    {
        string path = Path.Combine(PathUtil.BundleResourcesPath, Constant.FileListName);

        byte[] bytes = File.ReadAllBytes(path);

        List<BundleInfo> bundleInfolist = Sirenix.Serialization.SerializationUtility.DeserializeValue<List<BundleInfo>>(bytes, DataFormat.Binary);

        foreach (BundleInfo info in bundleInfolist)
        {
            bundleInfoDic.Add(info.AssetName, info);

            // 如果是lua脚本，则添加到管理器
            if (info.AssetName.IndexOf("LuaScripts") > 0)
                Manager.luaManager.LuaNames.Add(info.AssetName);
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// 编辑器环境加载资源
    /// </summary>
    /// <param name="assetName">资源名</param>
    /// <param name="action">回调</param>
    public void LoadAssetInEditorMode(string assetName, Action<UObject> action = null)
    {
        Debug.Log("现在是编辑器资源加载模式");
        UObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, typeof(UObject));
        if (obj == null)
            Debug.LogError($"资源不存在：{assetName}");
        action?.Invoke(obj);
    }

#endif

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="assetName">资源名</param>
    /// <param name="type">资源类型</param>
    /// <param name="action">回调函数</param>
    public void LoadAsset(string assetName, AssetType type, Action<UObject> action = null)
    {
        if (Constant.AssetsLoadMode == AssetsLoadMode.PackageBundle || Constant.AssetsLoadMode == AssetsLoadMode.HotUpdate)
        {
            switch (type)
            {
                case AssetType.UI:
                    StartCoroutine(LoadBundleAsync(PathUtil.GetUIPath(assetName), action));
                    break;

                case AssetType.Lua:
                    StartCoroutine(LoadBundleAsync(assetName, action));
                    break;

                case AssetType.Effect:
                    StartCoroutine(LoadBundleAsync(PathUtil.GetEffectPath(assetName), action));
                    break;

                case AssetType.Scene:
                    StartCoroutine(LoadBundleAsync(PathUtil.GetScenePath(assetName), action));
                    break;

                default:
                    break;
            }
        }
        // 加载资源（编辑器模式）
#if UNITY_EDITOR
        else
        {
            LoadAssetInEditorMode(assetName, action);
        }
#endif
    }

    /// <summary>
    /// 加载资源（带扩展名）
    /// </summary>
    /// <param name="assetName">资源名</param>
    /// <param name="extension">资源扩展名</param>
    /// <param name="type">资源类型</param>
    /// <param name="action">回调函数</param>
    public void LoadAsset(string assetName, string extension, AssetType type, Action<UObject> action = null)
    {
        // 加载资源（编辑器模式）
        if (Constant.AssetsLoadMode == AssetsLoadMode.PackageBundle || Constant.AssetsLoadMode == AssetsLoadMode.HotUpdate)
        {
            switch (type)
            {
                case AssetType.Music:
                    StartCoroutine(LoadBundleAsync(PathUtil.GetMusicPath(assetName, extension), action));
                    break;

                case AssetType.Sound:
                    StartCoroutine(LoadBundleAsync(PathUtil.GetSoundPath(assetName, extension), action));
                    break;

                case AssetType.Sprite:
                    StartCoroutine(LoadBundleAsync(PathUtil.GetSpritePath(assetName, extension), action));
                    break;

                default:
                    break;
            }
        }
#if UNITY_EDITOR
        else
        {
            LoadAssetInEditorMode(assetName, action);
        }
#endif
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="assetName">资源名</param>
    /// <param name="action">完成回调</param>
    /// <returns></returns>
    private IEnumerator LoadBundleAsync(string assetName, Action<UObject> action = null)
    {
        string bundleName = bundleInfoDic[assetName].BundleName;
        string bundlePath = Path.Combine(PathUtil.BundleResourcesPath, bundleName);
        List<string> dependencies = bundleInfoDic[assetName].Dependency;
        if (dependencies != null && dependencies.Count > 0)
        {
            foreach (string dependency in dependencies)
            {
                yield return LoadBundleAsync(dependency);
            }
        }

        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return request;

        AssetBundleRequest bundleRequest = request.assetBundle.LoadAssetAsync(assetName);
        yield return bundleRequest;

        Debug.Log("现在是 Bundle 资源加载模式");
        //if (action != null && bundleRequest != null)
        //{
        //    action.Invoke(bundleRequest.asset);
        //}
        action?.Invoke(bundleRequest?.asset);
    }
}