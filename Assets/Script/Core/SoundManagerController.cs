using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
namespace Core
{

    public class SoundManagerController : SingletonObjectND<SoundManagerController>
    {
        [FoldoutGroup("共用UI基本設定")]
        public AudioSource BGMSource;
        [FoldoutGroup("共用UI基本設定")]
        public AudioSource FXSource_A;
        [FoldoutGroup("共用UI基本設定")]
        public AudioSource FXSource_B;
        [FoldoutGroup("共用UI基本設定")]
        public AudioSource OneShotSource_A;
        [FoldoutGroup("共用UI基本設定")]
        public AudioSource OneShotSource_B;


        private bool isPlayBGMRequest = false;
        private List<int> pauseLocker = new List<int>();
        public bool AllowChangeMusicVolumn = true;
        WaitForSeconds Increasetime = new WaitForSeconds(0.03f);
        WaitForSeconds Decreasetime = new WaitForSeconds(0.03f);

        [ShowInInspector]
        public bool IsPaused
        {
            get
            {
                bool val = pauseLocker.Count > 0;
                //PrintLog($"IsPaused={val}");
                return val;
            }
            //private set;
        }
        [ShowInInspector]
        public bool IsBGM_Playing
        {
            get
            {
                return BGMSource != null && BGMSource.isPlaying && !IsPaused;
            }
        }

        // ToDo: 這裡坑不少... (1.外部未透過SoundManagerController呼叫元件, 2.Uni底層Mute順序有問題, 3.MuteAll 寫法未有隙, 4.SettingManager 初始順序)
        int BGMVolumeIdx = 0;
        int FXVolumeIdx = 1;
        int OneShotVolumeIdx = 2;
        float[] volumes = new float[] { 1, 1, 1 };
        public float BGM_Volume
        {
            get
            {
                return volumes[BGMVolumeIdx];
            }
            set
            {
                volumes[BGMVolumeIdx] = value;
                BGMSource.volume = volumes[BGMVolumeIdx] * MuteAllVol;
            }
        }

        public float OneShot_Volume
        {
            get
            {
                return volumes[OneShotVolumeIdx];
            }
            set
            {
                volumes[OneShotVolumeIdx] = value;
                OneShotSource_A.volume = volumes[OneShotVolumeIdx] * MuteAllVol;
                OneShotSource_B.volume = volumes[OneShotVolumeIdx] * MuteAllVol;
            }
        }

        public float FX_Volume
        {
            get
            {
                return volumes[FXVolumeIdx];
            }
            set
            {
                volumes[FXVolumeIdx] = value;
                FXSource_A.volume = volumes[FXVolumeIdx] * MuteAllVol;
                FXSource_B.volume = volumes[FXVolumeIdx] * MuteAllVol;
            }
        }


        public void PlayBGM(AudioClip clip = null, bool isLoop = true)
        {
            if (clip != null)
                PrintLog($"BGMSource PlayBGM => {clip.name}");
            isPlayBGMRequest = true;
            BGMSource.loop = isLoop;

            if (clip != null && BGMSource.clip != clip)
            {
                BGMSource.clip = clip;
                PrintLog("BGMSource.clip reset");
            }

            if (BGMSource.clip != null && !IsPaused && !BGMSource.isPlaying)
            {
                BGMSource.Play();
                PrintLog("BGMSource.Play()");
            }
        }


        public void StopBGM()
        {
            MediaResume(gameObject.GetInstanceID());
            BGMSource.Stop();
            isPlayBGMRequest = false;
            PrintLog("BGMSource.Stop()");
        }

        public void PlayFX_A(AudioClip clip = null, bool isLoop = true)
        {

            if (clip != null)
                PrintLog($"PlayFX_A => {clip.name} IsPaused=>{IsPaused} isPlaying=>{FXSource_A.isPlaying}");

            if (clip != null && FXSource_A.clip != clip)
            {
                FXSource_A.loop = isLoop;
                FXSource_A.clip = clip;
                PrintLog("FXSource_A.clip reset");
            }

            if (FXSource_A.clip != null && !IsPaused && !FXSource_A.isPlaying)
            {
                FXSource_A.Play();
                PrintLog("FXSource_A.Play()");
            }
        }

        public void PlayFX_B(AudioClip clip = null, bool isLoop = true)
        {
            if (clip != null && FXSource_B.clip != clip)
            {
                FXSource_B.loop = isLoop;
                FXSource_B.clip = clip;
                PrintLog("FXSource_B.clip reset");
            }

            if (FXSource_B.clip != null && !IsPaused && !FXSource_B.isPlaying)
            {
                FXSource_B.Play();
                PrintLog("FXSource_B.Play()");
            }
        }

