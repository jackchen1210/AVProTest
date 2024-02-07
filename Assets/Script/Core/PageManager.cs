using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Core
{
    public class PageManager : Singleton<PageManager>
    {
        public static bool IsActive { get { return PageList.Count > 0; } }
        private static Dictionary<string, List<ViewPage>> PageList = new Dictionary<string, List<ViewPage>>();

        //事件通知改變 方便開發
        public static Action PageChanged;

        public static ViewPage GetPage(string SceneName, string PageName)
        {
            if (PageList.ContainsKey(SceneName))
            {
                var list = PageList[SceneName];
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].PageName == PageName)
                    {
                        return list[i];
                    }
                }
            }
            return null;
        }

        public static void OpenPage(string SceneName, string PageName, bool closeAll = false)
        {
            OpenPage(SceneName, new List<string> { PageName }, closeAll);
        }
        public static void OpenPage(string SceneName, List<string> PageName, bool closeAll = false)
        {
            //PrintLog($"[{SceneName}] OpenPage =>[{string.Join(",",PageName)}][{closeAll}]");
            //廣播
            if (PageList.ContainsKey(SceneName))
            {
                foreach (var li in PageList[SceneName])
                {
                    if (PageName.Contains(li.PageName))
                    {
                        li.Show();
                    }
                    else
                    {
                        if (closeAll)
                            li.Hide();
                    }
                }
            }
            //PageChanged?.Invoke();
        }
        public static void CloseAll()
        {
            foreach (var li in PageList)
            {
                CloseAll(li.Key);
            }
        }
        public static void CloseAll(string SceneName)
        {
            if (PageList.ContainsKey(SceneName))
            {
                //Debug.Log($"CloseAll[{SceneName}]");
                PageList[SceneName].ForEach(x => x.Hide());
            }
            //PageChanged?.Invoke();
        }
        public static void Back()
        {
            foreach (var li in PageList)
            {
                li.Value.ForEach(x => x.Back());
            }
            //PageChanged?.Invoke();
        }
        public static bool ContainsKey(ViewPage item)
        {
            bool result = false;
            if (PageList.ContainsKey(item.SceneName))
            {
                if (PageList[item.SceneName].Contains(item))
                {
                    result = true;
                }
            }
            return result;
        }
        public static void RegisterItem(ViewPage item)
        {
            List<ViewPage> list;
            if (PageList.ContainsKey(item.SceneName))
            {
                list = PageList[item.SceneName];
                list.Add(item);
            }
            else
            {
                list = new List<ViewPage>();
                list.Add(item);
                PageList.Add(item.SceneName, list);
            }
            Debug.LogWarning($"添加 PageName[{item.SceneName}][{item.PageName}]");
        }
        public static void RemoveItem(ViewPage item)
        {
            if (PageList.ContainsKey(item.SceneName))
            {
                var list = PageList[item.SceneName];
                Debug.LogWarning($"移除 PageName[{item.PageName}]");
                list.Remove(item);
            }
            else
            {
                Debug.LogError($"PageName[{item.PageName}]不存在，無法移除");
            }
        }
        public static void PrintLog(string value, string title = "PageManager", string color = "white")
        => CLog.PrintLog(value, title, color, LogLevel.Share_UI);

        public static void PrintLogWarning(string value, string title = "PageManager", string color = "white")
        => CLog.PrintLogWarning(value, title, color, LogLevel.Share_UI);

        public static void PrintLogError(string value, string title = "PageManager", string color = "white")
        {
            CLog.PrintLogError(value, title, color, LogLevel.Share_UI);
        }
    }
}