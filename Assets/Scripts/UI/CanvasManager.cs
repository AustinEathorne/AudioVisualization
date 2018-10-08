﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoSingleton<CanvasManager>
{
    [Header("Song")]
    public Dropdown songDropdown;

    [Header("SidePanels")]
    public GameObject visualizationPanel;
    public GameObject libraryPanel;
    public List<SettingsPanelBase> settingsPanelList;

    [Header("SideButtons")]
    public float sideButtonFadeTime;
    public float sideButtonMoveTime;
    public List<Vector2> sideButtonPositions;
    public List<RectTransform> sideButtonTransforms;
    public List<Image> sideButtonImages;

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

    [Header("Escape Container")]
    public GameObject escapeContainer;

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
        this.StartCoroutine(this.ToggleSettingsPanel());
    }

    public void OnVisualizationClick()
    {
        this.StartCoroutine(this.ToggleVisualizationPanel());
    }

    public void OnLibraryClick()
    {
        this.StartCoroutine(this.ToggleLibraryPanel());
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

    public void OnVisualizationSelectClick(int _index)
    {
        DemoManager.Instance.StartCoroutine(DemoManager.Instance.ChangeVisualization(_index));
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

    public void OnQuitClick()
    {
        DemoManager.Instance.StartCoroutine(DemoManager.Instance.Quit());
    }

    public void OnSongSelectClick()
    {

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


    public void ToggleEscapeContainer()
    {
        this.escapeContainer.SetActive(!this.escapeContainer.activeSelf);
    }


    public IEnumerator ToggleSettingsPanel()
    {
        if (this.settingsPanelList[DemoManager.Instance.currentVisualization].settingsPanel.activeSelf)
        {
            this.settingsPanelList[DemoManager.Instance.currentVisualization].TogglePanel();
            yield return this.StartCoroutine(this.ShiftSideButtons(0, false));
        }
        else
        {
            yield return this.StartCoroutine(this.ShiftSideButtons(0, true));
            this.settingsPanelList[DemoManager.Instance.currentVisualization].TogglePanel();
        }

        yield return null;
    }

    public IEnumerator ToggleVisualizationPanel()
    {
        if (this.visualizationPanel.activeSelf)
        {
            this.visualizationPanel.SetActive(false);
            yield return this.StartCoroutine(this.ShiftSideButtons(1, false));
        }
        else
        {
            yield return this.StartCoroutine(this.ShiftSideButtons(1, true));
            this.visualizationPanel.SetActive(true);
        }

        yield return null;
    }

    public IEnumerator ToggleLibraryPanel()
    {
        if (this.libraryPanel.activeSelf)
        {
            this.libraryPanel.SetActive(false);
            yield return this.StartCoroutine(this.ShiftSideButtons(2, false));
        }
        else
        {
            yield return this.StartCoroutine(this.ShiftSideButtons(2, true));
            this.libraryPanel.SetActive(true);
        }

        yield return null;
    }

    public IEnumerator ShiftSideButtons(int _selectedButton, bool _isOpeningPanel)
    {
        if (_isOpeningPanel)
        {
            // Fade out not selected buttons
            for (int i = 0; i < this.sideButtonImages.Count; i++)
            {
                if (i == _selectedButton)
                    continue;

                if (i == this.sideButtonImages.Count - 1)
                    yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.sideButtonImages[i], this.sideButtonFadeTime, 0.0f));
                else
                    UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.sideButtonImages[i], this.sideButtonFadeTime, 0.0f));
            }

            // move selected button to selected position
            yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.MoveTransformOverTime(this.sideButtonTransforms[_selectedButton], this.sideButtonPositions[0], this.sideButtonMoveTime));
        }
        else
        {
            // Move selected button back to original position
            yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.MoveTransformOverTime(this.sideButtonTransforms[_selectedButton], this.sideButtonPositions[_selectedButton], this.sideButtonMoveTime));

            // Fade in not selected buttons
            for (int i = 0; i < this.sideButtonImages.Count; i++)
            {
                if (i == _selectedButton)
                    continue;

                if (i == this.sideButtonImages.Count - 1)
                    yield return UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.sideButtonImages[i], this.sideButtonFadeTime, 1.0f));
                else
                    UIUtility.Instance.StartCoroutine(UIUtility.Instance.FadeOverTime(this.sideButtonImages[i], this.sideButtonFadeTime, 1.0f));
            }
        }

        yield return null;
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
