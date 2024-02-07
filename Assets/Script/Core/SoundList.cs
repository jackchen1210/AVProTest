using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public struct CustomAudio
    {
        [HorizontalGroup("group01", 0.2f)]
        [HideLabel]
        public string name;

        [HorizontalGroup("group01", 0.2f)]
        [HideLabel]
        public AudioClip Chip;

        [HorizontalGroup("group01", 0.2f)]
        [HideLabel]
        public AudioType Type;

        [HorizontalGroup("group01", 0.1f)]
        [HideLabel]
        public bool IsLoop;
        [HorizontalGroup("group01", 0.2f)]
        [HideLabel]
        public AudioPriority Priority;

    }
    public class SoundBase : SerializedMonoBehaviour
    {
        [LabelText("節點名稱")]
        public string Name;
        [HideInInspector]
        public List<AudioInfo> AudioList;
        protected virtual void OnEnable()
        {
            if (Name.Length == 0)
            {
                Name = name;
            }

            SoundController.RegisterItem(this);
        }
        protected virtual void OnDisable()
        {
            SoundController.RemoveItem(this);
        }

        protected virtual void Awake()
        {
            AudioList = new List<AudioInfo>();
        }
    }



    public class SoundList : SoundBase
    {
        [LabelText("RD環境專屬")]
        public bool IsRDTest = false;
        [LabelText("音樂音效節點設置")]
        public List<CustomAudio> _AudioList;
        protected override void Awake()
        {
            base.Awake();
            if (IsRDTest)
            {
#if !IS_QASTATION

                ImportAudio();
#endif
            }
            else
            {
                ImportAudio();
            }
        }

        private void ImportAudio()
        {
            foreach (var ac in _AudioList)
            {
                AudioList.Add(new AudioInfo()
                {
                    name = ac.name,
                    audioBase = new AudioBase()
                    {
                        Chip = ac.Chip,
                        Type = ac.Type,
                        IsLoop = ac.IsLoop,
                        Priority = ac.Priority,
                    }
                });
            }
        }
    }
}