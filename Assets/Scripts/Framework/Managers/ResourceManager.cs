using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Framework.Utils;
using System;
using Sirenix.Serialization;
using UObject = UnityEngine.Object;

namespace Framework.Managers
{
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
                    Manager.LuaManager.LuaNames.Add(info.AssetName);
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// 编辑器环境加载资源
        /// </summary>
        /// <param name="assetName">资源名</param>
        /// <param name="action">回调</param>
        public void LoadAssetInEditorMode(string assetName, AssetType type, Action<UObject> action = null)
        {
            Debug.Log("现在是编辑器资源加载模式");
            UObject obj = null;
            switch (type)
            {
                case AssetType.UI:
                    obj = UnityEditor.AssetDatabase.LoadAssetAtPath(PathUtil.GetUIPath(assetName), typeof(UObject));
                    break;

                case AssetType.Lua:
                    obj = UnityEditor.AssetDatabase.LoadAssetAtPath(PathUtil.GetLuaPath(assetName), typeof(UObject));
                    break;

                case AssetType.Effect:
                    obj = UnityEditor.AssetDatabase.LoadAssetAtPath(PathUtil.GetEffectPath(assetName), typeof(UObject));
                    break;

                case AssetType.Scene:
                    obj = UnityEditor.AssetDatabase.LoadAssetAtPath(PathUtil.GetScenePath(assetName), typeof(UObject));
                    break;

                case AssetType.Prefab:
                    obj = UnityEditor.AssetDatabase.LoadAssetAtPath(PathUtil.GetPrefabPath(assetName), typeof(UObject));
                    break;

                default:
                    break;
            }
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
            if (Constant.AssetsLoadMode == AssetsLoadMode.PackageBundle || Constant.AssetsLoadMode == AssetsLoadMode.Hotfix)
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
#if UNITY_EDITOR
            else
            {
                // 加载资源（编辑器模式）
                LoadAssetInEditorMode(assetName, type, action);
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
            if (Constant.AssetsLoadMode == AssetsLoadMode.PackageBundle || Constant.AssetsLoadMode == AssetsLoadMode.Hotfix)
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
                // 加载资源（编辑器模式）
                LoadAssetInEditorMode(assetName, type, action);
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
}