using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
namespace Core
{

#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "GlobalStateSetting", menuName = "GlobalStateSetting", order = 1)]
    public class GlobalStateSetting : SerializedScriptableObject
    {
        private static string AssetPath { get { return Path.Combine("Assets/Setting/", "GlobalStateSetting.asset"); } }

        private static GlobalStateSetting s_instance;

        public static GlobalStateSetting Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = UnityEditor.AssetDatabase.LoadAssetAtPath<GlobalStateSetting>(AssetPath);
                if (s_instance != null) return s_instance;
                s_instance = CreateInstance<GlobalStateSetting>();
                UnityEditor.AssetDatabase.CreateAsset(s_instance, AssetPath);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.EditorUtility.SetDirty(s_instance);
                return s_instance;
            }
        }

        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
        [LabelText("主狀態")]
        public Dictionary<string, List<string>> GlobalState = new Dictionary<string, List<string>>()
        {
        };
    }
#endif
}