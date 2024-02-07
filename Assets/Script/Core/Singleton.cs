using System;

public class Singleton<T>
    where T : class, new()
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// 釋放元件讓 GC 回收
    /// </summary>
    public static void Clean()
    {
        if (_instance != null)
        {
            lock (_lock)
            {
                if (_instance != null)
                {
                    // 元件繼承 IDisposable
                    //_instance.Dispose();

                    _instance = null;

                    //GC.SuppressFinalize(_instance);
                }
            }
        }

    }


    public static void Dispose()
    {
        if (_instance != null)
        {
            lock (_lock)
            {
                if (_instance != null)
                {
                    GC.SuppressFinalize(_instance);

                    _instance = null;
                }
            }
        }

    }
}
