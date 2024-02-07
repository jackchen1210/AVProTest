using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using Core;

namespace Platform
{
    /// <summary>
    /// 用來輔佐ViewPage，主要為統一管理Header按鈕功能所使用。
    /// [EN]2022.01.27
    /// </summary>
    public class SubViewPage : MonoBehaviour
    {
        [SerializeField] private Button btBack;
        [SerializeField] private Button btMore;
        [SerializeField] private Button btCancel;
        private ViewPage viewPage = null;

        public Button ButtonBack { get => btBack; }
        public Button ButtonMore { get => btMore; }
        public Button ButtonCancel { get => btCancel; }

        /// <summary>
        /// 是否開啟點擊遮擋
        /// </summary>
        public bool IsRaycast
        {
            set => viewPage.CurRaycaster.enabled = value;
        }

        /// <summary>
        /// 一次性事件
        /// </summary>
        public UnityAction OnShow
        {
            set => onShow = value;
        }
        private UnityAction onShow = null;

        /// <summary>
        /// 一次性事件
        /// </summary>
        public UnityAction OnHide
        {
            set => onHide = value;
        }
        private UnityAction onHide = null;

        void Start()
        {
            viewPage = GetComponentInParent<ViewPage>();
        }

        /// <summary>
        /// 返回
        /// </summary>
        public void Back()
        {
            if (NetUtility.ViewPageHistory.Count <= 1)
            {
                //Debug.Log($"[NetUtility.ViewPageHistory.Count] {NetUtility.ViewPageHistory.Count}");
                return;
            }

            NetUtility.ViewPageHistory.Pop(); //移除自己

            var peek = NetUtility.ViewPageHistory.Peek();

            NetUtility.SetPlatformCurrentState(peek.ViewState, peek.SubKey);

            //Debug.Log($"[NetUtility.ViewPageHistory_Pop()] {pop.ToString()}");
            //Debug.Log($"[NetUtility.ViewPageHistory_Peek()] {peek.ToString()}");
        }

        public void Show()
        {
            viewPage?.Show();

            onShow?.Invoke();
            onShow = null;
        }

        public void Hide()
        {
            viewPage?.Hide();

            onHide?.Invoke();
            onHide = null;
        }

        public void HideAll()
        {
            DialogManager.HideAll();
        }

    }
}