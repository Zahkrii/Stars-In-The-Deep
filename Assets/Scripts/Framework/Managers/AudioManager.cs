using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Managers
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource soundSource;
        private AudioSource musicSource;

        //音效音量
        private float SoundVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("SoundVolume", 1.0f);
            }
            set
            {
                soundSource.volume = value;
                PlayerPrefs.SetFloat("SoundVolume", value);
            }
        }

        //音乐音量
        private float MusicVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            }
            set
            {
                musicSource.volume = value;
                PlayerPrefs.SetFloat("MusicVolume", value);
            }
        }

        private void Awake()
        {
            //初始化
            musicSource = this.gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;

            soundSource = this.gameObject.AddComponent<AudioSource>();
            soundSource.loop = false;
        }

        #region 播放音乐

        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="name"></param>
        public void PlayMusic(string name, string extension)
        {
            //如果玩家设置音量低于0.1f，则完全不播放，节省性能
            if (this.MusicVolume < 0.1f)
                return;

            string previousName = "";
            if (musicSource.clip != null)
                previousName = musicSource.clip.name;

            //相同音乐不重复加载资源
            if (previousName == name)
            {
                musicSource.Play();
                return;
            }

            Manager.ResourceManager.LoadAsset(name, extension, AssetType.Music, (UnityEngine.Object obj) =>
            {
                musicSource.clip = obj as AudioClip;
                musicSource.Play();
            });
        }

        /// <summary>
        /// 暂停音乐
        /// </summary>
        public void PauseMusic()
        {
            musicSource.Pause();
        }

        /// <summary>
        /// 继续播放音乐
        /// </summary>
        public void UnPauseMusic()
        {
            musicSource.UnPause();
        }

        /// <summary>
        /// 停止播放音乐
        /// </summary>
        public void StopMusic()
        {
            musicSource.Stop();
        }

        #endregion 播放音乐

        #region 播放音效

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="name"></param>
        public void PlaySound(string name, string extension)
        {
            //如果玩家设置音量低于0.1f，则完全不播放，节省性能
            if (this.SoundVolume < 0.1f)
                return;

            Manager.ResourceManager.LoadAsset(name, extension, AssetType.Sound, (UnityEngine.Object obj) =>
            {
                soundSource.PlayOneShot(obj as AudioClip);
            });
        }

        #endregion 播放音效

        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="value"></param>
        public void SetMusicVolume(float value)
        {
            this.MusicVolume = value;
        }

        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="value"></param>
        public void SetSoundVolume(float value)
        {
            this.SoundVolume = value;
        }
    }
}