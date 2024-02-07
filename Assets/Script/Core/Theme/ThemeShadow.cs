using Core;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

namespace Platform
{
    [RequireComponent(typeof(Shadow)), DisallowMultipleComponent]

    public class ThemeShadow : MonoBehaviour
    {
        [SerializeField, LabelText("主題顏色群,同主題可再依狀況切換Index改變顏色")] private ThemeGraphicColorGroupCollection themeColorGroups;
#pragma warning disable CS0414
        [SerializeField, LabelText("需要給顏色群組(index)寫註解的話,請寫在這裡")] private string remark = "";
#pragma warning restore CS0414
        private Shadow m_shadow;
        private Shadow shadow
        {
            get
            {
                if (m_shadow == null)
                {
                    m_shadow = GetComponent<Shadow>();
                }
                return m_shadow;
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
            if (themeColorGroups != null)
            {
                var grp = themeColorGroups.GetByKey(ThemeType.White);
                if (grp.colors.Count > 0)
                {
                    grp.colors[0] = shadow.effectColor;
                }
                else
                {
                    grp.colors.Add(shadow.effectColor);
                }
            }
        }
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
                shadow.effectColor = ca[tmpIdx];
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
    }
}