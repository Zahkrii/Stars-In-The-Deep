using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Framework.Utils;
using System;
using Sirenix.Serialization;
using UObject = UnityEngine.Object;

namespace Framework.Manager
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

            List<BundleInfo> bundleInfolist = SerializationUtility.DeserializeValue<List<BundleInfo>>(bytes, DataFormat.Binary);

            foreach (BundleInfo info in bundleInfolist)
            {
                bundleInfoDic.Add(info.AssetName, info);
            }
        }

        public void LoadAsset(string assetName, AssetType type, Action<UObject> action = null)
        {
            switch (type)
            {
                case AssetType.UI:
                    StartCoroutine(LoadBundleAsync(PathUtil.GetUIPath(assetName), action));
                    break;

                case AssetType.Lua:
                    StartCoroutine(LoadBundleAsync(PathUtil.GetLuaPath(assetName), action));
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

        public void LoadAsset(string assetName, AssetType type, string extension, Action<UObject> action = null)
        {
            switch (type)
            {
                case AssetType.Music:
                    StartCoroutine(LoadBundleAsync(PathUtil.GetMusicPath(assetName,extension), action));
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

            //if (action != null && bundleRequest != null)
            //{
            //    action.Invoke(bundleRequest.asset);
            //}
            action?.Invoke(bundleRequest?.asset);
        }

        public void Start()
        {
            ParseVersionFile();
            LoadAsset("Image", AssetType.UI, OnComplete);
        }

        private void OnComplete(UObject obj)
        {
            GameObject gameObject = Instantiate(obj) as GameObject;
            gameObject.transform.SetParent(this.transform);
            gameObject.SetActive(true);
            gameObject.transform.localPosition = Vector3.zero;
        }
    }
}