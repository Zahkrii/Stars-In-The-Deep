using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework.Utils;
using System.Linq;
using System;
using Sirenix.Serialization;
using Framework.Managers;

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
            // ��Ҫ�������Դ�б�
            List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
            // �ļ���Ϣ�б����洢·�����������ļ���
            List<BundleInfo> bundleInfos = new List<BundleInfo>();
            // ��Ŀ¼��ȡ��Ҫ�������Դ�ļ���
            string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                // �ų� meta �ļ�
                if (file.EndsWith(".meta"))
                    continue;
                // ������Դ
                AssetBundleBuild assetBundle = new AssetBundleBuild();
                string fileName = PathUtil.FormatPathToStandard(file);
                // ��Ҫ�������Դ���ƣ��������·�� https://docs.unity.cn/cn/2021.3/ScriptReference/AssetBundleBuild-assetNames.html
                string assetName = PathUtil.GetUnityRelativePath(fileName);
                assetBundle.assetNames = new string[] { assetName };
#if UNITY_EDITOR
                Debug.Log($"���ڴ����{assetName}");
#endif
                // ������ bundle ���ƣ��������·��
                string bundleName = fileName.Replace(PathUtil.BuildResourcesPath, "").ToLower().Substring(1);
                assetBundle.assetBundleName = bundleName + ".bundle";
                // ���ӵ���Դ�б�
                assetBundleBuilds.Add(assetBundle);

                // ��ȡ�����ļ���Ϣ
                List<string> dependencies = GetDependency(assetName);
                BundleInfo bundleInfo = new BundleInfo(assetName, bundleName + ".bundle", dependencies);
                bundleInfos.Add(bundleInfo);
            }
            // ���Ŀ¼
            if (Directory.Exists(PathUtil.BundleOutputPath))
                Directory.Delete(PathUtil.BundleOutputPath, true);
            Directory.CreateDirectory(PathUtil.BundleOutputPath);
            // ���
            BuildPipeline.BuildAssetBundles(PathUtil.BundleOutputPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, buildTarget);

            SaveToFile(bundleInfos);

            // ˢ����Դ
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// ��ȡ��Դ�����ļ�
        /// </summary>
        /// <param name="curFile">��Դ�ļ�</param>
        /// <returns>�����ļ�</returns>
        private static List<string> GetDependency(string curFile)
        {
            List<string> dependencies = new List<string>();
            string[] files = AssetDatabase.GetDependencies(curFile);
            // ȥ���ű��ļ��Լ���Դ������ʣ�µĲ��������ļ�����������Sprite��
            dependencies = files.Where(file => !file.EndsWith(".cs") && !file.Equals(curFile) && !file.StartsWith("Packages")).ToList();
            return dependencies;
        }

        private static void SaveToFile(object data)
        {
            string path = Path.Combine(PathUtil.BundleOutputPath, Constant.FileListName);
            byte[] bytes = Sirenix.Serialization.SerializationUtility.SerializeValue(data, DataFormat.JSON);

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