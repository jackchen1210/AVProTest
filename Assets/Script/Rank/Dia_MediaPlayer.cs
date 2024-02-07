using Core;
using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace Platform
{
    public class Dia_MediaPlayer : DialogItem
    {
        [SerializeField]
        private CustomMediaPlayerUI mediaPlayerUI;

        //public int EGCanvasOrder = -61;
        //public Canvas canvas;

        //[SerializeField]
        //private GameObject backButton;
        //[SerializeField]
        //private GameObject cancelButton;

        //用來暫停音樂用的辨識物件
        private object _mediaLocker = new object();

        public enum RankType
        {
            富豪榜 = 0,
            單局贏分 = 1,
            單局倍率 = 2,
            彩金榜 = 3
        }

        public enum EnterType
        {
            彩金回放 = 0,
            排行榜回放 = 1
        }

        public class MyParam : ShowParameters_Base
        {
            /// <summary>
            /// 注單號
            /// </summary>
            public string WagerSerial { get; set; } = string.Empty;

            /// <summary>
            /// 回放類型
            /// </summary>
            public EnterType enterType { get; set; } = EnterType.排行榜回放;

            /// <summary>
            /// 回放連結
            /// </summary>
            public string VedioUrl { get; set; } = string.Empty;
        }

        //private IDisposable timer;
        private MyParam param;

        public override void Awake()
        {
            NetUtility.PLATFORM_INFO_DATA.OnRsDisconnect.Subscribe(OnRsDisconnect);
            NetUtility.EGGame_ReconnectStateEvent += OnConnectStateEvent;
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            mediaPlayerUI.SetFreezeFunction(StopBackground);
        }

        void OnConnectStateEvent(NetConnectState state, EGGamePlayState game_st)
        {
            if (!gameObject.activeSelf) return;

            switch (state)
            {
                case NetConnectState.None:
                    {
                    }
                    break;
                case NetConnectState.Disconencted:
                    {
                        mediaPlayerUI.PauseMedia();
                    }
                    break;
                case NetConnectState.Connecting:
                    {
                    }
                    break;
                case NetConnectState.ConnectFail:
                    {
                        mediaPlayerUI.PauseMedia();
                    }
                    break;
                case NetConnectState.Connected:
                    {
                        mediaPlayerUI.ResumeMedia();
                        StopBackground();
                    }
                    break;
            }
        }

        protected void OnRsDisconnect(DisconnectReasonType reason)
        {
            if (!gameObject.activeSelf) return;

            mediaPlayerUI.CloseMedia();
        }


        protected override void OnViewPageShow()
        {
            StopBackground();
            param = myParams as MyParam;

            base.OnViewPageShow();
            if (param != null)
            {
                //if (param.enterType == EnterType.排行榜回放)
                //{
                //    canvas.overrideSorting = false;
                //    //Canvas.sortingOrder = EGMessageBox.Instance.halfBlackCanvasOrder - 1;

                //}
                //else
                //{
                //    canvas.overrideSorting = true;
                //    canvas.sortingOrder = EGCanvasOrder;
                //}

                if (string.IsNullOrEmpty(param.WagerSerial) && string.IsNullOrEmpty(param.VedioUrl))
                {
                    MessageBox.ShowSystemMsg("SYSTEM_BUSY", null);
                    NetUtility.PrintLogError("回放單號為空");
                    return;
                }
                if (!string.IsNullOrEmpty(param.VedioUrl))
                {
                    DelayToInvoke.Instance.Do(() =>
                    {
                        ShowVideo(RankType.彩金榜, param.VedioUrl);
                    }, 0.1f);
                }
                else
                {
                    DelayToInvoke.Instance.Do(() =>
                    {
                        ShowVideo(RankType.彩金榜, CreateVideoUrl(param.WagerSerial));
                    }, 0.1f);
                }
                return;
            }
        }

        protected void StopBackground()
        {
            if (!gameObject.activeSelf) return;
            SoundManagerController.Instance.MediaPause(_mediaLocker.GetHashCode());
            NetUtility.FreezeUpdate(true);
        }


        protected void PlayBackground()
        {
            NetUtility.FreezeUpdate(false);
            SoundManagerController.Instance.MediaResume(_mediaLocker.GetHashCode());
        }

        protected override void OnViewPageHide()
        {
            mediaPlayerUI.CloseMedia();
            PlayBackground();
        }

        private string CreateVideoUrl(string wagerSerial)
        {
            return $"https://rd4.alpha5d777.com/Record/Video/{wagerSerial}.mp4";
        }

        private void ShowVideo(RankType rankType, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("url 為空，無法撥放");
                return;
            }
            url = Tool.AddUrlProtocolIfDontHave(url);
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Debug.LogError("url格式錯誤 : " + url);
                return;
            }

            Debug.Log("URL: " + url);
            mediaPlayerUI.OpenMedia(url);
        }

        public override void UI_Init()
        {

        }

        private void OnTimeout(long tick)
        {
            HideDialog();
            MessageBox.Show("SYSTEM_BUSY", NetUtility.GetStringByKey("SYSTEM_BUSY"));
        }
    }
}