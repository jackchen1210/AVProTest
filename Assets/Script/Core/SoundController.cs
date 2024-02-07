using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public enum AudioType
    {
        ONESHOT_A,
        ONESHOT_B,
        FX_A,
        FX_B,
        BGM,
    }
    public enum AudioPriority
    {
        Low,
        Medium,
        High
    }
    [TableList(ShowIndexLabels = true)]
    public class AudioInfo
    {
        [HorizontalGroup("group01", 0.3f)]
        [HideLabel]
        public string name;

        [HorizontalGroup("group01", 0.7f)]
        [HideLabel]
        public AudioBase audioBase;
    }
    [InlineProperty]
    public struct AudioBase
    {
        [HorizontalGroup("group01", 0.3f)]
        [HideLabel]
        public AudioClip Chip;

        [HorizontalGroup("group01", 0.3f)]
        [HideLabel]
        public AudioType Type;

        [HorizontalGroup("group01", 0.1f)]
        [HideLabel]
        public bool IsLoop;
        [HorizontalGroup("group01", 0.1f)]
        [HideLabel]
        public AudioPriority Priority;

    }
    public class SoundController : Singleton<SoundController>
    {
        public static Dictionary<string, SoundBase> SoundList = new Dictionary<string, SoundBase>();

        public string CurBGM;
        public static void RegisterItem(SoundBase item)
        {
            if (SoundList.ContainsKey(item.Name))
            {
                Instance.PrintLogError($"添加 SoundList[{item.Name}] 失敗，已經有相同名稱");
            }
            else
            {
                SoundList.Add(item.Name, item);
            }
            Instance.PrintLogWarning($"添加 SoundList[{item.Name}] 成功");
        }
        public static void RemoveItem(SoundBase item)
        {
            if (SoundList.ContainsKey(item.Name))
            {
                Instance.PrintLogWarning($"移除 SoundList[{item.Name}]");
                SoundList.Remove(item.Name);
            }
            else
            {
                Instance.PrintLogError($"SoundList[{item.Name}]不存在，無法移除");
            }
        }
        /// <summary>
        /// 取得AudioClip
        /// </summary>
        /// <param name="key"> AudioClip 在SoundController Inspector 的KEY </param>
        /// <returns></returns>
        public AudioClip GetAudio(string key, string subkey)
        {
            if (SoundList.ContainsKey(key))
            {
                //找出所有相同
                var al = SoundList[key].AudioList.FindAll(x => x.name == subkey);
                //排序
                var all = al.OrderByDescending(x => x.audioBase.Priority);
                return all.FirstOrDefault()?.audioBase.Chip;
            }
            return null;
        }
        public AudioClip GetAudio(string key)
        {
            List<AudioInfo> ai = new List<AudioInfo>();
            foreach (var kv in SoundList)
            {
                var al = kv.Value.AudioList.FindAll(x => x.name == key);
                if (al != null)
                {
                    ai.AddRange(al);
                }
            }

            //排序
            var all = ai.OrderByDescending(x => x.audioBase.Priority);
            return all.FirstOrDefault()?.audioBase.Chip;
        }
        public AudioInfo GetAudioInfo(string key)
        {
            List<AudioInfo> ai = new List<AudioInfo>();
            foreach (var kv in SoundList)
            {
                var al = kv.Value.AudioList.FindAll(x => x.name == key);
                if (al != null)
                {
                    ai.AddRange(al);
                }
            }

            //排序
            var all = ai.OrderByDescending(x => x.audioBase.Priority);
            return all.FirstOrDefault();
        }
        public AudioInfo GetAudioInfo(string key, string subkey)
        {
            if (SoundList.ContainsKey(key))
            {
                //找出所有相同
                var al = SoundList[key].AudioList.FindAll(x => x.name == subkey);
                //排序
                var all = al.OrderByDescending(x => x.audioBase.Priority);
                return all.FirstOrDefault();
            }
            return null;
        }
        /// <summary>
        /// 在BGM的AudioSource播放音樂,會重複撥放
        /// </summary>
        /// <param name="name">AudioClip 在SoundController Inspector 的KEY</param>
        public void PlayBackGroundAudio(string name, bool isLoop = true)
        {
            //Debug.LogError($"PlayBackGroundAudio {name}");
            CurBGM = name;
            var ac = GetAudio(name);

            if (ac)
            {
                if ((SoundManagerController.Instance.BGMSource.clip != ac) || (SoundManagerController.Instance.BGMSource.clip != null && !SoundManagerController.Instance.BGMSource.isPlaying))
                {
                    SoundManagerController.Instance.GradientBGMPlay(ac, isLoop);
                    PrintLog($"[SoundController] PlayBackGroundAudio{ac.name}");
                }
            }
            else
            {
                SoundManagerController.Instance.GradientBGMStop();

                PrintLog($"[SoundController] Failed to PlayBackGroundAudio , Not Found {name}");
            }
        }
        public virtual void PlayBackGroundAudio(string key, string name, bool isLoop = true)
        {
            //Debug.LogError($"PlayBackGroundAudio {name}");
            CurBGM = name;
            var ac = GetAudio(key, name);
            if (ac)
            {
                SoundManagerController.Instance.GradientBGMPlay(ac, isLoop);
                PrintLog($"[SoundController] PlayBackGroundAudio{ac.name}");
            }
            else
            {
                SoundManagerController.Instance.GradientBGMStop();
            }
        }

        /// <summary>
        /// 在FX的AudioSource播放音樂,會重複撥放
        /// </summary>
        /// <param name="name">AudioClip 在SoundController Inspector 的KEY</param>
        public virtual void PlayFX(string name)
        {
            //Debug.LogError($"PlayFX {name}");
            var ac = GetAudio(name);
            if (ac)
                SoundManagerController.Instance.PlayFX_A(ac);
        }
        public virtual void PlayFX(string key, string name)
        {
            //Debug.LogError($"PlayFX {name}");
            var ac = GetAudio(key, name);
            if (ac)
                SoundManagerController.Instance.PlayFX_A(ac);
        }

        /// <summary>
        /// 在OneShot的AudioSource播放音樂,只撥放一次
        /// </summary>
        /// <param name="name">AudioClip 在SoundController Inspector 的KEY</param>
        public virtual void PlayOneShot(string name)
        {
            //Debug.LogError($"PlayOneShot<color=red>{name}</color>");
            var ac = GetAudio(name);
            if (ac)
            {
                SoundManagerController.Instance.PlayOneShot_A(ac);
                PrintLog($"[SoundController] PlayOneShot{ac.name}");
            }
        }
        public virtual void PlayOneShot(string key, string name)
        {
            //Debug.LogError($"PlayOneShot<color=red>{name}</color>");
            var ac = GetAudio(key, name);
            if (ac)
            {
                SoundManagerController.Instance.PlayOneShot_A(ac);

                PrintLog($"[SoundController] PlayOneShot{ac.name}");
            }
        }


        public void Play(string name)
        {
            var info = GetAudioInfo(name);
            if (info != null)
            {
                switch (info.audioBase.Type)
                {
                    case AudioType.BGM:
                        PlayBackGroundAudio(name, info.audioBase.IsLoop);
                        break;
                    case AudioType.FX_A:
                        SoundManagerController.Instance.PlayFX_A(info.audioBase.Chip, info.audioBase.IsLoop);
                        break;
                    case AudioType.FX_B:
                        SoundManagerController.Instance.PlayFX_B(info.audioBase.Chip, info.audioBase.IsLoop);
                        break;
                    case AudioType.ONESHOT_A:
                        SoundManagerController.Instance.PlayOneShot_A(info.audioBase.Chip);
                        break;
                    case AudioType.ONESHOT_B:
                        SoundManagerController.Instance.PlayOneShot_B(info.audioBase.Chip);
                        break;
                }
            }
        }
        public void Play(string key, string name)
        {
            var info = GetAudioInfo(key, name);
            if (info != null)
            {
                switch (info.audioBase.Type)
                {
                    case AudioType.BGM:
                        PlayBackGroundAudio(name, info.audioBase.IsLoop);
                        break;
                    case AudioType.FX_A:
                        SoundManagerController.Instance.PlayFX_A(info.audioBase.Chip);
                        break;
                    case AudioType.FX_B:
                        SoundManagerController.Instance.PlayFX_B(info.audioBase.Chip);
                        break;
                    case AudioType.ONESHOT_A:
                        SoundManagerController.Instance.PlayOneShot_A(info.audioBase.Chip);
                        break;
                    case AudioType.ONESHOT_B:
                        SoundManagerController.Instance.PlayOneShot_B(info.audioBase.Chip);
                        break;
                }
            }
        }

        public void Play<T>(T enumName) where T : System.Enum
        {
            Play(System.Enum.GetName(typeof(T), enumName));
        }
        public void Stop(string name)
        {
            var info = GetAudioInfo(name);
            if (info != null)
            {
                switch (info.audioBase.Type)
                {
                    case AudioType.BGM:
                        StopBackGroundAudio();
                        break;
                    case AudioType.FX_A:
                        SoundManagerController.Instance.StopFX_A(info.audioBase.Chip);
                        break;
                    case AudioType.FX_B:
                        SoundManagerController.Instance.StopFX_B(info.audioBase.Chip);
                        break;
                    case AudioType.ONESHOT_A:
                        SoundManagerController.Instance.StopOneShot_A();
                        break;
                    case AudioType.ONESHOT_B:
                        SoundManagerController.Instance.StopOneShot_B();
                        break;
                }
            }
        }
        public void Stop(string key, string name)
        {
            var info = GetAudioInfo(key, name);
            if (info != null)
            {
                switch (info.audioBase.Type)
                {
                    case AudioType.BGM:
                        StopBackGroundAudio();
                        break;
                    case AudioType.FX_A:
                        SoundManagerController.Instance.StopFX_A(info.audioBase.Chip);
                        break;
                    case AudioType.FX_B:
                        SoundManagerController.Instance.StopFX_B(info.audioBase.Chip);
                        break;
                    case AudioType.ONESHOT_A:
                        SoundManagerController.Instance.StopOneShot_A();
                        break;
                    case AudioType.ONESHOT_B:
                        SoundManagerController.Instance.StopOneShot_B();
                        break;
                }
            }
        }

        public void Stop<T>(T enumName) where T : System.Enum
        {
            Stop(System.Enum.GetName(typeof(T), enumName));
        }

        /// <summary> 停止BGM的AudioSource播放的音樂 </summary>
        public virtual void StopBackGroundAudio()
        {
            CurBGM = "";

            if (SoundManagerController.Instance != null)
                SoundManagerController.Instance.StopBGM();
        }

        /// <summary> 背景音樂淡出 </summary>
        /// <param name="fadeTime"> 淡出時間 </param>
        public void FadeOutBackGroundAudio(float fadeTime = 1.2f)
        {
            SoundManagerController.Instance.GradientBGMStop(fadeTime);
        }

        /// <summary>
        /// 停止FX的AudioSource播放的音樂
        /// </summary>
        public virtual void StopFX()
        {
            SoundManagerController.Instance.StopFX();
        }

        /// <summary>
        /// 停止所有OneShot的AudioSource播放的音樂
        /// </summary>
        public virtual void StopOneShot_All()
        {
            SoundManagerController.Instance.StopAllOneShot();
        }

        /// <summary>
        /// 停止StopOneShot_B的AudioSource播放的音樂
        /// </summary>
        public virtual void StopOneShot_B()
        {
            SoundManagerController.Instance.StopOneShot_B();
        }

        /// <summary>
        /// 停止StopOneShot_A的AudioSource播放的音樂
        /// </summary>
        public virtual void StopOneShot_A()
        {
            SoundManagerController.Instance.StopOneShot_A();
        }


        /**********************************************************************/

        #region Log功能
        public void PrintLog(string log)
        {
            CLog.PrintLog(log, "音樂控制器", "#AA00AA", LogLevel.Sound);
        }
        public void PrintLogWarning(string log)
        {
            CLog.PrintLogWarning(log, "音樂控制器", "#AA00AA", LogLevel.Sound);
        }
        public void PrintLogError(string log)
        {
            CLog.PrintLogError(log, "音樂控制器", "#AA00AA", LogLevel.Sound);
        }
        #endregion

        /**********************************************************************/
    }
}