using System;
using UnityEngine;

public class MessageBox : Singleton<MessageBox>
{

    internal static void ShowSystemMsg(string v, object p)
    {
        Debug.LogError("Show System Msg");
    }

    internal static void Show(string v, object p)
    {
        Debug.LogError("Show System Msg");
    }
}
