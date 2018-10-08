using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoSingleton<CanvasManager>
{
    [Header("Song")]
    public Dropdown songDropdown;

    [Header("Visualization")]
    public Dropdown visualizationDropdown;

    [Header("Settings Panels")]
    public List<SettingsPanelBase> settingsPanelList;

    [Header("Search Bar")]
    public Slider searchBar;
    public bool isSearching;

    [Header("Play Button")]
    public Image playButtonImage;
    public List<Sprite> playButtonSprites;

    [Header("Time Text")]
    public Text timeText;

    [Header("Volume")]
    public float[] volumeImageThresholds = new float[3];
    public List<Sprite> volumeSpriteList;
    public Image volumeImage;
    public Slider volumeSlider;

    #region Main

    public override IEnumerator Initialize()
    {
        this.isInitialized = false;

        yield return this.StartCoroutine(this.SetupUI());

        this.isInitialized = true;
    }

    public override IEnumerator Run()
    {
        this.isRunning = true;

        while (this.isRunning)
        {
            // update search bar if we're not searching
            if (!isSearching)
            {
                this.searchBar.value = AudioManager.Instance.audioSource.time;
            }

            this.SetTimeText();

            yield return null;
        }

        yield return null;
    }

    public override IEnumerator Stop()
    {
        this.isRunning = false;
        yield return null;
    }

    #endregion

    #region Events

    public void OnSettingsClick()
    {
        this.ToggleSettingsPanel();
    }

    public void OnSongDropdownUpdate(int _index)
    {
        AudioManager.Instance.ChangeSong(_index);
        this.ResetSearchBar();
    }

    public void OnSearchBarSelect()
    {
        this.StartCoroutine(this.SearchRoutine());
    }

    public void OnSearchBarRelease()
    {
        this.isSearching = false;
    }

    public void OnVisualizationDropdownUpdate(int _index)
    {
        DemoManager.Instance.StartCoroutine(DemoManager.Instance.ChangeVisualization(_index));
    }

    public void OnChannelSelect(int _channel)
    {
        AudioPeer.Instance.channel = (Channel)_channel;
        AudioManager.Instance.SetStereoPan((Channel) _channel);
    }

    public void OnPlayClick()
    {
        if (AudioManager.Instance.audioSource.isPlaying)
        {
            AudioManager.Instance.PauseMusic();
        }
        else
        {
            AudioManager.Instance.PlayMusic();
        }

        this.SetPlayButtonSprite();
    }

    public void OnVolumeClick()
    {
        AudioManager.Instance.audioSource.mute = !AudioManager.Instance.audioSource.mute;
        this.SetVolumeSprite();
    }

    public void OnVolumeUpdate(float _value)
    {
        // Update volume
        AudioManager.Instance.SetVolume(_value);

        // Update volume image
        this.SetVolumeSprite();
    }

    #endregion

    #region UI

    public IEnumerator SetupUI()
    {
        // Visualization dropdown
        this.visualizationDropdown.value = DemoManager.Instance.currentVisualization;

        // Song dropdown
        List<Dropdown.OptionData> dataList = new List<Dropdown.OptionData>();

        foreach (AudioClip clip in AudioManager.Instance.songs)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = clip.name;

            dataList.Add(data);            
        }

        this.songDropdown.AddOptions(dataList);

        // Search bar
        this.searchBar.maxValue = AudioManager.Instance.songs[0].length;
        this.searchBar.value = 0.0f;

        // Volume
        this.volumeSlider.value = AudioManager.Instance.audioSource.volume;
        this.SetVolumeSprite();

        // Settings panel
        yield return this.settingsPanelList[DemoManager.Instance.currentVisualization].StartCoroutine(this.settingsPanelList[DemoManager.Instance.currentVisualization].Initialize());

        yield return null;
    }

    public IEnumerator SearchRoutine()
    {
        if (this.isSearching)
            yield break;

        this.isSearching = true;

        AudioManager.Instance.PauseMusic();

        while (this.isSearching)
        {
            AudioManager.Instance.Search(this.searchBar.value);

            yield return null;
        }

        AudioManager.Instance.PlayMusic();

        yield return null;
    }

    public void ResetSearchBar()
    {
        this.searchBar.maxValue = AudioManager.Instance.songs[this.songDropdown.value].length;
        this.searchBar.value = 0.0f;
    }

    public void ToggleSettingsPanel()
    {
        this.settingsPanelList[this.visualizationDropdown.value].TogglePanel();
    }

    public IEnumerator ChangeVisualization(int _index)
    {
        this.settingsPanelList[DemoManager.Instance.currentVisualization].StartCoroutine(
            this.settingsPanelList[DemoManager.Instance.currentVisualization].Stop());

        this.settingsPanelList[_index].StartCoroutine(this.settingsPanelList[_index].Initialize());

        yield return null;
    }

    public void SetVolumeSprite()
    {
        // Check if the audio is muted
        if (AudioManager.Instance.audioSource.mute)
        {
            this.volumeImage.sprite = this.volumeSpriteList[0];
            return;
        }

        float volume = AudioManager.Instance.audioSource.volume;
        for (int i = 0; i < this.volumeImageThresholds.Length; i++)
        {
            if (volume <= this.volumeImageThresholds[i])
            {
                this.volumeImage.sprite = this.volumeSpriteList[i + 1]; // 0 is volume mute
                i = this.volumeImageThresholds.Length;
            }
        }
    }

    public void SetPlayButtonSprite()
    {
        if (AudioManager.Instance.audioSource.isPlaying)
        {
            this.playButtonImage.sprite = this.playButtonSprites[1];
        }
        else
        {
            this.playButtonImage.sprite = this.playButtonSprites[0];
        }
    }

    public void SetTimeText()
    {
        float time = AudioManager.Instance.audioSource.time;
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);

        this.timeText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    #endregion
}
