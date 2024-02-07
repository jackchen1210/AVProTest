// UnityEngine.UI was moved to a package in 2019.2.0
// Unfortunately no way to test for this across all Unity versions yet
// You can set up the asmdef to reference the new package, but the package doesn't 
// existing in Unity 2017 etc, and it throws an error due to missing reference
#define AVPRO_PACKAGE_UNITYUI
#if (UNITY_2019_2_OR_NEWER && AVPRO_PACKAGE_UNITYUI) || (!UNITY_2019_2_OR_NEWER)

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RenderHeads.Media.AVProVideo;
using Core;

//-----------------------------------------------------------------------------
// Copyright 2018-2021 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Platform
{
    public class CustomMediaPlayerUI : MonoBehaviour
    {
        [SerializeField] MediaPlayer _mediaPlayer = null;

        [Header("Options")]

        [SerializeField] float _keyVolumeDelta = 0.05f;
        [SerializeField] float _jumpDeltaTime = 5f;
        [SerializeField] bool _showOptions = true;
        [SerializeField] bool _autoHide = true;
        [SerializeField] float _userInactiveDuration = 1.5f;
        [SerializeField] bool _useAudioFading = true;

        [Header("Keyboard Controls")]
        [SerializeField] bool _enableKeyboardControls = true;
        [SerializeField] KeyCode KeyVolumeUp = KeyCode.UpArrow;
        [SerializeField] KeyCode KeyVolumeDown = KeyCode.DownArrow;
        [SerializeField] KeyCode KeyTogglePlayPause = KeyCode.Space;
        [SerializeField] KeyCode KeyToggleMute = KeyCode.M;
        [SerializeField] KeyCode KeyJumpForward = KeyCode.RightArrow;
        [SerializeField] KeyCode KeyJumpBack = KeyCode.LeftArrow;

        [Header("Optional Components")]
        [SerializeField] CustomOverlayManager _overlayManager = null;
        [SerializeField] MediaPlayer _thumbnailMediaPlayer = null;
        [SerializeField] RectTransform _timelineTip = null;

        [Header("UI Components")]
        [SerializeField] RectTransform _canvasTransform = null;
        //[SerializeField] Image image = null;
        [SerializeField] Slider _sliderTime = null;
        [SerializeField] EventTrigger _videoTouch = null;
        [SerializeField] CanvasGroup _controlsGroup = null;

        private bool _wasPlayingBeforeTimelineDrag;
        private float _controlsFade = 1f;
        private Material _playPauseMaterial;
        private Material _volumeMaterial;
        private Material _subtitlesMaterial;
        private Material _optionsMaterial;
        private Material _audioSpectrumMaterial;
        private float[] _spectrumSamples = new float[128];
        private float[] _spectrumSamplesSmooth = new float[128];
        private float _maxValue = 1f;
        private float _audioVolume = 1f;

        private float _audioFade = 0f;
        private bool _isAudioFadingUpToPlay = true;
        private const float AudioFadeDuration = 0.25f;
        private float _audioFadeTime = 0f;

        private readonly LazyShaderProperty _propMorph = new LazyShaderProperty("_Morph");
        private readonly LazyShaderProperty _propMute = new LazyShaderProperty("_Mute");
        private readonly LazyShaderProperty _propVolume = new LazyShaderProperty("_Volume");
        private readonly LazyShaderProperty _propSpectrum = new LazyShaderProperty("_Spectrum");
        private readonly LazyShaderProperty _propSpectrumRange = new LazyShaderProperty("_SpectrumRange");

        void Awake()
        {
            //#if UNITY_IOS
            //			Application.targetFrameRate = 60;
            //#endif

#if UNITY_IOS
            delayLoadingTime = 0.5f;
#else
            delayLoadingTime = 0.1f;
#endif
        }

        void Start()
        {
            if (_mediaPlayer)
            {
                _audioVolume = _mediaPlayer.AudioVolume;
            }

            CreateTimelineDragEvents();
            CreateVideoTouchEvents();
            BuildOptionsMenu();

            _mediaPlayer.Events.AddListener(OnMediaPlayerEvent);
        }

        private struct UserInteraction
        {
            public static float InactiveTime;
            private static Vector3 _previousMousePos;
            private static int _lastInputFrame;

            public static bool IsUserInputThisFrame()
            {
                if (Time.frameCount == _lastInputFrame)
                {
                    return true;
                }
#if (!ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER)
                bool touchInput = (Input.touchSupported && Input.touchCount > 0);
                bool mouseInput = (Input.mousePresent && (Input.mousePosition != _previousMousePos || Input.mouseScrollDelta != Vector2.zero || Input.GetMouseButton(0)));

                if (touchInput || mouseInput)
                {
                    _previousMousePos = Input.mousePosition;
                    _lastInputFrame = Time.frameCount;
                    return true;
                }

                return false;
#else
				return true;
#endif
            }
        }

        private bool _isHoveringOverTimeline;

        void UpdateAudioFading()
        {
            // Increment fade timer
            if (_audioFadeTime < AudioFadeDuration)
            {
                _audioFadeTime = Mathf.Clamp(_audioFadeTime + Time.deltaTime, 0f, AudioFadeDuration);
            }

            // Trigger pause when audio faded down
            if (_audioFadeTime >= AudioFadeDuration)
            {
                if (!_isAudioFadingUpToPlay)
                {
                    Pause(skipFeedback: true);
                }
            }

            // Apply audio fade value
            if (_mediaPlayer.Control != null && _mediaPlayer.Control.IsPlaying())
            {
                _audioFade = Mathf.Clamp01(_audioFadeTime / AudioFadeDuration);
                if (!_isAudioFadingUpToPlay)
                {
                    _audioFade = (1f - _audioFade);
                }
                ApplyAudioVolume();
            }
        }

        public void TogglePlayPause()
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                if (_useAudioFading && _mediaPlayer.Info.HasAudio())
                {
                    if (_mediaPlayer.Control.IsPlaying())
                    {
                        if (_overlayManager)
                        {
                            _overlayManager.TriggerFeedback(CustomOverlayManager.Feedback.Pause);
                        }
                        _isAudioFadingUpToPlay = false;
                    }
                    else
                    {
                        _isAudioFadingUpToPlay = true;
                        Play();
                    }
                    _audioFadeTime = 0f;
                }
                else
                {
                    if (_mediaPlayer.Control.IsPlaying())
                    {
                        Pause();
                    }
                    else
                    {
                        Play();
                    }
                }
            }
        }

        private void Play()
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                if (_overlayManager)
                {
                    _overlayManager.TriggerFeedback(CustomOverlayManager.Feedback.Play);
                }
                _mediaPlayer.Play();
            }
        }

        private void Pause(bool skipFeedback = false)
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                if (!skipFeedback)
                {
                    if (_overlayManager)
                    {
                        _overlayManager.TriggerFeedback(CustomOverlayManager.Feedback.Pause);
                    }
                }
                _mediaPlayer.Pause();
            }
        }

        public void SeekRelative(float deltaTime)
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                TimeRange timelineRange = GetTimelineRange();
                double time = _mediaPlayer.Control.GetCurrentTime() + deltaTime;
                time = System.Math.Max(time, timelineRange.startTime);
                time = System.Math.Min(time, timelineRange.startTime + timelineRange.duration);
                _mediaPlayer.Control.Seek(time);

                if (_overlayManager)
                {
                    _overlayManager.TriggerFeedback(deltaTime > 0f ? CustomOverlayManager.Feedback.SeekForward : CustomOverlayManager.Feedback.SeekBack);
                }
            }
        }

        public void ChangeAudioVolume(float delta)
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                // Change volume
                _audioVolume = Mathf.Clamp01(_audioVolume + delta);

                // Trigger the overlays
                if (_overlayManager)
                {
                    _overlayManager.TriggerFeedback(delta > 0f ? CustomOverlayManager.Feedback.VolumeUp : CustomOverlayManager.Feedback.VolumeDown);
                }
            }
        }

        public void ToggleMute()
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                if (_mediaPlayer.AudioMuted)
                {
                    MuteAudio(false);
                }
                else
                {
                    MuteAudio(true);
                }
            }
        }

        private void MuteAudio(bool mute)
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                // Change mute
                _mediaPlayer.AudioMuted = mute;

                // Update the UI
                // The UI element is constantly updated by the Update() method

                // Trigger the overlays
                if (_overlayManager)
                {
                    _overlayManager.TriggerFeedback(mute ? CustomOverlayManager.Feedback.VolumeMute : CustomOverlayManager.Feedback.VolumeUp);
                }
            }
        }

        private void BuildOptionsMenu()
        {
            // Temporary code for now disables to touch controls while the debug menu
            // is shown, to stop it consuming mouse input for IMGUI
            _videoTouch.enabled = !_showOptions;
        }

        private void ApplyAudioVolume()
        {
            if (_mediaPlayer)
            {
                _mediaPlayer.AudioVolume = (_audioVolume * _audioFade);
            }
        }

        private void UpdateAudioSpectrum()
        {
            bool showAudioSpectrum = false;
#if !UNITY_IOS || UNITY_EDITOR
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                AudioSource audioSource = _mediaPlayer.AudioSource;
                if (audioSource && _audioSpectrumMaterial)
                {
                    showAudioSpectrum = true;

                    float maxFreq = (Helper.GetUnityAudioSampleRate() / 2);

                    // Frequencies over 18Khz generally aren't very interesting to visualise, so clamp the range
                    const float clampFreq = 18000f;
                    int sampleRange = Mathf.FloorToInt(Mathf.Clamp01(clampFreq / maxFreq) * _spectrumSamples.Length);

                    // Add new samples and smooth the samples over time
                    audioSource.GetSpectrumData(_spectrumSamples, 0, FFTWindow.BlackmanHarris);

                    // Find the maxValue sample for normalising with
                    float maxValue = -1.0f;
                    for (int i = 0; i < sampleRange; i++)
                    {
                        if (_spectrumSamples[i] > maxValue)
                        {
                            maxValue = _spectrumSamples[i];
                        }
                    }

                    // Chase maxValue to zero
                    _maxValue = Mathf.Lerp(_maxValue, 0.0f, Mathf.Clamp01(2.0f * Time.deltaTime));

                    // Update maxValue
                    _maxValue = Mathf.Max(_maxValue, maxValue);
                    if (_maxValue <= 0.01f)
                    {
                        _maxValue = 1f;
                    }

                    // Copy and smooth the spectrum values
                    for (int i = 0; i < sampleRange; i++)
                    {
                        float newSample = _spectrumSamples[i] / _maxValue;
                        _spectrumSamplesSmooth[i] = Mathf.Lerp(_spectrumSamplesSmooth[i], newSample, Mathf.Clamp01(15.0f * Time.deltaTime));
                    }

                    // Update shader
                    _audioSpectrumMaterial.SetFloatArray(_propSpectrum.Id, _spectrumSamplesSmooth);
                    _audioSpectrumMaterial.SetFloat(_propSpectrumRange.Id, (float)sampleRange);
                }
            }
