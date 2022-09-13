using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Framework.Managers
{
    public class SceneManager : MonoBehaviour
    {
        private string sceneScriptObjectName = "SceneScripts";

        private void Awake()
        {
            UnitySceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        /// <summary>
        /// �����л��Ļص�
        /// </summary>
        /// <param name="scene0"></param>
        /// <param name="scene1"></param>
        private void OnActiveSceneChanged(Scene scene0, Scene scene1)
        {
            //��scene0 �л��� scene1

            // ��֤���г����Ѽ���
            if (!scene0.isLoaded || !scene1.isLoaded)
                return;

            SceneBehaviour sceneBehaviour0 = GetSceneBehaviour(scene0);
            SceneBehaviour sceneBehaviour1 = GetSceneBehaviour(scene1);

            sceneBehaviour0?.Inactive();
            sceneBehaviour1?.OnActive();
        }

        /// <summary>
        /// �����
        /// </summary>
        /// <param name="name"></param>
        public void SetActive(string name)
        {
            Scene scene = UnitySceneManager.GetSceneByName(name);
            UnitySceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// ����ģʽ���س�����ԭ������ɾ��
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="luaName"></param>
        public void LoadScene(string sceneName, string luaName)
        {
            Manager.ResourceManager.LoadAsset(sceneName, AssetType.Scene, (UnityEngine.Object obj) =>
            {
                StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Additive));
            });
        }

        /// <summary>
        /// ж�س���
        /// </summary>
        /// <param name="name"></param>
        public void UnloadScene(string sceneName)
        {
            StartCoroutine(UnloadSceneAsync(sceneName));
        }

        private IEnumerator UnloadSceneAsync(string sceneName)
        {
            Scene scene = UnitySceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                Debug.LogError($"{sceneName} unload already.");
                yield break;
            }
            SceneBehaviour sceneBehaviour = GetSceneBehaviour(scene);
            sceneBehaviour?.Unload();
            AsyncOperation asyncOperation = UnitySceneManager.UnloadSceneAsync(scene);
            yield return asyncOperation;
        }

        /// <summary>
        /// ���ҳ����е�SceneScripts����
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        private SceneBehaviour GetSceneBehaviour(Scene scene)
        {
            GameObject[] gameObjects = scene.GetRootGameObjects();
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.name.CompareTo(sceneScriptObjectName) == 0)
                {
                    SceneBehaviour sceneBehaviour = gameObject.GetComponent<SceneBehaviour>();
                    return sceneBehaviour;
                }
            }
            return null;
        }

        /// <summary>
        /// ����ģʽ���س�����ֻ����һ������
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="luaName"></param>
        public void ChangeScene(string sceneName, string luaName)
        {
            Manager.ResourceManager.LoadAsset(sceneName, AssetType.Scene, (UnityEngine.Object obj) =>
            {
                StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Single));
                //SetActive(sceneName);
            });
        }

        private IEnumerator StartLoadScene(string sceneName, string luaName, LoadSceneMode mode)
        {
            if (IsSceneLoaded(sceneName))
                yield break;
            AsyncOperation asyncOperation = UnitySceneManager.LoadSceneAsync(sceneName, mode);
            //��֤������ȫ���أ�false���ܳ��ּ��ز���ȫ������
            asyncOperation.allowSceneActivation = true;
            yield return asyncOperation;

            //���غõĳ�������ͨ��AsyncOperation���أ����ֶ���ȡ
            Scene scene = UnitySceneManager.GetSceneByName(sceneName);
            //����һ�����ڹ��ؽű��Ŀն���
            GameObject go = new GameObject(sceneScriptObjectName);
            SceneBehaviour sceneBehaviour = go.AddComponent<SceneBehaviour>();

            UnitySceneManager.MoveGameObjectToScene(go, scene);

            sceneBehaviour.SceneName = sceneName;
            sceneBehaviour.Init(luaName);
            Debug.Log($"{sceneName} Init Complete");
            sceneBehaviour.Load();
            SetActive(sceneName);
            //Debug.Log(sceneName);
        }

        /// <summary>
        /// ��ⳡ���Ƿ����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsSceneLoaded(string name)
        {
            Scene scene = UnitySceneManager.GetSceneByName(name);
            return scene.isLoaded;
        }
    }
}