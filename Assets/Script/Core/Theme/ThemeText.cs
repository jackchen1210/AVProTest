using Core;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

namespace Platform
{
    [Serializable]
    public class ThemeTextDictionary : SerializableDictionary<ThemeType, Color>
    { 


    }
    [DisallowMultipleComponent]
    public class ThemeText : MonoBehaviour
    {
		public ThemeTextDictionary mapColors = new ThemeTextDictionary()
        {

        };
		

        private Text text;


        private void Awake()
        {
            text = GetComponent<Text>();
            NetUtility.PLATFORM_INFO_DATA.PlatformThemeType.Subscribe(x => UpdateColor(x)).AddTo(this);
        }

        void Start()
        {
            
        }

		#if UNITY_EDITOR
        [Button("自動偵測WhiteTheme-Color")]
        private void autoWhiteThemeColorDetect()
        {
            if (mapColors != null)
            {
				text = GetComponent<Text>();
				
				if( text != null && text.color != null )
					mapColors[ThemeType.White] = text.color;
            }
        }
#endif


        public string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }



        private void UpdateColor(ThemeType iType)
        {
            if (text == null)
                return;

            Color c;
            if( !mapColors.TryGetValue( iType, out c ) )
            {
                PrintLogError($"Update the text of component [ {GetGameObjectPath(text.gameObject)} ] Theme: {iType}...failed");
                return;
            }

            text.color = c;
           // PrintLogWarning($"Update the text of component [ {text.name} ] Theme: {iType}....success");
        }
        #region Log相關
        /// <summary>
        /// 統一推送錯誤訊息
        /// </summary>
        /// <param name="log"></param>
        private void PrintLog(string log)
        {
            CLog.PrintLog(log, "ThemeImage", "#FF0000", LogLevel.Share_UI);
        }
        /// <summary>
        /// 統一推送錯誤訊息
        /// </summary>
        /// <param name="value"></param>
        private void PrintLogError(string log)
        {
            CLog.PrintLogError(log, "ThemeImage", "#FF0000", LogLevel.Share_UI);
        }
        /// <summary>
        /// 統一推送警告訊息
        /// </summary>
        /// <param name="log"></param>
        private void PrintLogWarning(string log)
        {
            CLog.PrintLogWarning(log, "ThemeImage", "#FF0000", LogLevel.Share_UI);
        }
        #endregion Log相關


    }

}