#endif
        }

        private void OnTimeSliderBeginDrag()
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                _wasPlayingBeforeTimelineDrag = _mediaPlayer.Control.IsPlaying();
                if (_wasPlayingBeforeTimelineDrag)
                {
                    _mediaPlayer.Pause();
                }
                OnTimeSliderDrag();
            }
        }

        private void OnTimeSliderDrag()
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                TimeRange timelineRange = GetTimelineRange();
                double time = timelineRange.startTime + (_sliderTime.value * timelineRange.duration);
                _mediaPlayer.Control.Seek(time);
                _isHoveringOverTimeline = true;
            }
        }

        private void OnTimeSliderEndDrag()
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                if (_wasPlayingBeforeTimelineDrag)
                {
                    _mediaPlayer.Play();
                    _wasPlayingBeforeTimelineDrag = false;
                }
            }
        }

        private TimeRange GetTimelineRange()
        {
            if (_mediaPlayer.Info != null)
            {
                return Helper.GetTimelineRange(_mediaPlayer.Info.GetDuration(), _mediaPlayer.Control.GetSeekableTimes());
            }
            return new TimeRange();
        }

        private bool CanHideControls()
        {
            bool result = true;
            if (!_autoHide)
            {
                result = false;
            }
#if (!ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER)
            else if (Input.mousePresent)
            {
                // Check whether the mouse cursor is over the controls, in which case we can't hide the UI
                RectTransform rect = _controlsGroup.GetComponent<RectTransform>();
                Vector2 canvasPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, null, out canvasPos);

                Rect rr = RectTransformUtility.PixelAdjustRect(rect, null);
                result = !rr.Contains(canvasPos);
            }
