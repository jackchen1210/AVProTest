using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class DialogEvent
    {
        Action _action;

        public void Add(Action action)
        {
            _action += action;
        }

        public void Remove(Action action)
        {
            _action -= action;
        }

        public void Clear()
        {
            _action = null;
        }

        public void Emit()
        {
            //foreach (Action v in _action.GetInvocationList())
            //{
            //    try
            //    {
            //        v();
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.LogError(ex);
            //    }
            //}
            _action?.Invoke();
        }
    }

    public class DialogEvent<T>
    {
        Action<T> _action;

        public void Add(Action<T> action)
        {
            _action += action;
        }

        public void Remove(Action<T> action)
        {
            _action -= action;
        }

        public void Clear()
        {
            _action = null;
        }

        public void Emit(T p1)
        {
            //foreach (Action<T> v in _action.GetInvocationList())
            //{
            //    try
            //    {
            //        v(p1);
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.LogError(ex);
            //    }
            //}
            _action?.Invoke(p1);
        }
    }

    public class DialogEvent<T1, T2>
    {
        Action<T1, T2> _action;

        public void Add(Action<T1, T2> action)
        {
            _action += action;
        }

        public void Remove(Action<T1, T2> action)
        {
            _action -= action;
        }

        public void Clear()
        {
            _action = null;
        }

        public void Emit(T1 p1, T2 p2)
        {
            //foreach (Action<T1, T2> v in _action.GetInvocationList())
            //{
            //    try
            //    {
            //        v(p1, p2);
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.LogError(ex);
            //    }                
            //}
            _action?.Invoke(p1, p2);
        }
    }

    public class DialogEvent<T1, T2, T3>
    {
        Action<T1, T2, T3> _action;

        public void Add(Action<T1, T2, T3> action)
        {
            _action += action;
        }

        public void Remove(Action<T1, T2, T3> action)
        {
            _action -= action;
        }

        public void Clear()
        {
            _action = null;
        }

        public void Emit(T1 p1, T2 p2, T3 p3)
        {
            //foreach (Action<T1, T2, T3> v in _action.GetInvocationList())
            //{
            //    try
            //    {
            //        v(p1, p2, p3);
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.LogError(ex);
            //    }
            //}
            _action?.Invoke(p1, p2, p3);
        }
    }

    public class DialogEvent<T1, T2, T3, T4>
    {
        Action<T1, T2, T3, T4> _action;

        public void Add(Action<T1, T2, T3, T4> action)
        {
            _action += action;
        }

        public void Remove(Action<T1, T2, T3, T4> action)
        {
            _action -= action;
        }

        public void Clear()
        {
            _action = null;
        }

        public void Emit(T1 p1, T2 p2, T3 p3, T4 p4)
        {
            //foreach (Action<T1, T2, T3, T4> v in _action.GetInvocationList())
            //{
            //    try
            //    {
            //        v(p1, p2, p3, p4);
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.LogError(ex);
            //    }
            //}
            _action?.Invoke(p1, p2, p3, p4);
        }
    }
}
