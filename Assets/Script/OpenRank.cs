using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Platform;
using UnityEngine;
using UnityEngine.UI;

public class OpenRank : MonoBehaviour
{
    [SerializeField] private Button openRankBtn;

    private void Start()
    {
        openRankBtn.onClick.AddListener(OnOpenBtnClicked);
        SoundController.Instance.Play(EGSoundKey.EGLobbyBGM);
    }

    private void OnOpenBtnClicked()
    {
        DialogManager.ShowDialog("Dia_MediaPlayer", new Dia_MediaPlayer.MyParam
        {
            VedioUrl = "https://grd4.wdapprd1234.net/Record/Video/6011400780570000000200000.mp4",
            enterType = Dia_MediaPlayer.EnterType.排行榜回放
        });
    }
}