        public void StopFX()
        {
            FXSource_A.Stop();
            FXSource_B.Stop();
            PrintLog("FXSource.Stop()");
        }
        public void StopFX_A(AudioClip clip = null)
        {
            if (clip != null)
            {
                if (FXSource_A.clip != clip) return;
            }
            FXSource_A.Stop();
            PrintLog("FXSourceA.Stop()");
        }
        public void StopFX_B(AudioClip clip = null)
        {
            if (clip != null)
            {
                if (FXSource_B.clip != clip) return;
            }
            FXSource_B.Stop();
            PrintLog("FXSourceB.Stop()");
        }
        public AudioSource PlayOneShot_A(AudioClip ac)
        {
            if (OneShotSource_A.enabled && ac)
            {
                PrintLog($"PlayOneShot_A {ac.name}");
                OneShotSource_A.PlayOneShot(ac, NetUtility.SoundVolume);

                return OneShotSource_A;
            }
            return null;
        }

        public AudioSource PlayOneShot_B(AudioClip ac)
        {
            if (OneShotSource_B.enabled && ac)
            {
                PrintLog($"PlayOneShot_B {ac.name}");
                OneShotSource_B.PlayOneShot(ac, NetUtility.SoundVolume);
                return OneShotSource_B;
            }
            return null;
        }

        public void StopOneShot_A()
        {
            OneShotSource_A?.Stop();
            PrintLog("OneShotSource_A.Stop()");
        }

        public void StopOneShot_B()
        {
            OneShotSource_B?.Stop();
            PrintLog("OneShotSource_B.Stop()");
        }


        public void StopAllOneShot()
        {
            OneShotSource_A.Stop();
            OneShotSource_B.Stop();
            PrintLog("ALL　OneShotSource　Stop");
        }

        /// <summary>
        /// 聲音暫停
        /// </summary>
        /// <param name="locker">解鎖時辨識身份用的物件</param>
        public void MediaPause(int locker)
        {
            pauseLocker.Add(locker);
            PrintLog($"聲音暫停，添加聲音鎖[{locker}]");
            foreach (int obj in pauseLocker)
            {
                PrintLog($"聲音暫停 當前聲音鎖:[{obj}]");
            }
            if (pauseLocker.Count == 1)
            {
                PrintLog($"聲音暫停 僅1個聲音鎖時真正暫停音樂音效");
                _pause();
            }
        }
        private void _pause()
        {
            if (BGMSource != null)
            {

                BGMSource.Pause();
                FXSource_A.Pause();
                FXSource_B.Pause();
                OneShotSource_A.enabled = false;
                OneShotSource_B.enabled = false;


                //IsPaused = true;
                PrintLog("=========背景音樂暫停=========");
            }
        }


        // ToDo : AudioSouce 為開放所以外部非都透過 SoundManagerController 控制, 先新增 SetAudioSouceMute 整合 mute 控制部分
        public void SetMusicMute(bool mute)
        {
            BGMSource.mute = mute;
        }
        public void SetSoundMute(bool mute)
        {
            FXSource_A.mute = mute;
            FXSource_B.mute = mute;
            OneShotSource_A.mute = mute;
            OneShotSource_B.mute = mute;
        }

        float MuteAllVol = 1;
        public void MuteAll()
        {
            // BUG:  這個bug已回報給官方, audioSource 同偵設定開關 mute,vol 音效錯亂爆音
            // ----- BUG -----
            BGMSource.enabled = false;
            FXSource_A.enabled = false;
            FXSource_B.enabled = false;
            OneShotSource_A.enabled = false;
            OneShotSource_B.enabled = false;
            BGMSource.enabled = true;
            FXSource_A.enabled = true;
            FXSource_B.enabled = true;
            OneShotSource_A.enabled = true;
            OneShotSource_B.enabled = true;
            // ----- BUG -----

            PrintLog("MuteAll");
            MuteAllVol = 0;
            BGM_Volume = volumes[BGMVolumeIdx];
            FX_Volume = volumes[FXVolumeIdx];
            OneShot_Volume = volumes[OneShotVolumeIdx];
        }

        public void ResumeMute(bool isBGMReplay = false)
        {
            PrintLog("ResumeMute");
            MuteAllVol = 1;
            if (isBGMReplay) BGMSource.time = 0;
            BGM_Volume = volumes[BGMVolumeIdx];
            FX_Volume = volumes[FXVolumeIdx];
            OneShot_Volume = volumes[OneShotVolumeIdx];
        }

