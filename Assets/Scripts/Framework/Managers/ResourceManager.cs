using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Framework.Utils;
using UnityEngine.Playables;
using Sirenix.Serialization;

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
    }
}