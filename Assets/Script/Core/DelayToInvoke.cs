using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DelayToInvoke : Singleton<DelayToInvoke>
{
    /// <summary>
    /// 延時委派
    /// </summary>
    /// <param name="action">事件</param>
    /// <param name="delaySeconds">延遲秒數</param>
    public void Do(Action action, float delaySeconds, int Id = 0)
    {
        if (!DelayToInvokeDic.ContainsKey(Id))
        {
            DelayToInvokeDic.Add(Id, new Queue<IDisposable>());
        }
        DelayToInvokeDic[Id].Enqueue(
        Observable.Timer(TimeSpan.FromSeconds(delaySeconds), Scheduler.MainThreadIgnoreTimeScale).SubscribeOnMainThread().Subscribe(_ =>
        {
            action?.Invoke();
        }));
    }
    /// <summary>
    /// 延時委派
    /// </summary>
    /// <param name="action">事件</param>
    /// <param name="delaySeconds">延遲秒數</param>
    public IDisposable DoAndCallBack(Action action, float delaySeconds, int Id = 0)
    {
        if (!DelayToInvokeDic.ContainsKey(Id))
        {
            DelayToInvokeDic.Add(Id, new Queue<IDisposable>());
        }
        var cancel = Observable.Timer(TimeSpan.FromSeconds(delaySeconds), Scheduler.MainThreadIgnoreTimeScale).SubscribeOnMainThread().Subscribe(_ =>
        {
            action?.Invoke();
        });
        DelayToInvokeDic[Id].Enqueue(cancel);
        return cancel;
    }

    public void NextFrame(Action action, int Id = 0)
    {
        if (!DelayToInvokeDic.ContainsKey(Id))
        {
            DelayToInvokeDic.Add(Id, new Queue<IDisposable>());
        }
        DelayToInvokeDic[Id].Enqueue(
        Observable.NextFrame().SubscribeOnMainThread().Subscribe(_ =>
        {
            action?.Invoke();
        }));
    }

    Dictionary<int, Queue<IDisposable>> DelayToInvokeDic = new Dictionary<int, Queue<IDisposable>>();
    Queue<IDisposable> Disposables = new Queue<IDisposable>();
    public void ClearDo(int Id = 0)
    {
        // Debug.LogError($"刪除 Disposables!! ID:{Id}");
        if (!DelayToInvokeDic.ContainsKey(Id))
        {
            return;
        }
        while (DelayToInvokeDic[Id].Count > 0)
        {
            DelayToInvokeDic[Id].Dequeue()?.Dispose();
        }
    }

    public void ClearAll(int Id = 0)
    {
        // Debug.LogError($"刪除全部");
        foreach (var d in DelayToInvokeDic.Values)
        {
            while (d.Count > 0)
            {
                d.Dequeue()?.Dispose();
            }
        }
    }
}
