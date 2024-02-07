using System.Collections.Generic;
using UniRx;
using Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json.Linq;
using System;

namespace Core
{
    [RequireComponent(typeof(ViewPage))]
    public class DialogItem : MonoBehaviour
    {
        [ValueDropdown("GetDialogList", IsUniqueList = true)]
        public string DialogName;
#if UNITY_EDITOR
        private IEnumerable<ValueDropdownItem> GetDialogList()
        {
            return DialogSettings.Instance.Dialogs.ConvertAll(x => new ValueDropdownItem(x, x));
        }
#endif
        public ViewPage View { get { return vg; } }
        ViewPage vg;
        public DialogEvent<JObject> JEvent = new DialogEvent<JObject>();
        public DialogEvent<string, Action> UEvent = new DialogEvent<string, Action>();

        private bool _isOnConfirm = false; //給_onViewPageHide過濾用的判斷旗標
        private bool _isOnCancel = false;//給_onViewPageHide過濾用的判斷旗標
        protected ShowParameters_Base myParams { get; set; }

        public virtual void Awake()
        {
            vg = GetComponent<ViewPage>();
            DialogManager.RegisterItem(this);

            if (transform.localScale == Vector3.zero)
            {
                transform.localScale = Vector3.one;
            }
        }

        protected virtual void Start()
        {
            UI_Init();
            vg?.onShow?.AddListener(_onViewPageShow);
            vg?.onHide?.AddListener(_onViewPageHide);
        }

        protected virtual void OnDestroy()
        {
            DialogManager.RemoveItem(this);
            vg?.onShow?.RemoveListener(_onViewPageShow);
            vg?.onHide?.RemoveListener(_onViewPageHide);
        }


        private void _onViewPageShow()
        {
            OnViewPageShow();
        }

        private void _onViewPageHide()
        {
            if (_isOnConfirm || _isOnCancel)
            {

            }
            else
            {
                ReturnValue_Base myReturnVal = new ReturnValue_Base() { Result = DialogResult.None };
                if (myParams != null && myParams.ReplyToMeOnly != null)
                {
                    myParams.ReplyToMeOnly.Invoke(myReturnVal);
                }
            }
            _isOnConfirm = false;
            _isOnCancel = false;
            OnViewPageHide();
        }

        protected virtual void OnViewPageShow()
        {
        }

        protected virtual void OnViewPageHide()
        {
        }

        public virtual void UI_Init()
        {

        }

        public void ShowDialog<T>(T parameter) where T : ShowParameters_Base
        {
            myParams = parameter;
            ShowDialog();
        }

        public virtual void ShowDialog()
        {
            UI_Init();
            vg.Show();
        }

        public virtual void HideDialog()
        {
            vg?.Hide();
        }

        public virtual void OnConfirm()
        {
            OnConfirm(true);
        }

        public void OnConfirm(bool autoHide)
        {
            _onConfirm(new ReturnValue_Base(), autoHide);
        }

        public virtual void OnCancel()
        {
            _onCancel(new ReturnValue_Base());
        }

        public virtual void OnClose()
        {
            _onClose(new ReturnValue_Base());
        }

        protected void _onConfirm(ReturnValue_Base rtv, bool autoHide = true)
        {
            _isOnConfirm = true;
            if (autoHide)
            {
                vg.Hide();
            }
            if (rtv != null)
            {
                rtv.Result = DialogResult.Confirm;
                if (myParams != null && myParams.ReplyToMeOnly != null)
                {
                    myParams.ReplyToMeOnly?.Invoke(rtv);
                }
                else
                {
                    if (DialogManager.DialogReturnValueDictionary.ContainsKey(DialogName))
                    {
                        DialogManager.DialogReturnValueDictionary[DialogName].SetValueAndForceNotify(rtv);
                    }
                }
                myParams?.OnConfirmAction?.Invoke();
            }
        }
        protected void _onCancel(ReturnValue_Base rtv)
        {
            _isOnCancel = true;
            vg.Hide();
            if (rtv != null)
            {
                rtv.Result = DialogResult.Cancel;
                if (myParams != null && myParams.ReplyToMeOnly != null)
                {
                    myParams.ReplyToMeOnly?.Invoke(rtv);
                }
                else
                {
                    if (DialogManager.DialogReturnValueDictionary.ContainsKey(DialogName))
                    {
                        DialogManager.DialogReturnValueDictionary[DialogName].SetValueAndForceNotify(rtv);
                    }
                }
                myParams?.OnCancelAction?.Invoke();
            }
        }

        protected void _onClose(ReturnValue_Base rtv)
        {
            vg.Hide();
            if (rtv != null)
            {
                rtv.Result = DialogResult.Cancel;
                if (myParams != null && myParams.ReplyToMeOnly != null)
                {
                    myParams.ReplyToMeOnly?.Invoke(rtv);
                }
                else
                {
                    if (DialogManager.DialogReturnValueDictionary.ContainsKey(DialogName))
                    {
                        DialogManager.DialogReturnValueDictionary[DialogName].SetValueAndForceNotify(rtv);
                    }
                }
                myParams?.OnCloseAction?.Invoke();
            }
        }

        public class ShowParameters_Base
        {
            public string content = String.Empty;
            public Action OnConfirmAction = null;
            public Action OnCancelAction = null;
            public Action OnCloseAction = null;
            public virtual Action<ReturnValue_Base> ReplyToMeOnly { get; set; } = null;
        }
        public class ReturnValue_Base
        {
            public DialogResult Result = DialogResult.None;
        }
        public void PrintLog(string value, string color = "yellow")
        => CLog.PrintLog(value, this.name, color, LogLevel.StepProcess);

        public void PrintLogWarning(string value, string color = "yellow")
        => CLog.PrintLogWarning(value, this.name, color, LogLevel.StepProcess);

        public void PrintLogError(string value, string color = "yellow")
        {
            CLog.PrintLogError(value, this.name, color, LogLevel.StepProcess);
        }
    }
}
