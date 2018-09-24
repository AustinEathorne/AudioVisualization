using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas512Samples : CanvasBase
{
    [Header("Vis Manager")]
    public Visualization512Samples visualization;

    [Header("Settings")]
    public GameObject settingsPanel;
    public Slider minScaleSlider;
    public Text minScaleText;
    public Slider maxScaleSlider;
    public Text maxScaleText;
    public Slider scaleMultiplierSlider;
    public Text scaleMultiplierText;

    [Header("Song")]
    public Dropdown songDropdown;

    [Header("Search Bar")]
    public Slider searchBar;
    public bool isSearching;

    public override IEnumerator Initialize()
    {
        yield return this.StartCoroutine(this.SetupUI());
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


    #region Events

    public void OnSettingsClick()
    {
        this.ToggleSettingsPanel();
    }

    public void OnMinScaleUpdate(float _value)
    {
        this.minScaleText.text = _value.ToString();
        this.visualization.SetMinScale(_value);
    }

    public void OnMaxScaleUpdate(float _value)
    {
        this.maxScaleText.text = _value.ToString();
        this.visualization.SetMaxScale(_value);
    }

    public void OnMultiplierUpdate(float _value)
    {
        this.scaleMultiplierText.text = _value.ToString();
        this.visualization.SetScaleMultiplier(_value);
    }

    public void OnSongDropdownUpdate(int _index)
    {
        AudioManager.Instance.ChangeSong(_index);
        this.ResetSearchBar();
    }

    public void OnSearchBarSelect()
    {
        this.StartCoroutine(this.SearchRoutine());
        Debug.Log("Select");
    }

    public void OnSearchBarReleased()
    {
        this.isSearching = false;
        Debug.Log("Release");
    }

    #endregion

    #region UI

    public IEnumerator SetupUI()
    {
        // Settings panel
        this.minScaleSlider.minValue = this.visualization.minMinScale;
        this.minScaleSlider.maxValue = this.visualization.maxMinScale;
        this.minScaleSlider.value = this.visualization.minScale;
        this.minScaleText.text = this.visualization.minScale.ToString();

        this.maxScaleSlider.minValue = this.visualization.minMaxScale;
        this.maxScaleSlider.maxValue = this.visualization.maxMaxScale;
        this.maxScaleSlider.value = this.visualization.maxScale;
        this.maxScaleText.text = this.visualization.maxScale.ToString();

        this.scaleMultiplierSlider.minValue = this.visualization.minScaleMultiplier;
        this.scaleMultiplierSlider.maxValue = this.visualization.maxScaleMultiplier;
        this.scaleMultiplierSlider.value = this.visualization.scaleMultiplier;
        this.scaleMultiplierText.text = this.visualization.scaleMultiplier.ToString();

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
        this.settingsPanel.SetActive(!this.settingsPanel.activeSelf);
    }

    #endregion
}
