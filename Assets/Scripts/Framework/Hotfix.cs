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
        private byte[] fileListData;

        /// <summary>
        /// �ӵ�ַ���ص����ļ�
        /// </summary>
        /// <param name="info">�����ļ���Ϣ</param>
        /// <param name="complete">��ɻص�</param>
        /// <returns></returns>
        private IEnumerator DownloadFile(DownloadFileInfo info, Action<DownloadFileInfo> complete)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(info.url);
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"�Ӹõ�ַ���س�����{info.url}");
                yield break;
                //TODO: ��������
            }
            info.fileData = webRequest.downloadHandler;
            complete?.Invoke(info);
            // ����
            webRequest.Dispose();
        }

        /// <summary>
        /// �ӵ�ַ���ض���ļ�
        /// </summary>
        /// <param name="infos">�����ļ���Ϣ</param>
        /// <param name="complete">������ɻص�</param>
        /// <param name="allComplete">��ɻص�</param>
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
        /// �Ӱ汾�ļ����������ļ���Ϣ�б�
        /// </summary>
        /// <param name="fileData">�ļ�����</param>
        /// <param name="path">�ļ�·��</param>
        /// <returns>�����ļ���Ϣ�б�</returns>
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
        /// ��Զ�̵�ַ�����ļ�
        /// </summary>
        private void CheckForUpdates()
        {
            // ��Զ�̵�ַ��ȡ�汾�ļ�
            string url = Path.Combine(Constant.ResourceURL, Constant.FileListName);

            DownloadFileInfo fileInfo = new DownloadFileInfo();
            fileInfo.url = url;
            // ��ȡ�ļ����ݣ���ͨ���ص�����
            StartCoroutine(DownloadFile(fileInfo, onRemoteFileListComplete));
        }

        /// <summary>
        /// ��Զ�̵�ַ��ȡ���汾�ļ������
        /// </summary>
        /// <param name="file">�汾�ļ���Ϣ</param>
        private void onRemoteFileListComplete(DownloadFileInfo file)
        {
            fileListData = file.fileData.data;
            // ��Զ�̵�ַ��ȡ�ļ���Ϣ�����Դ��ݵ� path Ϊ Constant.ResourceURL
            List<DownloadFileInfo> fileInfos = GetFileList(file.fileData.data, Constant.ResourceURL);
            // ��Ҫ���ص��ļ���Ϣ�б�����ȫ�����أ�
            List<DownloadFileInfo> downloadFileInfos = new List<DownloadFileInfo>();

            foreach (DownloadFileInfo fileInfo in fileInfos)
            {
                string localFile = Path.Combine(PathUtil.ReadAndWritePath, fileInfo.fileName);
                // ��������ļ������������ӵ����������б�
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
        /// ���Զ������һ���ļ�
        /// </summary>
        /// <param name="obj"></param>
        private void onUpdateComplete(DownloadFileInfo obj)
        {
            Debug.Log($"onUpdateComplete: {obj.url}");
            string path = Path.Combine(PathUtil.ReadAndWritePath, obj.fileName);
            Utils.FileUtil.WriteFile(path, obj.fileData.data);
        }

        /// <summary>
        /// ��ֻ��Ŀ¼�ͷ��ļ�
        /// </summary>
        private void ReleaseResources()
        {
            // ��ֻ��Ŀ¼��ȡ�汾�ļ�
            string url = Path.Combine(PathUtil.ReadOnlyPath, Constant.FileListName);

            DownloadFileInfo fileInfo = new DownloadFileInfo();
            fileInfo.url = url;
            // ��ȡ�ļ����ݣ���ͨ���ص�����
            StartCoroutine(DownloadFile(fileInfo, onGetFileListComplete));
        }

        /// <summary>
        /// ��ֻ��Ŀ¼��ȡ���汾�ļ������
        /// </summary>
        /// <param name="file">�汾�ļ���Ϣ</param>
        private void onGetFileListComplete(DownloadFileInfo file)
        {
            // ��ֻ��Ŀ¼�ͷ��ļ������Դ��ݵ� path Ϊ PathUtil.ReadOnlyPath
            List<DownloadFileInfo> fileInfos = GetFileList(file.fileData.data, PathUtil.ReadOnlyPath);

            // ��������
            fileListData = file.fileData.data;

            StartCoroutine(DownloadFile(fileInfos, onReleaseFileComplete, onReleaseAllFileComplete));
        }

        /// <summary>
        /// �������һ���ļ������
        /// </summary>
        /// <param name="obj">�����ļ���Ϣ</param>
        private void onReleaseFileComplete(DownloadFileInfo obj)
        {
            Debug.Log($"onReleaseFileComplete: {obj.url}");
            string path = Path.Combine(PathUtil.ReadAndWritePath, obj.fileName);
            Utils.FileUtil.WriteFile(path, obj.fileData.data);
        }

        /// <summary>
        /// �����ļ�������ɺ����
        /// </summary>
        private void onReleaseAllFileComplete()
        {
            Utils.FileUtil.WriteFile(Path.Combine(PathUtil.ReadAndWritePath, Constant.FileListName), fileListData);
            CheckForUpdates();
        }

        /// <summary>
        /// �ж��Ƿ��״ΰ�װ
        /// </summary>
        /// <returns>�Ƿ��״ΰ�װ</returns>
        private bool IsFirstInstall()
        {
            // �ж�ֻ��Ŀ¼�Ƿ���ڰ汾�ļ�
            bool isExistsInReadOnlyPath = Utils.FileUtil.IsExists(Path.Combine(PathUtil.ReadOnlyPath, Constant.FileListName));
            // �жϿɶ�дĿ¼�Ƿ���ڰ汾�ļ�
            bool isExistsInRWPath = Utils.FileUtil.IsExists(Path.Combine(PathUtil.ReadAndWritePath, Constant.FileListName));

            // ֻ��Ŀ¼���ڣ��ɶ�дĿ¼�����ڰ汾�ļ������״ΰ�װ
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