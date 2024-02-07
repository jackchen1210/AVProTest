using Core;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

namespace Platform
{
    [RequireComponent(typeof(Graphic)), DisallowMultipleComponent]

    public class ThemeGraphicColor : MonoBehaviour
    {
        [SerializeField, LabelText("主題顏色群,同主題可再依狀況切換Index改變顏色")] private ThemeGraphicColorGroupCollection themeColorGroups;
#pragma warning disable CS0414
        [SerializeField, LabelText("需要給顏色群組(index)寫註解的話,請寫在這裡")] private string remark = "";
#pragma warning restore CS0414

        private Graphic m_graphic;
        private Graphic graphic
        {
            get
            {
                if (m_graphic == null)
                {
                    m_graphic = GetComponent<Graphic>();
                }
                return m_graphic;
            }
        }
        private int _colorGrpIdx = 0;
        /// <summary>
        /// 主題中還需要根據狀況切換顏色時使用的
        /// </summary>
        public void SetColorGroupIndex(int value)
        {
            if (_colorGrpIdx != value)
            {
                _colorGrpIdx = value;
                UpdateColor(NetUtility.PLATFORM_INFO_DATA.PlatformThemeType.Value);
            }
        }
#if UNITY_EDITOR
        [Button("自動偵測WhiteTheme-Color")]
        private void autoWhiteThemeColorDetect()
        {
            //if (themeColors != null)
            //{
            //    themeColors[ThemeType.White] = graphic.color;
            //}
            if (themeColorGroups != null)
            {
                var grp = themeColorGroups.GetByKey(ThemeType.White);
                if (grp.colors.Count > 0)
                {
                    grp.colors[0] = graphic.color;
                }
                else
                {
                    grp.colors.Add(graphic.color);
                }
            }
        }
        //[Button("複製舊設定")]
        //private void cloner()
        //{
        //    if (themeColors != null)
        //    {
        //        foreach (KeyValuePair<ThemeType, Color> kvp in themeColors)
        //        {
        //            var grp = themeColorGroups.GetByKey(kvp.Key);
        //            if (grp.colors.Count > 0)
        //            {
        //                grp.colors[0] = kvp.Value;
        //            }
        //            else
        //            {
        //                grp.colors.Add(kvp.Value);
        //            }                    
        //        }
        //        UnityEditor.EditorUtility.SetDirty(gameObject);
        //    }
        //}
#endif

        private void Awake()
        {
            NetUtility.PLATFORM_INFO_DATA.PlatformThemeType.Subscribe(x => UpdateColor(x)).AddTo(this);
        }
        private void UpdateColor(ThemeType iType)
        {
            List<Color> ca = new List<Color>();
            if (!themeColorGroups?.TryGetValue(iType, out ca) ?? false)
            {
                if (themeColorGroups?.groups.Exists(a => a.type == iType) ?? false)//沒定義忽略ErrorLog
                {
                    PrintLogError($"Update the color of component [ {gameObject.name} ] Theme: {iType}...failed");
                }
                return;
            }
            if (ca.Count > 0)
            {
                int tmpIdx = (ca.Count > _colorGrpIdx) ? _colorGrpIdx : 0;
                graphic.color = ca[tmpIdx];
                return;
            }
        }
        private void PrintLog(string log)
        {
            CLog.PrintLog(log, "ThemeImage", "#FF0000", LogLevel.Share_UI);
        }
        private void PrintLogError(string log)
        {
            CLog.PrintLogError(log, "ThemeImage", "#FF0000", LogLevel.Share_UI);
        }
        private void PrintLogWarning(string log)
        {
            CLog.PrintLogWarning(log, "ThemeImage", "#FF0000", LogLevel.Share_UI);
        }

        //private void OnValidate()
        //{
        //    cloner();
        //}
    }
    [Serializable]
    class ThemeGraphicColorGroupCollection
    {
        public List<ThemeGraphicColorGroup> groups = new List<ThemeGraphicColorGroup>();
        public ThemeGraphicColorGroup GetByKey(ThemeType key)
        {
            ThemeGraphicColorGroup grp;
            int index = groups.FindIndex(a => a.type == key);
            if(index>=0)
            {
                grp = groups[index];
            }
            else
            {
                grp = new ThemeGraphicColorGroup(key);
                groups.Add(grp);
            }
            return grp;
        }
        public bool TryGetValue(ThemeType key, out List<Color> colors)
        {
            int index = groups.FindIndex(a => a.type == key);
            if (index >= 0)
            {
                colors = groups[index].colors;
            }
            else
            {
                colors = null;
            }
            return index >= 0;
        }
        public void Clear() => groups.Clear();        
    }

    [Serializable]
    class ThemeGraphicColorGroup
    {
        public ThemeGraphicColorGroup()
        {

        }
        public ThemeGraphicColorGroup(ThemeType type, List<Color> colors = null)
        {
            this.type = type;
            if (colors != null)
            {
                this.colors = colors;
            }
        }
        public ThemeType type;
        public List<Color> colors = new List<Color>();
    }
}