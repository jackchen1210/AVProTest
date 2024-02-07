using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public enum EGSoundKey
    {
        None = -1,
        EGLobbyBGM,
    }
    public class SoundList_EG_Share : SoundBase
    {
        [LabelText("EG共用音樂音效節點設置")]
        [DictionaryDrawerSettings(KeyLabel = "節點名稱", ValueLabel = "音源設置")]
        public Dictionary<EGSoundKey, AudioBase> _AudioList;
        protected override void Awake()
        {
            base.Awake();
            foreach (var au in _AudioList)
            {
                AudioList.Add(new AudioInfo() { name = au.Key.ToString(), audioBase = au.Value });
            }
        }
    }
}