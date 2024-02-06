using RenderHeads.Media.AVProVideo;
using RenderHeads.Media.AVProVideo.Demos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayVideo : MonoBehaviour
{
    public AudioSource bgm = null;
    public AudioClip clip;
    [SerializeField] MediaPlayerUI mediaPlayerUI = null;

    // Start is called before the first frame update
    void Start()
    {
        clip = Resources.Load<AudioClip>("joystock-upbeat-motivation");
        bgm.clip = clip;
        bgm.Play();
        Debug.Log("bgm.Play");
    }

    public void ToggleMediaPlayer()
    {
        bool active = !mediaPlayerUI.gameObject.activeSelf;
        Debug.Log(mediaPlayerUI.gameObject.activeSelf);
        Debug.Log(active);
        mediaPlayerUI.gameObject.SetActive(active);
        if (active)
        {
            bgm.Pause();
            Debug.Log("bgm.Pause");
            mediaPlayerUI.gameObject.SetActive(active);
            mediaPlayerUI.ResumeMedia();
        }
        else
        {
            mediaPlayerUI.gameObject.SetActive(active);
            mediaPlayerUI.CloseMedia();
            bgm.Play();
            Debug.Log("bgm.Play");
        }
    }

}
