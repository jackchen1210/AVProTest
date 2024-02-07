using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Core;
using System;

namespace Core
{
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(Canvas))]
    public class ViewPage : MonoBehaviour
    {
        public Canvas CurCanvas
        {
            get
            {
                if (_CurCanvas == null)
                    _CurCanvas = GetComponent<Canvas>();
                return _CurCanvas;
            }
        }
        public bool IsShow
        {
            get
            {
                bool result = false;
                if (CurCanvas != null)
                {
                    result = CurCanvas.enabled;
                }
                return result;
            }
            set
            {
                if (value)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }
        private Canvas _CurCanvas;
        public GraphicRaycaster CurRaycaster
        {
            get
            {
                if (_CurRaycaster == null)
                    _CurRaycaster = GetComponent<GraphicRaycaster>();
                return _CurRaycaster;
            }
        }
        private GraphicRaycaster _CurRaycaster;
        public string PageName
        {
            get
            {
                if (_PageName.Length == 0)
                    _PageName = gameObject.name;
                return _PageName;
            }
            set { _PageName = value; }
        }
        public string SceneName
        {
            get
            {
                return gameObject.scene.name;
            }
        }
        [LabelText("頁面名稱"), PropertyOrder(0)]
        public string _PageName;
        [LabelText("自動開啟"), PropertyOrder(0)]
        public bool IsAutoShow = false;

        [NonSerialized] public UnityEvent onReset = new UnityEvent();
        [FoldoutGroup("事件")] public UnityEvent onShow;
        [FoldoutGroup("事件")] public UnityEvent onHide;
        [FoldoutGroup("事件")] public UnityEvent onBack;
        [FoldoutGroup("事件")] public UnityEvent<bool> onStateChange;
        [SerializeField, LabelText("啟用位置重設")]
        private bool UseStartPos = true;
        [LabelText("起始位置"), PropertyOrder(-1)]
        public Vector3 StartPos = Vector3.zero;
        [SerializeField, LabelText("不註冊到底層")]
        public bool DontRegister = false;

        /// <summary>
        /// 開啟頁面
        /// </summary>
        public virtual void Show()
        {
            SetCanvas(true);
            onReset?.Invoke();
            onShow?.Invoke();
        }

        private void SetCanvas(bool value)
        {
            if (CurCanvas == null)
            {
                Debug.LogError($"Set ViewPage[{PageName}] canvas enable={value},but canvas=NULL");
                return;
            }
            if (CurCanvas.enabled != value)
            {
                onStateChange?.Invoke(value);
            }
            /*if (!DontRegister)
            {
                gameObject.SetActive(value);
            }*/
            CurCanvas.enabled = value;
            CurRaycaster.enabled = value;
            SmartActive(value);
            PageManager.PageChanged?.Invoke();
        }

        public virtual void Hide()
        {
            SetCanvas(false);
            //Debug.Log($"########Page [{PageName}] Hide");
            onHide?.Invoke();
        }

        public void Back()
        {
            //Debug.Log($"Page [{PageName}] Back");
            onBack?.Invoke();
        }
        private void Awake()
        {
            if (PageManager.ContainsKey(this))
            {
                PageName += "x";
            }
            if (!DontRegister)
            {
                PageManager.RegisterItem(this);
            }

            if (IsAutoShow)
            {
                Show();
            }
            else
            {
                Hide();
            }
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }
        private void Start()
        {
            if (UseStartPos)
                transform.localPosition = StartPos;
            OnStart();
        }
        protected virtual void OnStart()
        {
        }
        private void Update()
        {
            OnUpdate();
        }
        protected virtual void OnUpdate()
        {

        }
        private void OnDestroy()
        {
            if (!DontRegister)
            {
                PageManager.RemoveItem(this);
            }
            Destroy();
        }
        protected virtual void Destroy()
        {
        }

        bool? ToDoIgnore = null;
        /// <summary>
        /// 檢查是否為不可關閉 GameObject
        /// </summary>
        private void CheckIgnoreActive()
        {
            // ToggleGroup 在 GO 被關閉時沒有運作, GO 打開 ToggleGroup 卻發現子 Toogle 不合法的亂象顯示
            // ToggleGroup 搭配 Toogle ,IsOn 理想在 GO 開啟時 ex. OnEnable ... (ResetMe 有可能在物件未開啟時也能呼叫, 有風險)
            ToDoIgnore = false;
            ToDoIgnore |= GetComponentsInChildren_IgnoreActive<ToggleGroup>(transform).Count > 0;
        }
        private void SmartActive(bool value)
        {
            if (ToDoIgnore == null)
            {
                CheckIgnoreActive();
            }

            if (ToDoIgnore.Value)
            {
                return;
            }

            if (value == true)
            {
                SetActive(true);
                SetActiveInLateUpdate = null;
            }
            else
            {
                SetActiveInLateUpdate = () =>
                {
                    SetActive(false);
                };
            }
        }
        Action SetActiveInLateUpdate = null;
        private void LateUpdate()
        {
            if (SetActiveInLateUpdate != null)
            {
                SetActiveInLateUpdate.Invoke();
                SetActiveInLateUpdate = null;
            }
        }

        List<T> GetComponentsInChildren_IgnoreActive<T>(Transform target)
        {
            List<T> _result = new List<T>();
            T _get = target.GetComponent<T>();
            if (_get != null)
                _result.Add(_get);
            for (int i = 0; i < target.childCount; i++)
            {
                var _childResult = GetComponentsInChildren_IgnoreActive<T>(target.GetChild(i));
                _result.AddRange(_childResult);
            }
            return _result;
        }

        // 用移至超遠座標取代關閉節點 (如果有不希望被關閉的GO可使用)
        [Tooltip("用移至超遠座標取代關閉節點 (如果有不希望被關閉的GO可使用)")]
        [SerializeField]
        private bool USE_POS_TO_ACTIVE = false;
        private Vector3 ActivePos = new Vector3(0, 0, 0);
        private Vector3 InactivePos = new Vector3(10000, 10000, 10000);
        private void SetActive(bool value)
        {
            if (USE_POS_TO_ACTIVE)
            {
                transform.localPosition = value ? this.ActivePos : this.InactivePos;
            }
            else
            {
                gameObject.SetActive(value);
            }
        }
    }
}