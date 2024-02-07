[System.Flags]
public enum LogLevel
{
    None = 0,

    /// <summary> 一般 </summary>
    Normal = 0x01 << 1,

    /// <summary> RSAPI </summary>
    Api = 0x01 << 2,

    /// <summary> RS網路 </summary>
    RsNet = 0x01 << 3,

    /// <summary> 電子館網路 </summary>
    EGNet = 0x01 << 4,

    /// <summary> 電子館邏輯 </summary>
    EGNetLogic = 0x01 << 5,

    /// <summary> 網路底層 </summary>
    Socket = 0x01 << 6,

    /// <summary> 電子共用 </summary>
    EG_UI = 0x01 << 7,

    /// <summary> 電子遊戲 </summary>
    EG_Game = 0x01 << 8,

    /// <summary> 音樂音效 </summary>
    Sound = 0x01 << 9,

    /// <summary> 步驟節點 </summary>
    StepProcess = 0x01 << 10,

    /// <summary> 共用 </summary>
    Share_UI = 0x01 << 11,

    /// <summary> 平台相關 </summary>
    Platform = 0x01 << 12,

    /// <summary> CLOG </summary>
    CLog = 0x01 << 13,

    WebView = 0x01 << 14,
    PlatformLogin = 0x01 << 15,
    CustomerService,
    /// <summary> 全部 </summary>
    All = Normal | Api | RsNet | EGNet | EGNetLogic | Socket | EG_UI | EG_Game | Sound | StepProcess | Share_UI | Platform | CLog | WebView | PlatformLogin | CustomerService
}