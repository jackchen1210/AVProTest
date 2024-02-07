using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine.Events;
using UniRx;
using static Core.DialogItem;

namespace Core
{
    public class DialogManager : Singleton<DialogManager>
    {
        public static bool IsActive { get { return PageList.Count > 0; } }

        public static string CurrentPageName { get; private set; } = string.Empty;

        private static Dictionary<string, DialogItem> PageList = new Dictionary<string, DialogItem>();

        public static event Action<string> ShowDialogEvent;
        public static event Action<string> HideDialogEvent;

        public static event Action<string, bool> ToggleRegisterItemEvent;

        //public static Dictionary<string, DialogItem.ShowParameters_Base> DialogShowParameters = new Dictionary<string, DialogItem.ShowParameters_Base>();
        public static Dictionary<string, ReactiveProperty<DialogItem.ReturnValue_Base>> DialogReturnValueDictionary = new Dictionary<string, ReactiveProperty<DialogItem.ReturnValue_Base>>();

        public static void HideCustomDialog()
        {
            HideDialog("Dia_ForCustom");
        }
        public static void ShowDialog(string PageName, DialogItem.ShowParameters_Base param)
        {
            //if (param != null)
            //{
            //DialogShowParameters[PageName] = param;
            //}
            //ShowDialog(PageName);
            PrintLog($"ShowDialog =>[{PageName}]");
            //廣播
            if (PageList.ContainsKey(PageName))
            {
                CurrentPageName = PageName;
                PageList[PageName].ShowDialog(param);
                ShowDialogEvent?.Invoke(PageName);
            }
            else
            {
                Debug.LogError($"找不到Dialog : {PageName}");
            }
        }
        /// <summary>
        /// 打開顯示並傳入AC參數
        /// </summary>
        /// <param name="PageName"></param>
        /// <param name="ActName"></param>
        /// <param name="ac"></param>
        public static void ShowDialog(string PageName, string ActName, Action ac)
        {
            SetDialogUEvent(PageName, ActName, ac);
            ShowDialog(PageName);
        }
        /// <summary>
        /// 打開顯示並傳入JEvent
        /// </summary>
        /// <param name="PageName"></param>
        /// <param name="ActName"></param>
        /// <param name="ac"></param>
        public static void ShowDialog(string PageName, JObject obj)
        {
            SetDialogJEvent(PageName, obj);
            ShowDialog(PageName);
        }
        public static void ShowDialog(string PageName)
        {
            //廣播
            if (PageList.ContainsKey(PageName))
            {
                /*ShowParameters_Base b = PageList[PageName].GetParams();

                if( b != null && b.content != string.Empty )
                {
                    // 印出部分內容, 方便Debug
                    string strContent = b.content.Length > 10 ? b.content: b.content.Substring(0,10);
                    PrintLog($"ShowDialog =>[{PageName}], [{strContent}]");
                }
                else
                {
                    PrintLog($"ShowDialog =>[{PageName}]");
                }
                */

                PrintLog($"ShowDialog =>[{PageName}]");
                CurrentPageName = PageName;
                PageList[PageName].ShowDialog();
                ShowDialogEvent?.Invoke(PageName);
            }
            else
            {
                PrintLog($"ShowDialog =>[{PageName}]");
                Debug.LogError($"找不到Dialog : {PageName}");
            }
        }
        public static void SetDialogJEvent(string PageName, JObject obj)
        {
            PrintLog($"SetDialogJEvent =>[{PageName}]");
            //廣播
            if (PageList.ContainsKey(PageName))
            {
                PageList[PageName].JEvent.Emit(obj);
            }
        }
        public static void SetDialogUEvent(string PageName, string ActName, Action ac)
        {
            PrintLog($"SetDialogUEvent =>[{PageName}]");
            //廣播
            if (PageList.ContainsKey(PageName))
            {
                PageList[PageName].UEvent.Emit(ActName, ac);
            }
        }
        public static DialogItem GetDialog(string PageName)
        {
            PrintLog($"GetDialog =>[{PageName}]");
            //廣播
            if (PageList.ContainsKey(PageName))
            {
                return PageList[PageName];
            }
            return null;
        }
        public static GameObject GetDialogGameObject(string PageName)
        {
            PrintLog($"GetDialogGameObject =>[{PageName}]");
            //廣播
            if (PageList.ContainsKey(PageName))
            {
                return PageList[PageName].gameObject;
            }
            return null;
        }
        public static void HideAll()
        {
            foreach (var li in PageList)
            {
                HideDialog(li.Key);
            }
        }
        public static void HideAllByKey(string Key)
        {
            foreach (var li in PageList)
            {
                if (li.Key.Contains(Key))
                    HideDialog(li.Key);
            }
        }
        public static void HideDialog(string PageName)
        {
            if (PageList.ContainsKey(PageName))
            {
                PrintLog($"HideDialog =>[{PageName}]");
                PageList[PageName].HideDialog();
                HideDialogEvent?.Invoke(PageName);
            }
        }
        public static bool ContainsKey(DialogItem item)
        {
            bool result = false;
            if (PageList.ContainsKey(item.DialogName))
            {
                result = true;
            }
            return result;
        }
        public static void RegisterItem(DialogItem item)
        {
            PrintLog($"RegisterItem =>[{item.DialogName}]");

            if (PageList.ContainsKey(item.DialogName))
            {
                Debug.LogError($"添加 DialogName[{item.DialogName}] 失敗，已經有相同名稱");
            }
            else
            {
                PageList.Add(item.DialogName, item);
                ToggleRegisterItemEvent?.Invoke(item.DialogName, true);
            }
            //DialogReturnValueDictionary跨場景使用時,因載入場景時間差有可能造成還沒有註冊DialogItem就因為先被Subcribe而新增;不影響使用
            if (!DialogReturnValueDictionary.ContainsKey(item.DialogName))
            {
                DialogReturnValueDictionary.Add(item.DialogName, new ReactiveProperty<DialogItem.ReturnValue_Base>(null));
            }
            Debug.LogWarning($"添加 PageName[{item.DialogName}] 成功");
        }
        public static void RemoveItem(DialogItem item)
        {
            if (PageList.ContainsKey(item.DialogName))
            {
                Debug.LogWarning($"移除 PageName[{item.DialogName}]");
                PageList.Remove(item.DialogName);
                DialogReturnValueDictionary.Remove(item.DialogName);

                ToggleRegisterItemEvent?.Invoke(item.DialogName, false);
            }
            else
            {
                Debug.LogError($"PageName[{item.DialogName}]不存在，無法移除");
            }
        }
        public static void PrintLog(string value, string title = "PageManager", string color = "white")
        => Debug.Log(value);
        public static void PrintLogWarning(string value, string title = "PageManager", string color = "white")
        => Debug.LogWarning(value);
        public static void PrintLogError(string value, string title = "PageManager", string color = "white")
        {
            Debug.LogError(value);
        }
        //public Action<string, DialogResult> DialogResultEvent;
        public static IDisposable SubscribeReturnValue(string key, Action<DialogItem.ReturnValue_Base> callback)
        {
            IDisposable dispose = null;
            //DialogReturnValueDictionary跨場景使用時,因載入場景時間差有可能造成還沒有註冊DialogItem就因為先被Subcribe;自行新增ReactiveProperty
            if (!DialogReturnValueDictionary.ContainsKey(key))
            {
                DialogReturnValueDictionary[key] = new ReactiveProperty<DialogItem.ReturnValue_Base>();
                Debug.Log($"DialogManager-SubscribeReturnValue[{key}] does not exist, create a new one.");
            }
            dispose = DialogReturnValueDictionary[key].Subscribe(callback);
            return dispose;
        }
        /// <summary>
        /// 這個Function不應該存在;偷懶專用-為了讓不是Dialog的頁面發出假的DialogShow事件通知
        /// </summary>
        /// <param name=""></param>
        public static void Dummy_ShowDialogEventSender(string dialogName) //目前應該只有MemberLevelUpViewPage使用;如果有更好的方法最好砍掉
        {
            ShowDialogEvent?.Invoke(dialogName);
        }
        /// <summary>
        /// 這個Function不應該存在;偷懶專用-為了讓不是Dialog的頁面發出假的DialogHide事件通知
        /// </summary>
        /// <param name=""></param>
        public static void Dummy_HideDialogEventSender(string dialogName) //目前應該只有MemberLevelUpViewPage使用;如果有更好的方法最好砍掉
        {
            HideDialogEvent?.Invoke(dialogName);
        }
    }

    public enum DialogResult
    {
        Cancel = -1,
        None = 0,
        Confirm = 1
    }
}