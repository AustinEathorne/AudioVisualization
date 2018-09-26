using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoSingleton<CanvasManager>
{
    [Header("Song")]
    public Dropdown songDropdown;

    [Header("Search Bar")]
    public Slider searchBar;
    public bool isSearching;

    [Header("Visualization")]
    public Dropdown visualizationDropdown;

    [Header("Settings Panels")]
    public List<SettingsPanelBase> settingsPanelList;



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

    #endregion

    #region UI

    public IEnumerator SetupUI()
    {
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

        // Settings panel
        yield return this.settingsPanelList[0].StartCoroutine(this.settingsPanelList[0].Initialize());

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

    #endregion
}
