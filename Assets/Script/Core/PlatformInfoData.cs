using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Core
{
    public class PlatformInfoData
    {
        public List<Route> Routes { get; set; }
        public bool IsAutoLogin { get; set; }
        public bool bEnableThridLogin = false;
        public bool IsLevelUp = false;
        /// <summary>
        /// 是否正在客服頁面。
        /// </summary>
        public bool IS_IN_SERVICE = false;
        public List<ReactiveProperty<long>> GamePointList = new List<ReactiveProperty<long>>() { new ReactiveProperty<long>() { Value = 0 }, new ReactiveProperty<long>() { Value = 0 }, new ReactiveProperty<long>() { Value = 0 }, new ReactiveProperty<long>() { Value = 0 } };
        public ReactiveProperty<long> GoldPoint = new ReactiveProperty<long>() { Value = 0 };
        public ReactiveProperty<long> GiftPoint = new ReactiveProperty<long>() { Value = 0 };
        public ReactiveProperty<string> NickName = new ReactiveProperty<string>() { Value = "" };
        public ReactiveProperty<string> FullName = new ReactiveProperty<string>() { Value = "" };
        public ReactiveProperty<DateTimeOffset> RegisterTime = new ReactiveProperty<DateTimeOffset>() { Value = new DateTimeOffset() };
        //public ReactiveProperty<bool> IsUpdateAccount { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<API_AppSettings> AppSettings = new ReactiveProperty<API_AppSettings>();

        public Subject<DisconnectReasonType> OnRsDisconnect { get; } = new Subject<DisconnectReasonType>();

        public List<ReactiveProperty<AvailableGamePlatformModel>> GamePlatformList = new List<ReactiveProperty<AvailableGamePlatformModel>>()
        {
            new ReactiveProperty<AvailableGamePlatformModel>() { Value = new AvailableGamePlatformModel() {
                Id = 0,
                ServiceId = ServiceIDEnum.ElectGame.ToString(),
                ServiceName = "電子館",
                Sort = 1,
                TransferStatus = true,
                MaintainStatus = false,
                IsTest = true
            } },
            new ReactiveProperty<AvailableGamePlatformModel>() { Value = new AvailableGamePlatformModel() {
                Id = 1,
                ServiceId = ServiceIDEnum.LiveGame.ToString(),
                ServiceName = "視訊館",
                Sort = 2,
                TransferStatus = true,
                MaintainStatus = false,
                IsTest = true
            } },
            new ReactiveProperty<AvailableGamePlatformModel>() { Value = new AvailableGamePlatformModel() {
                Id = 2,
                ServiceId = ServiceIDEnum.LotteryGame.ToString(),
                ServiceName = "彩球館",
                Sort = 3,
                TransferStatus = false,
                MaintainStatus = false,
                IsTest = true
            } },
            new ReactiveProperty<AvailableGamePlatformModel>() { Value = new AvailableGamePlatformModel() {
                Id = 3,
                ServiceId = ServiceIDEnum.SportGame.ToString(),
                ServiceName = "體育館",
                Sort = 4,
                TransferStatus = true,
                MaintainStatus = true,
                IsTest = true
            } },
        };
        public Dictionary<string, ReactiveProperty<JToken>> PlatformInfoList = new Dictionary<string, ReactiveProperty<JToken>>();
        /// <summary>
        /// 玩家頭像資料
        /// </summary>
        public ReactiveProperty<MemberAvatarSharedDataModel> MemberAvatarSharedData = new ReactiveProperty<MemberAvatarSharedDataModel>() { Value = new MemberAvatarSharedDataModel() };

        public bool IsSymbolPasswordReturn { get; set; }
        public bool IsLoginPanelReturn { get; set; }
        public ReactiveProperty<MemberAvatarSharedDataModel[]> SystemAvatars { get; set; } = new ReactiveProperty<MemberAvatarSharedDataModel[]>(new MemberAvatarSharedDataModel[0]);

        /// <summary>
        /// 平台主題
        /// </summary>
        public ReactiveProperty<ThemeType> PlatformThemeType = new ReactiveProperty<ThemeType>() { Value = ThemeType.White };

    }

    /// <summary> 來自SR封包的會員資料結構 </summary>
    public class RawMemberAvatarSharedData
    {
        public string MemberAvatarSharedData;
    }

    public class MemberAvatarSharedDataModel
    {
        /// <summary>
        /// 是否為系統頭像
        /// </summary>
        public bool IsSysAvatar = true;
        /// <summary>
        /// 系統頭像編號
        /// </summary>
        public int AvatarNo = 1;
        /// <summary>
        /// 個人化頭像網址
        /// </summary>
        public string AvatarUrl = "";
    }
    public enum RequestType
    {
        PostData,
        UploadImage,
    }
    public class APIRequestList
    {
        public string RequestKey { get; set; } = null;
        public string PutData { get; set; } = null;
        public byte[] PutBytes { get; set; } = null;
        public bool NeedAuthorization { get; set; } = false;
        public RequestType RequestType { get; set; } = RequestType.PostData;
        public Action<JToken> Callback { get; set; } = null;
        public JObject otherHeader { get; set; } = null;
    }
    public class API_AppSettings
    {
        /// <summary>
        /// 是否開啟系統推播
        /// </summary>
        public bool IsSystemNotification = true;
        /// <summary>
        /// 是否為第一次編輯
        /// </summary>
        public bool IsFirstEdit;
        /// <summary>
        /// 是否開啟大獎推播
        /// </summary>
        public bool IsAwardNotification;
        /// <summary>
        /// 遊戲轉幣設定
        /// </summary>
        public List<GameCollectionItem> GameCollection = new List<GameCollectionItem>();

        public class GameCollectionItem
        {
            public string ServiceID;
            public string ServiceName;
            public bool IsDisabled = true;
            public bool IsAutoExch = true;
        }
    }
    public enum ServiceIDEnum
    {
        /// <summary>
        /// 平台大廳
        /// </summary>
        PlatformLobby,
        /// <summary>
        /// 電子館
        /// </summary>
        ElectGame,
        /// <summary>
        /// 視訊館
        /// </summary>
        LiveGame,
        /// <summary>
        /// 彩球館;暫訂,API還沒給
        /// </summary>
        LotteryGame,
        /// <summary>
        /// 體育館;暫訂,API還沒給
        /// </summary>
        SportGame,
    }
    public enum NetStateEnum
    {
        /// <summary>
        /// 大廳
        /// </summary>
        PlatformLobby,
        /// <summary>
        /// 電子館
        /// </summary>
        ElectGame,
        /// <summary>
        /// 正在前往電子館
        /// </summary>
        Going_ElectGame,
        /// <summary>
        /// 正在前往大廳
        /// </summary>
        Going_PlatformLobby,
    }
    public partial class Route
    {
        public long? Sid { get; set; }
        public string Url { get; set; }
    }
    public partial class GetMarqueeResult
    {
        public int? Seqno { get; set; }
        public string Detail { get; set; }
    }
    public partial class TransferInGamePointsListResultModel
    {
        public string ServiceId { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
    }

    public class AvailableGamePlatformModel
    {
        /// <summary>
        /// 遊戲平台 序號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 遊戲平台 ID
        /// </summary>
        public string ServiceId { get; set; }
        /// <summary>
        /// 遊戲平台名稱
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 平台設定，轉帳是否開啟
        /// </summary>
        public bool TransferStatus { get; set; }
        /// <summary>
        /// 平台設定，是否為維運狀態
        /// </summary>
        public bool MaintainStatus { get; set; }
        /// <summary>
        /// 維運開始時間
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }
        /// <summary>
        /// 維運結束時間
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }
        /// <summary>
        /// 是否對測試帳號有開啟
        /// </summary>
        public bool IsTest { get; set; }
        /// <summary>
        /// 是否開啟
        /// <br></br>
        /// 單一會員關閉進館
        /// </summary>
        public bool IsDisabled { get; set; }
    }

    public class GetMemberLevelAuthByMemberLevelResultModel
    {
        /// <summary>
        /// 當日轉出上限，若為Null代表該等級未開放轉帳交易
        /// </summary>
        public double? LimitTransfer { get; set; }
        /// <summary>
        /// 是否可以商城購買
        /// </summary>
        public bool IsGameMall { get; set; }
        /// <summary>
        /// 單日儲值限額
        /// </summary>
        public double LimitDepositAMT { get; set; }
        /// <summary>
        /// 單日儲值次數
        /// </summary>
        public double LimitDepositCNT { get; set; }
        /// <summary>
        /// 贈禮金幣(體驗金幣)NULL:沒設定
        /// </summary>
        public double? BonusGold { get; set; }
        /// <summary>
        /// 訪客、玩家禁止的遊戲館
        /// </summary>
        public string[] ProhibitServiceIDs { get; set; } = new string[0];
    }

    public enum ThemeType
    {
        White = 0,
        Black = 1
    }
}