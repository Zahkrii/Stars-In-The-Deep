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
        /// 场景切换的回调
        /// </summary>
        /// <param name="scene0"></param>
        /// <param name="scene1"></param>
        private void OnActiveSceneChanged(Scene scene0, Scene scene1)
        {
            //从scene0 切换到 scene1

            // 保证所有场景已加载
            if (!scene0.isLoaded || !scene1.isLoaded)
                return;

            SceneBehaviour sceneBehaviour0 = GetSceneBehaviour(scene0);
            SceneBehaviour sceneBehaviour1 = GetSceneBehaviour(scene1);

            sceneBehaviour0?.Inactive();
            sceneBehaviour1?.OnActive();
        }

        /// <summary>
        /// 激活场景
        /// </summary>
        /// <param name="name"></param>
        public void SetActive(string name)
        {
            Scene scene = UnitySceneManager.GetSceneByName(name);
            UnitySceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// 叠加模式加载场景，原场景不删除
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
        /// 卸载场景
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
        /// 查找场景中的SceneScripts对象
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
        /// 单个模式加载场景，只保留一个场景
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
            //保证场景完全加载，false可能出现加载不完全的问题
            asyncOperation.allowSceneActivation = true;
            yield return asyncOperation;

            //加载好的场景不会通过AsyncOperation返回，得手动获取
            Scene scene = UnitySceneManager.GetSceneByName(sceneName);
            //定义一个用于挂载脚本的空对象
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
        /// 检测场景是否加载
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