#endif
            return result;
        }

        private void UpdateControlsVisibility()
        {
            if (!_controlsGroup) return;
            if (UserInteraction.IsUserInputThisFrame() || !CanHideControls())
            {
                UserInteraction.InactiveTime = 0f;
                FadeUpControls();
            }
            else
            {

                UserInteraction.InactiveTime += Time.unscaledDeltaTime;
                if (UserInteraction.InactiveTime >= _userInactiveDuration)
                {
                    FadeDownControls();
                }
                else
                {
                    FadeUpControls();
                }
            }
        }

        private void FadeUpControls()
        {
            if (!_controlsGroup.gameObject.activeSelf)
            {
                _controlsGroup.gameObject.SetActive(true);
            }
            _controlsFade = Mathf.Min(1f, _controlsFade + Time.deltaTime * 8f);
            _controlsGroup.alpha = Mathf.Pow(_controlsFade, 5f);
        }

        private void FadeDownControls()
        {
            if (_controlsGroup.gameObject.activeSelf)
            {
                _controlsFade = Mathf.Max(0f, _controlsFade - Time.deltaTime * 3f);
                _controlsGroup.alpha = Mathf.Pow(_controlsFade, 5f);
                if (_controlsGroup.alpha <= 0f)
                {
                    _controlsGroup.gameObject.SetActive(false);
                }
            }
        }

        void Update()
        {
            if (isPlaying && isNeedCloseLoading)
            {
                closeLoadingTime += Time.unscaledDeltaTime;

                if (closeLoadingTime >= delayLoadingTime)
                {
                    isNeedCloseLoading = false;
                    closeLoadingTime = 0;
                    SetLoading(false);
                }
            }

            if (isPlaying && Time.timeScale == 1f)
            {
                FreezeFunction();
            }

            if (!_mediaPlayer) return;

            UpdateControlsVisibility();
            UpdateAudioFading();
            UpdateAudioSpectrum();

            if (_mediaPlayer.Info != null)
            {
                TimeRange timelineRange = GetTimelineRange();

                // Update timeline hover popup
#if (!ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER)
                if (_timelineTip != null)
                {
                    if (_isHoveringOverTimeline)
                    {
                        Vector2 canvasPos;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransform, Input.mousePosition, null, out canvasPos);

                        _timelineTip.gameObject.SetActive(true);
                        Vector3 mousePos = _canvasTransform.TransformPoint(canvasPos);

                        _timelineTip.position = new Vector2(mousePos.x, _timelineTip.position.y);

                        if (UserInteraction.IsUserInputThisFrame())
                        {
                            // Work out position on the timeline
                            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(this._sliderTime.GetComponent<RectTransform>());
                            float x = Mathf.Clamp01((canvasPos.x - bounds.min.x) / bounds.size.x);

                            double time = (double)x * timelineRange.Duration;

                            // Seek to the new position
                            if (_thumbnailMediaPlayer != null && _thumbnailMediaPlayer.Control != null)
                            {
                                _thumbnailMediaPlayer.Control.SeekFast(time);
                            }

                            // Update time text
                            Text hoverText = _timelineTip.GetComponentInChildren<Text>();
                            if (hoverText != null)
                            {
                                time -= timelineRange.startTime;
                                time = System.Math.Max(time, 0.0);
                                time = System.Math.Min(time, timelineRange.Duration);
                                hoverText.text = Helper.GetTimeString(time, false);
                            }
                        }
                    }
                    else
                    {
                        _timelineTip.gameObject.SetActive(false);
                    }
                }
#endif

                // Updated stalled display
                if (_overlayManager)
                {
                    _overlayManager.Reset();
                    if (_mediaPlayer.Info.IsPlaybackStalled())
                    {
                        _overlayManager.TriggerStalled();
                    }
                }

                // Update keyboard input
                if (_enableKeyboardControls)
                {
#if (!ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER)
                    // Keyboard toggle play/pause
                    if (Input.GetKeyDown(KeyTogglePlayPause))
                    {
                        TogglePlayPause();
                    }

                    // Keyboard seek 5 seconds
                    if (Input.GetKeyDown(KeyJumpBack))
                    {
                        SeekRelative(-_jumpDeltaTime);
                    }
                    else if (Input.GetKeyDown(KeyJumpForward))
                    {
                        SeekRelative(_jumpDeltaTime);
                    }

                    // Keyboard control volume
                    if (Input.GetKeyDown(KeyVolumeUp))
                    {
                        ChangeAudioVolume(_keyVolumeDelta);
                    }
                    else if (Input.GetKeyDown(KeyVolumeDown))
                    {
                        ChangeAudioVolume(-_keyVolumeDelta);
                    }

                    // Keyboard toggle mute
                    if (Input.GetKeyDown(KeyToggleMute))
                    {
                        ToggleMute();
                    }
#endif
                }

                // Animation play/pause button
                if (_playPauseMaterial != null)
                {
                    float t = _playPauseMaterial.GetFloat(_propMorph.Id);
                    float d = 1f;
                    if (_mediaPlayer.Control.IsPlaying())
                    {
                        d = -1f;
                    }
                    t += d * Time.deltaTime * 6f;
                    t = Mathf.Clamp01(t);
                    _playPauseMaterial.SetFloat(_propMorph.Id, t);
                }

                // Animation volume/mute button
                if (_volumeMaterial != null)
                {
                    float t = _volumeMaterial.GetFloat(_propMute.Id);
                    float d = 1f;
                    if (!_mediaPlayer.AudioMuted)
                    {
                        d = -1f;
                    }
                    t += d * Time.deltaTime * 6f;
                    t = Mathf.Clamp01(t);
                    _volumeMaterial.SetFloat(_propMute.Id, t);
                    _volumeMaterial.SetFloat(_propVolume.Id, _audioVolume);
                }

                // Animation subtitles button
                if (_subtitlesMaterial)
                {
                    float t = _subtitlesMaterial.GetFloat(_propMorph.Id);
                    float d = 1f;
                    if (_mediaPlayer.TextTracks.GetActiveTextTrack() == null)
                    {
                        d = -1f;
                    }
                    t += d * Time.deltaTime * 6f;
                    t = Mathf.Clamp01(t);
                    _subtitlesMaterial.SetFloat(_propMorph.Id, t);
                }

                // Animation options button
                if (_optionsMaterial)
                {
                    float t = _optionsMaterial.GetFloat(_propMorph.Id);
                    float d = 1f;
                    if (!_showOptions)
                    {
                        d = -1f;
                    }
                    t += d * Time.deltaTime * 6f;
                    t = Mathf.Clamp01(t);
                    _optionsMaterial.SetFloat(_propMorph.Id, t);
                }

                // Update time slider position
                if (_sliderTime && !_isHoveringOverTimeline)
                {
                    double t = 0.0;
                    if (timelineRange.duration > 0.0)
                    {
                        t = ((_mediaPlayer.Control.GetCurrentTime() - timelineRange.startTime) / timelineRange.duration);
                    }
                    _sliderTime.value = Mathf.Clamp01((float)t);
                }

                if (_textDuration && _textVedioLength)
                {
                    string t1 = Helper.GetTimeString((_mediaPlayer.Control.GetCurrentTime() - timelineRange.startTime), false);
                    string d1 = Helper.GetTimeString(timelineRange.duration, false);

                    _textDuration.text = t1;
                    _textVedioLength.text = d1;
                }

                // Update progress segment
                if (_timeLineProgress && !_wasPlayingBeforeTimelineDrag)
                {
                    _sliderTime.value = 0;

                    TimeRanges times = _mediaPlayer.Control.GetBufferedTimes();
                    if (times.Count > 0 && timelineRange.Duration > 0.0)
                    {
                        //double x1 = (times.MinTime - timelineRange.startTime) / timelineRange.duration;
                        double x2 = ((_mediaPlayer.Control.GetCurrentTime() - timelineRange.startTime) / timelineRange.duration);

                        // 取得進度的百分比
                        float progressPercentage = Mathf.Min(1f, (float)x2);
                        // 計算當前的寬度
                        float currentWidth = _timeLineArea.sizeDelta.x * progressPercentage;
                        // 設置_timeLineProgress的寬度
                        _timeLineProgress.sizeDelta = new Vector2(currentWidth, _timeLineProgress.sizeDelta.y);
                    }
                }

                if (_wasPlayingBeforeTimelineDrag)
                {
                    _timeLineProgress.sizeDelta = new Vector2(0, _timeLineProgress.sizeDelta.y);
                }
            }
        }

        void OnGUI()
        {
            // NOTE: These this IMGUI is just temporary until we implement the UI using uGUI
            if (!_showOptions) return;
            if (!_mediaPlayer || _mediaPlayer.Control == null) return;

            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(2f, 2f, 1f));

            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical(GUI.skin.box);
            GUI.backgroundColor = Color.white;

            GUILayout.Label("Duration " + _mediaPlayer.Info.GetDuration() + "s");
            GUILayout.BeginHorizontal();
            GUILayout.Label("States: ");
            GUILayout.Toggle(_mediaPlayer.Control.HasMetaData(), "HasMetaData", GUI.skin.button);
            GUILayout.Toggle(_mediaPlayer.Control.IsPaused(), "Paused", GUI.skin.button);
            GUILayout.Toggle(_mediaPlayer.Control.IsPlaying(), "Playing", GUI.skin.button);
            GUILayout.Toggle(_mediaPlayer.Control.IsBuffering(), "Buffering", GUI.skin.button);
            GUILayout.Toggle(_mediaPlayer.Control.IsSeeking(), "Seeking", GUI.skin.button);
            GUILayout.Toggle(_mediaPlayer.Control.IsFinished(), "Finished", GUI.skin.button);
            GUILayout.EndHorizontal();

            {
                TimeRanges times = _mediaPlayer.Control.GetBufferedTimes();
                if (times != null)
                {
                    GUILayout.Label("Buffered Range " + times.MinTime + " - " + times.MaxTime);
                }
            }
            {
                TimeRanges times = _mediaPlayer.Control.GetSeekableTimes();
                if (times != null)
                {
                    GUILayout.Label("Seek Range " + times.MinTime + " - " + times.MaxTime);
                }
            }
            {
                GUILayout.Label("Video Tracks: " + _mediaPlayer.VideoTracks.GetVideoTracks().Count);

                GUILayout.BeginVertical();

                VideoTrack selectedTrack = null;
                foreach (VideoTrack track in _mediaPlayer.VideoTracks.GetVideoTracks())
                {
                    bool isSelected = (track == _mediaPlayer.VideoTracks.GetActiveVideoTrack());
                    if (isSelected) GUI.color = Color.green;
                    if (GUILayout.Button(track.DisplayName, GUILayout.ExpandWidth(false)))
                    {
                        selectedTrack = track;
                    }
                    if (isSelected) GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();
                if (selectedTrack != null)
                {
                    _mediaPlayer.VideoTracks.SetActiveVideoTrack(selectedTrack);
                }
            }
            {
                GUILayout.Label("Audio Tracks: " + _mediaPlayer.AudioTracks.GetAudioTracks().Count);

                GUILayout.BeginVertical();

                AudioTrack selectedTrack = null;
                foreach (AudioTrack track in _mediaPlayer.AudioTracks.GetAudioTracks())
                {
                    bool isSelected = (track == _mediaPlayer.AudioTracks.GetActiveAudioTrack());
                    if (isSelected) GUI.color = Color.green;
                    if (GUILayout.Button(track.DisplayName, GUILayout.ExpandWidth(false)))
                    {
                        selectedTrack = track;
                    }
                    if (isSelected) GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();
                if (selectedTrack != null)
                {
                    _mediaPlayer.AudioTracks.SetActiveAudioTrack(selectedTrack);
                }
            }
            {
                GUILayout.Label("Text Tracks: " + _mediaPlayer.TextTracks.GetTextTracks().Count);

                GUILayout.BeginVertical();

                TextTrack selectedTrack = null;
                foreach (TextTrack track in _mediaPlayer.TextTracks.GetTextTracks())
                {
                    bool isSelected = (track == _mediaPlayer.TextTracks.GetActiveTextTrack());
                    if (isSelected) GUI.color = Color.green;
                    if (GUILayout.Button(track.DisplayName, GUILayout.ExpandWidth(false)))
                    {
                        selectedTrack = track;
                    }
                    if (isSelected) GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();
                if (selectedTrack != null)
                {
                    _mediaPlayer.TextTracks.SetActiveTextTrack(selectedTrack);
                }
            }
            {
                GUILayout.Label("FPS: " + _mediaPlayer.Info.GetVideoDisplayRate().ToString("F2"));
            }
#if (UNITY_STANDALONE_WIN)
            if (_mediaPlayer.PlatformOptionsWindows.bufferedFrameSelection != BufferedFrameSelectionMode.None)
            {
                IBufferedDisplay bufferedDisplay = _mediaPlayer.BufferedDisplay;
                if (bufferedDisplay != null)
                {
                    BufferedFramesState state = bufferedDisplay.GetBufferedFramesState();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Buffered Frames: " + state.bufferedFrameCount);
                    GUILayout.HorizontalSlider(state.bufferedFrameCount, 0f, 12f);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Free Frames: " + state.freeFrameCount);
                    GUILayout.HorizontalSlider(state.freeFrameCount, 0f, 12f);
                    GUILayout.EndHorizontal();
                    GUILayout.Label("Min Timstamp: " + state.minTimeStamp);
                    GUILayout.Label("Max Timstamp: " + state.maxTimeStamp);
                    GUILayout.Label("Display Timstamp: " + _mediaPlayer.TextureProducer.GetTextureTimeStamp());
                }
            }
#endif
            GUILayout.EndVertical();
        }

        [SerializeField] Text _textDuration = null;
        [SerializeField] Text _textVedioLength = null;
        [SerializeField] RectTransform _timeLineProgress = null;
        [SerializeField] RectTransform _timeLineArea = null;
        [SerializeField] GameObject loadingAni = null;
        //[SerializeField] Animator animator = null;

        private Action FreezeFunction = null;

        private bool isPlaying = false;
        // 計算關閉loading延遲
        private bool isNeedCloseLoading = false;
        private float closeLoadingTime = 0;
        private float delayLoadingTime;

        public void SetFreezeFunction(Action action)
        {
            FreezeFunction = action;
        }

        public void OpenMedia(string urlPath)
        {
            ResetUI();
            if (!_mediaPlayer) return;

            SetLoading(true);
            isPlaying = true;
            isNeedCloseLoading = false;
            _mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, urlPath);
        }

        private void SetLoading(bool active)
        {
            if (loadingAni != null)
            {
                loadingAni.SetActive(active);
            }
        }

        private void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
        {
            // 影片已載入完成
            if (et == MediaPlayerEvent.EventType.FirstFrameReady)
            {
                // 延遲0.1秒關閉loading
                isNeedCloseLoading = true;
                closeLoadingTime = 0;
                //SetLoading(false);
            }
            // 影片已播放完畢
            if (et == MediaPlayerEvent.EventType.FinishedPlaying)
            {
                isPlaying = false;
                _mediaPlayer.Control.Stop();
                //_overlayManager.TriggerFeedback(OverlayManager.Feedback.Replay);

                DialogManager.HideDialog("Dia_MediaPlayer");
            }

        }

        public void PauseMedia()
        {
            if (!_mediaPlayer) return;

            _mediaPlayer.Control.Pause();
        }

        public void ResumeMedia()
        {
            if (!_mediaPlayer) return;

            _mediaPlayer.Control.Play();
        }


        public void CloseMedia()
        {
            ResetUI();
            if (!_mediaPlayer) return;

            _mediaPlayer.Control.Stop();
            _mediaPlayer.CloseMedia();
        }

        protected void CreateTimelineDragEvents()
        {
            EventTrigger trigger = _sliderTime.gameObject.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((data) => { OnTimeSliderBeginDrag(); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.Drag;
                entry.callback.AddListener((data) => { OnTimeSliderDrag(); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener((data) => { OnTimeSliderEndDrag(); });
                trigger.triggers.Add(entry);
            }
        }

        void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
            {
                _mediaPlayer.Control.Play();
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (_mediaPlayer.PauseMediaOnAppPause)
                {
                    _mediaPlayer.Control.Pause();
                }
            }
            else
            {
                if (_mediaPlayer.PlayMediaOnAppUnpause)
                {
                    OnApplicationFocus(true);
                }
            }
        }

        protected void CreateVideoTouchEvents()
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((data) => { OnCustomVideoPointerUp(); });
            _videoTouch.triggers.Add(entry);
        }

        private void OnCustomVideoPointerUp()
        {
            // 重播
            //if (isPlayFinish)
            //{
            //    if (_mediaPlayer.Info != null)
            //    {
            //        _mediaPlayer.Control.Rewind();
            //        isPlayFinish = false;
            //    }
            //}

            //TogglePlayPause();
        }

        private void ResetUI()
        {
            isPlaying = false;
            _sliderTime.value = 0;
            _textDuration.text = "00:00";
            _textVedioLength.text = "00:00";
            _overlayManager.TriggerFeedback(CustomOverlayManager.Feedback.Play);
            _timeLineProgress.sizeDelta = new Vector2(0, _timeLineProgress.sizeDelta.y);
        }
    }
}
#endif