        /// <summary>
        /// 延時聲音回復
        /// </summary>
        /// <param name="locker"></param>
        /// <param name="foreceResume"></param>
        /// <param name="time"></param>
        public void DelayMediaResume(int locker, bool foreceResume = false, int time = 0)
        {
            Observable.Timer(System.TimeSpan.FromMilliseconds(time), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ => MediaResume(locker, foreceResume));
        }
        /// <summary>
        /// 聲音回復
        /// </summary>
        /// <param name="locker">解鎖時辨識身份用的物件</param>
        /// <param name="foreceResume">是否強制解鎖</param>
        public void MediaResume(int locker, bool foreceResume = false)//
        {
            if (foreceResume)
            {
                PrintLog($"聲音恢復，強制移除所有聲音鎖");
                pauseLocker.Clear();
            }
            else
            {
                PrintLog($"聲音恢復，移除聲音鎖[{locker}]");
                if (pauseLocker.RemoveAll(item => item == locker) <= 0)
                {
                    PrintLog($"聲音鎖恢復 - [{locker}] 不存在");
                }
            }
            if (pauseLocker.Count == 0)
            {
                PrintLog($"聲音恢復，所有聲音鎖為0時恢復音樂音效");
                _resume();
            }
            else
            {
                PrintLog($"聲音恢復，尚有聲音鎖[{pauseLocker.Count}]");
                foreach (int obj in pauseLocker)
                {
                    PrintLog($"聲音恢復，剩餘聲音鎖:[{obj}]");
                }
            }
        }

        private void _resume()
        {
            if (BGMSource != null)
            {
                BGMSource.UnPause();
                FXSource_A.UnPause();
                FXSource_B.UnPause();

                OneShotSource_A.enabled = true;
                OneShotSource_B.enabled = true;


                //IsPaused = false;
                PrintLog($"背景音樂復原步驟[{isPlayBGMRequest}][{BGMSource.isPlaying}]");
                if (isPlayBGMRequest && !BGMSource.isPlaying)
                {
                    BGMSource.Play();
                    PrintLog("=========背景音樂恢復=========");
                }

            }
        }
        #region 背景音漸層功能
        IEnumerator IEBGM;

        public void GradientBGMStop(float durtaion = 0.3f)
        {
            if (IEBGM != null)
            {
                StopCoroutine(IEBGM);
            }

            IEBGM = BGMDecrease(durtaion);
            StartCoroutine(IEBGM);
        }

        public void GradientBGMPlay(AudioClip ac, bool isLoop, float durtaion = 0.0f)
        {
            if (IEBGM != null)
            {
                StopCoroutine(IEBGM);
            }

            IEBGM = BGMIncrease(ac, isLoop, durtaion);
            StartCoroutine(IEBGM);
        }


        IEnumerator BGMIncrease(AudioClip ac, bool isLoop, float durtaion)
        {
            float times = durtaion / 0.03f;
            float _gradient = NetUtility.MusicVolume / times;

            PlayBGM(ac, isLoop);
            BGM_Volume = 0;

            for (int i = 0; i < times; i++)
            {
                BGM_Volume += _gradient;
                yield return Increasetime;
            }

            BGM_Volume = NetUtility.MusicVolume;
        }

        IEnumerator BGMDecrease(float durtaion)
        {
            float times = durtaion / 0.03f;
            float _gradient = NetUtility.MusicVolume / times;

            BGM_Volume = NetUtility.MusicVolume;
            for (int i = 0; i < times; i++)
            {
                BGM_Volume -= _gradient;
                yield return Decreasetime;
            }

            StopBGM();

            BGM_Volume = NetUtility.MusicVolume;
        }
        private void Start()
        {
            AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
        }

        private void OnAudioConfigurationChanged(bool deviceWasChanged)
        {
            PrintLog($"音效播放裝置更換[{deviceWasChanged}] => BGM[{BGMSource.isPlaying}],FXA[{FXSource_A.isPlaying}],FXB[{FXSource_B.isPlaying}]OSA[{OneShotSource_A.isPlaying}]OSB[{OneShotSource_B.isPlaying}]");
            //if (deviceWasChanged)
            //{
            //    AudioConfiguration config = AudioSettings.GetConfiguration();
            //    AudioSettings.Reset(config);
            //}
            if (!IsPaused)
            {
                MediaResume(gameObject.GetInstanceID(), true);
            }
        }
        public void StopAllSource()
        {
            isPlayBGMRequest = false;
            BGMSource.Stop();
            FXSource_A.Stop();
            FXSource_B.Stop();
            OneShotSource_A.Stop();
            OneShotSource_B.Stop();
        }
        #endregion
        public void PrintLog(string log)
        {
            CLog.PrintLog(log, "音樂管理員", "#AA00AA", LogLevel.Sound);
        }
        public void PrintLogWarning(string log)
        {
            CLog.PrintLogWarning(log, "音樂管理員", "#AA00AA", LogLevel.Sound);
        }
        public void PrintLogError(string log)
        {
            CLog.PrintLogError(log, "音樂管理員", "#AA00AA", LogLevel.Sound);
        }
    }
}