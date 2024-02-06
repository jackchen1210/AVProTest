using System;
using RenderHeads.Media.AVProVideo.Demos;
using UnityEngine;
using UnityEngine.UI;

public class TestPanel : MonoBehaviour
{
    [SerializeField] private Button toggleVideoBtn;
    [SerializeField] private Button playBgmBtn;
    [SerializeField] private Button pauseBgmBtn;
    [SerializeField] private BGMPlayer bGMPlayer;
    [SerializeField] private MediaPlayerUI mediaPlayer;
    [SerializeField] private GameObject mediaGo;

    private void Start()
    {
        toggleVideoBtn.onClick.AddListener(OnToggleVideoBtnClicked);
        playBgmBtn.onClick.AddListener(OnPlayBgmBtnClicked);
        pauseBgmBtn.onClick.AddListener(OnPauseBgmBtnClicked);
    }

    private void OnPauseBgmBtnClicked()
    {
        bGMPlayer.Pause();
    }

    private void OnPlayBgmBtnClicked()
    {
        bGMPlayer.Play();
    }

    private void OnToggleVideoBtnClicked()
    {
        mediaGo.SetActive(!mediaGo.activeSelf);
    }
}
