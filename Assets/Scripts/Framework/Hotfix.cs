using Framework.Managers;
using Framework.Utils;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UObject = UnityEngine.Object;

namespace Framework
{
    public class Hotfix : MonoBehaviour
    {
        //暂时保存存有所有文件信息的版本文件，最后写入
        private byte[] fileListData;

        /// <summary>
        /// 从地址下载单个文件
        /// </summary>
        /// <param name="info">下载文件信息</param>
        /// <param name="complete">完成回调</param>
        /// <returns></returns>
        private IEnumerator DownloadFile(DownloadFileInfo info, Action<DownloadFileInfo> complete)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(info.url);
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"从该地址下载出错：{info.url}");
                yield break;
                //TODO: 下载重试
            }
            info.fileData = webRequest.downloadHandler;
            complete?.Invoke(info);
            // 销毁
            webRequest.Dispose();
        }

        /// <summary>
        /// 从地址下载多个文件
        /// </summary>
        /// <param name="infos">下载文件信息</param>
        /// <param name="complete">单个完成回调</param>
        /// <param name="allComplete">完成回调</param>
        /// <returns></returns>
        private IEnumerator DownloadFile(List<DownloadFileInfo> infos, Action<DownloadFileInfo> complete, Action allComplete)
        {
            foreach (DownloadFileInfo info in infos)
            {
                yield return DownloadFile(info, complete);
            }
            allComplete?.Invoke();
        }

        /// <summary>
        /// 从版本文件解析下载文件信息列表
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="path">文件路径</param>
        /// <returns>下载文件信息列表</returns>
        private List<DownloadFileInfo> GetFileList(byte[] fileData, string path)
        {
            List<BundleInfo> bundleInfolist = Sirenix.Serialization.SerializationUtility.DeserializeValue<List<BundleInfo>>(fileData, DataFormat.Binary);
            List<DownloadFileInfo> list = new List<DownloadFileInfo>();
            foreach (BundleInfo info in bundleInfolist)
            {
                DownloadFileInfo file = new DownloadFileInfo();
                file.fileName = info.BundleName;
                file.url = Path.Combine(path, info.BundleName);
                list.Add(file);
            }
            return list;
        }

        private void Start()
        {
            if (IsFirstInstall())
            {
                ReleaseResources();
            }
            else
            {
                CheckForUpdates();
            }
        }

        /// <summary>
        /// 从远程地址下载文件
        /// </summary>
        private void CheckForUpdates()
        {
            // 从远程地址获取版本文件
            string url = Path.Combine(Constant.ResourceURL, Constant.FileListName);

            DownloadFileInfo fileInfo = new DownloadFileInfo();
            fileInfo.url = url;
            // 获取文件数据，并通过回调处理
            StartCoroutine(DownloadFile(fileInfo, onRemoteFileListComplete));
        }

        /// <summary>
        /// 从远程地址获取到版本文件后调用
        /// </summary>
        /// <param name="file">版本文件信息</param>
        private void onRemoteFileListComplete(DownloadFileInfo file)
        {
            fileListData = file.fileData.data;
            // 从远程地址获取文件信息，所以传递的 path 为 Constant.ResourceURL
            List<DownloadFileInfo> fileInfos = GetFileList(file.fileData.data, Constant.ResourceURL);
            // 需要下载的文件信息列表（不全部下载）
            List<DownloadFileInfo> downloadFileInfos = new List<DownloadFileInfo>();

            foreach (DownloadFileInfo fileInfo in fileInfos)
            {
                string localFile = Path.Combine(PathUtil.ReadAndWritePath, fileInfo.fileName);
                // 如果本地文件不存在则添加到即将下载列表
                if (!Utils.FileUtil.IsExists(localFile))
                {
                    fileInfo.url = Path.Combine(Constant.ResourceURL, fileInfo.fileName);
                    downloadFileInfos.Add(fileInfo);
                }
            }
            if (downloadFileInfos.Count > 0)
                StartCoroutine(DownloadFile(downloadFileInfos, onUpdateComplete, onUpdateAllComplete));
            else
                EnterGame();
        }

        private void EnterGame()
        {
            Manager.ResourceManager.ParseVersionFile();
            Manager.ResourceManager.LoadAsset("Image", AssetType.UI, OnComplete);
        }

        private void OnComplete(UObject obj)
        {
            GameObject gameObject = Instantiate(obj) as GameObject;
            gameObject.transform.SetParent(this.transform);
            gameObject.SetActive(true);
            gameObject.transform.localPosition = Vector3.zero;
        }

        private void onUpdateAllComplete()
        {
            Utils.FileUtil.WriteFile(Path.Combine(PathUtil.ReadAndWritePath, Constant.FileListName), fileListData);
            EnterGame();
        }

        /// <summary>
        /// 完成远程下载一个文件
        /// </summary>
        /// <param name="obj"></param>
        private void onUpdateComplete(DownloadFileInfo obj)
        {
            Debug.Log($"onUpdateComplete: {obj.url}");
            string path = Path.Combine(PathUtil.ReadAndWritePath, obj.fileName);
            Utils.FileUtil.WriteFile(path, obj.fileData.data);
        }

        /// <summary>
        /// 从只读目录释放文件
        /// </summary>
        private void ReleaseResources()
        {
            // 从只读目录获取版本文件
            string url = Path.Combine(PathUtil.ReadOnlyPath, Constant.FileListName);

            DownloadFileInfo fileInfo = new DownloadFileInfo();
            fileInfo.url = url;
            // 获取文件数据，并通过回调处理
            StartCoroutine(DownloadFile(fileInfo, onGetFileListComplete));
        }

        /// <summary>
        /// 从只读目录获取到版本文件后调用
        /// </summary>
        /// <param name="file">版本文件信息</param>
        private void onGetFileListComplete(DownloadFileInfo file)
        {
            // 从只读目录释放文件，所以传递的 path 为 PathUtil.ReadOnlyPath
            List<DownloadFileInfo> fileInfos = GetFileList(file.fileData.data, PathUtil.ReadOnlyPath);

            // 保存数据
            fileListData = file.fileData.data;

            StartCoroutine(DownloadFile(fileInfos, onReleaseFileComplete, onReleaseAllFileComplete));
        }

        /// <summary>
        /// 完成下载一个文件后调用
        /// </summary>
        /// <param name="obj">下载文件信息</param>
        private void onReleaseFileComplete(DownloadFileInfo obj)
        {
            Debug.Log($"onReleaseFileComplete: {obj.url}");
            string path = Path.Combine(PathUtil.ReadAndWritePath, obj.fileName);
            Utils.FileUtil.WriteFile(path, obj.fileData.data);
        }

        /// <summary>
        /// 所有文件下载完成后调用
        /// </summary>
        private void onReleaseAllFileComplete()
        {
            Utils.FileUtil.WriteFile(Path.Combine(PathUtil.ReadAndWritePath, Constant.FileListName), fileListData);
            CheckForUpdates();
        }

        /// <summary>
        /// 判断是否首次安装
        /// </summary>
        /// <returns>是否首次安装</returns>
        private bool IsFirstInstall()
        {
            // 判断只读目录是否存在版本文件
            bool isExistsInReadOnlyPath = Utils.FileUtil.IsExists(Path.Combine(PathUtil.ReadOnlyPath, Constant.FileListName));
            // 判断可读写目录是否存在版本文件
            bool isExistsInRWPath = Utils.FileUtil.IsExists(Path.Combine(PathUtil.ReadAndWritePath, Constant.FileListName));

            // 只读目录存在，可读写目录不存在版本文件，即首次安装
            return isExistsInReadOnlyPath && !isExistsInRWPath;
        }
    }

    internal class DownloadFileInfo
    {
        public string url;
        public string fileName;
        public DownloadHandler fileData;
    }
}