using System;
using Core;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEditor;
using System.IO;

namespace Platform
{
    [Serializable]
    public class ThemeDictionary : SerializableDictionary<ThemeType, Sprite>
    { 


    }


    [DisallowMultipleComponent]
    public class ThemeImage : MonoBehaviour
    {
        public ThemeDictionary mapSprites = new ThemeDictionary()
        {
            [ThemeType.White] = null,
            [ThemeType.Black] = null,
        };

        private Image img;
#if UNITY_EDITOR

        private void OnValidate()
        {
            var comps = GetComponents<ThemeImage>();
            if(comps!=null && comps.Length > 1)
            {
                Debug.LogWarning($"在 {GetFullName2(gameObject)} 檢查到多個ThemeImage，請修正");
            }
        }
        string GetFullName2(GameObject go)
        {
            string name = go.name;
            while (go.transform.parent != null)
            {

                go = go.transform.parent.gameObject;
                name = go.name + "/" + name;
            }
            return name;
        }
#endif

        private void Awake()
        {
            img = GetComponent<Image>();
            NetUtility.PLATFORM_INFO_DATA.PlatformThemeType.Subscribe(x => UpdateSprite(x)).AddTo(this);
        }

        void Start()
        {
            
        }
		
		#if UNITY_EDITOR
        [Button("自動偵測WhiteTheme-Sprite")]
        private void autoWhiteThemeColorDetect()
        {
            if (mapSprites != null)
            {
				img = GetComponent<Image>();
				
				if( img != null && img.sprite != null )
				{
					mapSprites[ThemeType.White] = img.sprite;
				}
                
            }
        }
		#endif

        private string GetFullName( GameObject obj )
        {
            string strParentName = "";

            if( obj != null && obj.transform != null && obj.transform.parent != null )
            {
                strParentName = obj.transform.parent.name;
                string strParentName2 = GetFullName(obj.transform.parent.gameObject);

				return $"{strParentName2} / {strParentName}";
            }
            else
            {
				return obj.name;
            }

            //string str = $"{strParentName}  {strName}";
            //return str;
        }

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



        private void UpdateSprite(ThemeType iType)
        {
            if (img == null)
            {
                PrintLogWarning($"Update the image of component Theme: { iType}... Image is null");
                return;
            }

            Sprite c = null;
            if (!mapSprites.TryGetValue(iType, out c))
            {
                if (mapSprites?.ContainsKey(iType) ?? false)//沒定義忽略ErrorLog
                {
                    PrintLogError($"Update the image of component [ { GetGameObjectPath(img.gameObject) } ] Theme: {iType}....failed");
                }
                return;
            }
            /* if( img.name != null )
                 PrintLogWarning($"Update the image of component [ { GetGameObjectPath(img.gameObject) } ] Theme: {iType}....success");*/
            img.sprite = c;
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