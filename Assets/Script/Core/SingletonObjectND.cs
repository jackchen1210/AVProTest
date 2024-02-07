using Core;
using Sirenix.OdinInspector;
using UnityEngine;
/// <summary>
/// 自動單例包含DontDestroyOnLoad
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonObjectND<T> : MonoBehaviour where T : MonoBehaviour
{
    #region 自動單例

    private static T _instance;
    private static bool IsCreate = false;
    private static bool IsDestory = false;
    private static object _lock = new object();

    public static T Instance
    {
        get
        {

            lock (_lock)
            {
                if (_instance == null && !IsDestory)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        CLog.PrintLogError("<color=yellow>[單例ND]</color> 錯誤，現在有超過一個單例 ", $"單例");
                        return _instance;
                    }

                    if (_instance == null || _instance.gameObject == null)
                    {
                        IsCreate = true;
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = $"(singleton){typeof(T).ToString()}";

                        DontDestroyOnLoad(singleton);

                        Debug.LogWarning($"<color=yellow>[單例ND]</color> 場景中需要 {typeof(T)} 的對象，所以 {singleton} 創建了DontDestroyOnLoad單例.");
                    }
                    else
                    {
                        if (_instance.transform.parent == null)
                        {
                            DontDestroyOnLoad(_instance.gameObject);
                            Debug.LogWarning($"<color=yellow>[單例ND]</color> 使用的對象單例{typeof(T)} { _instance.gameObject.name}已存在，添加進DontDestroyOnLoad");
                        }
                        else
                        {
                            Debug.LogWarning($"<color=yellow>[單例ND]</color> 使用的對象單例{typeof(T)} { _instance.gameObject.name}已存在，不是ROOT所以不添加進DontDestroyOnLoad");
                        }
                    }
                }
                else if (_instance == null)
                {
                    Debug.LogWarning($"<color=yellow>[單例ND]</color> 使用的對象 {typeof(T)}單例 IsDestory = true");
                }

                return _instance;
            }
        }
    }
    public virtual void Awake()
    {
        if (IsCreate)
        {
            Debug.LogWarning($"<color=yellow>[單例ND]</color>{typeof(T)} Awake 單例自動創建");
        }
        else
        {
            if (Instance != null)
            {
                Debug.LogWarning($"<color=yellow>[單例ND]</color>{typeof(T)} Awake 成功 ");
            }
            else
            {
                Debug.LogError($"<color=yellow>[單例ND]</color>{typeof(T)} Awake 失敗");
            }
        }
    }
    public virtual void OnDestroy()
    {
        if (_instance == null) return;
        CLog.PrintLog($"<color=yellow>[單例ND]</color>  {typeof(T)} 的對象被摧毀，所以 {gameObject.name} 設為NULL.", $"單例 {gameObject.name}");
        _instance = null;
        IsDestory = true;
    }

    #endregion 自動單例

}

