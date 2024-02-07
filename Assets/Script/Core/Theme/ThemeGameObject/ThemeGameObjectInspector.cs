using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Platform
{
    public partial class ThemeGameObject : MonoBehaviour
    {

        private static readonly ThemeType defaultTheme = ThemeType.White;
        [EnumToggleButtons, HideLabel, OnValueChanged(nameof(ResetThemeChange)), SerializeField]
        private ThemeType themeState = defaultTheme;
        [FoldoutGroup("子物件創建工具"), HorizontalGroup("子物件創建工具/子物件屬性"), LabelText("狀態"), LabelWidth(50), HideInPlayMode, SerializeField]
        private ThemeType createThemeType = defaultTheme;
        [FoldoutGroup("子物件創建工具"), Button("建立狀態子物件"), HideInPlayMode]
        private void CreateButton()
        {
            CreateSub(createThemeType);
        }
        [LabelText("子物件清單"), HideInPlayMode, SerializeField]
        private List<GameObject> subItems = new List<GameObject>();
        [Button("刷新子物件")]
        void ResetSubItem()
        {
            subItems.Clear();
            var themes = Enum.GetValues(typeof(ThemeType));
            foreach (ThemeType theme in themes)
            {
                subItems.Add(transform.Find(GetSubItemNameByType(theme)).gameObject);
            }

            ResetSubItemActive();
            Debug.Log("刷新子物件完成");
        }

        private void ResetSubItemActive()
        {
            if (subItems.Count == 0)
            {
                Debug.LogError("子物件清單為空");
                return;
            }

            foreach (var item in subItems)
            {
                if (string.Equals(item.name, GetSubItemNameByType(themeState)))
                {
                    item.SetActive(true);
                }
                else
                {
                    item.SetActive(false);
                }
            }
        }

        private void CreateSub(ThemeType themeType)
        {
            if (transform.Find(GetSubItemNameByType(themeType)) != null)
            {
                Debug.LogError($"子物件已存在 : {themeType} 不建立");
                return;
            }

            //建立子物件
            var obj = new GameObject(GetSubItemNameByType(themeType));
            obj.transform.SetParent(gameObject.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.SetActive(themeType == themeState);
            Debug.Log($"子物件 : {themeType} 建立完成");
        }

        private static string GetSubItemNameByType(ThemeType themeType)
        {
            return themeType.ToString();
        }

        private void ResetThemeChange()
        {
            ResetSubItemActive();
        }

        [FoldoutGroup("子物件創建工具"), Button("建立所有狀態子物件"), HideInPlayMode]
        void CreateAllButton()
        {
            var themes = Enum.GetValues(typeof(ThemeType));
            foreach (ThemeType theme in themes)
            {
                CreateSub(theme);
            }
            Debug.Log("建立所有狀態子物件完成");
        }
    }

}
