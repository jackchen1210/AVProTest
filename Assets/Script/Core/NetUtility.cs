using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SocketData
    {
        public string proxyName;
        public int MID;
        public byte[] data;
        public DateTime time;

        public SocketData(string _proxyName, int _mid, byte[] _data)
        {
            proxyName = _proxyName;
            MID = _mid;
            data = _data;
            time = DateTime.Now;
        }
    }

    public enum GamePlatformID
    {
        EGLOBBY = 0,
        RPLOBBY = 1,
        CBLOBBY = 2,
        PELOBBY = 3
    }


    public enum GameListChangeType
    {
        DynamicAddTable = -3,
        SimpleTableInfo = -2,
        None = -1
    }

    public class GameSockData
    {
        public int MID;
        public byte[] data;

        public GameSockData(int _mid, byte[] _data)
        {
            MID = _mid;
            data = _data;
        }
    }


    public class NetUtility : Singleton<NetUtility>
    {
        public static float MaxDirectionRatio = 21f / 9f;  //美術極限比例 (21:9)
        public static float MaxDirectionRatio_H = 1;  //美術極限水平 (1:1)

        public static Action CameraViewportChanged;
        public static Action CameraViewportChangedLate; //CameraViewportChanged 後2偵的事件
        public static Action<float, float> referenceResolutionChanged;

        // 本地音效(免費及正式都可用)
        public static float SoundVolume = 1;
        public static float MusicVolume = 1;
        public static bool MusicIsOn = true;
        public static bool SoundIsOn = true;
        public static PlatformInfoData PLATFORM_INFO_DATA = new PlatformInfoData();
        public static Stack<PlatformStateData> ViewPageHistory = new Stack<PlatformStateData>();


        /// <summary> 電子館連線狀態通知 </summary>
        public static event Action<NetConnectState, EGGamePlayState> EGGame_ReconnectStateEvent;


        public static void PrintLogError(string value, string title = "NetUtility", string color = "white", LogLevel logLevel = LogLevel.Normal)
        {
            CLog.PrintLogError(value, title, color, logLevel);
        }
        /// <summary> 凍結畫面 </summary>
        public static void FreezeUpdate(bool isFreeze)
        {
            if (isFreeze)
            {
                Time.timeScale = 0f;
                Debug.Log("畫面凍結");
            }
            else
            {
                Time.timeScale = 1f;
                Debug.Log("解除畫面凍結");
            }
        }

        internal static string GetStringByKey(string v)
        {
            return v;
        }

        public static void SetPlatformCurrentState(string viewState, string subKey)
        {
            //PrintLog($"設定<color=red>[{viewState}][{subKey}]</color>", "主流程設定", "#00FF00");
            //PlatformCurrentState.SetValueAndForceNotify(PlatformCurrentState.Value.SetValue(viewState, subKey));
            //NavigationListenPlatformCurrentState.SetValueAndForceNotify(PlatformCurrentState.Value);
            //PlatformLastState.SetValueAndForceNotify(PlatformLastState.Value.SetValue(viewState, subKey));
        }
    }
}