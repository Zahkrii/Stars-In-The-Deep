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
    [Serializable]
    public class BundleInfo//包信息
    {
        //资源名
        public string AssetName;

        //包名
        public string BundleName;

        //依赖文件列表
        public List<string> Dependency;

        public BundleInfo(string assetName, string bundleName, List<string> dependency)
        {
            AssetName = assetName;
            BundleName = bundleName;
            Dependency = dependency;
        }
    }

    internal class BundleData//包数据
    {
        public AssetBundle Bundle;

        //引用计数
        public int RefCount;

        public BundleData(AssetBundle bundle)
        {
            Bundle = bundle;
            RefCount = 1;
        }
    }

    public class ResourceManager : MonoBehaviour
    {
        // 资源名与包信息一一对应的字典
        private Dictionary<string, BundleInfo> bundleInfoDic = new Dictionary<string, BundleInfo>();

        //存储已加载的bundle信息
        private Dictionary<string, BundleData> assetBundleDic = new Dictionary<string, BundleData>();

        /// <summary>
        /// 解析版本文件（文件清单）
        /// </summary>
        public void ParseVersionFile()
        {
            string path = Path.Combine(PathUtil.BundleResourcesPath, Constant.FileListName);

            byte[] bytes = File.ReadAllBytes(path);

            List<BundleInfo> bundleInfolist = Sirenix.Serialization.SerializationUtility.DeserializeValue<List<BundleInfo>>(bytes, DataFormat.JSON);

            foreach (BundleInfo info in bundleInfolist)
            {
                bundleInfoDic.Add(info.AssetName, info);

                // 如果是lua脚本，则添加到管理器
                if (info.AssetName.IndexOf("LuaScripts") > 0)
                    Manager.LuaManager.LuaNames.Add(info.AssetName);
            }
        }

        private BundleData GetBundle(string name)
        {
            BundleData bundle = null;
            if (assetBundleDic.TryGetValue(name, out bundle))
            {
                bundle.RefCount++;
                return bundle;
            }
            return null;
        }

#if UNITY_EDITOR

        /// <summary>
        /// 编辑器环境加载资源
        /// </summary>
        /// <param name="assetName">资源名</param>
        /// <param name="action">回调</param>
        private void LoadAssetInEditorMode(string assetName, AssetType type, Action<UObject> action = null, string extension = null)
        {
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

                case AssetType.Model:
                    obj = UnityEditor.AssetDatabase.LoadAssetAtPath(PathUtil.GetModelPath(assetName), typeof(UObject));
                    break;

                case AssetType.Music:
                    obj = UnityEditor.AssetDatabase.LoadAssetAtPath(PathUtil.GetMusicPath(assetName, extension), typeof(UObject));
                    break;

                case AssetType.Sound:
                    obj = UnityEditor.AssetDatabase.LoadAssetAtPath(PathUtil.GetSoundPath(assetName, extension), typeof(UObject));
                    break;

                case AssetType.Sprite:
                    obj = UnityEditor.AssetDatabase.LoadAssetAtPath(PathUtil.GetSpritePath(assetName, extension), typeof(UObject));
                    break;

                default:
                    break;
            }
            if (obj == null)
                Debug.LogError($"编辑器资源加载模式，资源不存在：{assetName}");
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

                    case AssetType.Model:
                        StartCoroutine(LoadBundleAsync(PathUtil.GetModelPath(assetName), action));
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
                LoadAssetInEditorMode(assetName, type, action, extension);
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

            BundleData bundle = GetBundle(bundleName);
            if (bundle == null)
            {
                UObject obj = Manager.PoolManager.Spwan("AssetBundle", bundleName);
                if (obj != null)
                {
                    AssetBundle assetBundle = obj as AssetBundle;
                    bundle = new BundleData(assetBundle);
                }
                else
                {
                    AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
                    yield return request;
                    bundle = new BundleData(request.assetBundle);
                }

                assetBundleDic.Add(bundleName, bundle);
            }

            if (dependencies != null && dependencies.Count > 0)
            {
                foreach (string dependency in dependencies)
                {
                    yield return LoadBundleAsync(dependency);
                }
            }

            //场景资源并不需要 bundleRequest
            if (assetName.EndsWith(".unity"))
            {
                action?.Invoke(null);
                yield break;
            }

            //当加载依赖资源时，没有回调
            if (action == null)
            {
                yield break;
            }

            AssetBundleRequest bundleRequest = bundle.Bundle.LoadAssetAsync(assetName);
            yield return bundleRequest;

            Debug.Log("现在是 Bundle 资源加载模式");

            action?.Invoke(bundleRequest?.asset);
        }

        /// <summary>
        /// 卸载 bundle
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void UnloadBundle(UObject obj)
        {
            AssetBundle ab = obj as AssetBundle;
            ab.Unload(true);
        }

        public void MinusBundleRefCount(string assetName)
        {
            string bundleName = bundleInfoDic[assetName].BundleName;

            MinusOneBundleRefCount(bundleName);

            List<string> dependencies = bundleInfoDic[assetName].Dependency;
            if (dependencies != null && dependencies.Count > 0)
            {
                foreach (string dependency in dependencies)
                {
                    string name = bundleInfoDic[dependency].BundleName;
                    MinusOneBundleRefCount(name);
                }
            }
        }

        private void MinusOneBundleRefCount(string bundleName)
        {
            if (assetBundleDic.TryGetValue(bundleName, out BundleData bundle))
            {
                if (bundle.RefCount > 0)
                {
                    bundle.RefCount--;
                    Debug.Log($"bundle: {bundleName} 引用计数：{bundle.RefCount}");
                }
                if (bundle.RefCount <= 0)
                {
                    Debug.Log("存入对象池：" + bundleName);
                    Manager.PoolManager.Recycle("AssetBundle", bundleName, bundle.Bundle);
                    assetBundleDic.Remove(bundleName);
                }
            }
        }
    }
}