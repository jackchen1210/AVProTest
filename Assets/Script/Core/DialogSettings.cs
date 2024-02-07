using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;

namespace Core
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "DialogSettings", menuName = "DialogSettings", order = 2)]

    public class DialogSettings : SerializedScriptableObject
    {
        private static string AssetPath { get { return Path.Combine("Assets/Setting/", "DialogSettings.asset"); } }

        private static DialogSettings s_instance;

        public static DialogSettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;

                s_instance = UnityEditor.AssetDatabase.LoadAssetAtPath<DialogSettings>(AssetPath);
                s_instance.Dialogs.Sort();

                if (s_instance != null) return s_instance;

                s_instance = CreateInstance<DialogSettings>();
                UnityEditor.AssetDatabase.CreateAsset(s_instance, AssetPath);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.EditorUtility.SetDirty(s_instance);

                return s_instance;
            }
        }

        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
        [LabelText("Dialog List")]
        public List<string> Dialogs = new List<string>()
        {
        };
    }
#endif
}