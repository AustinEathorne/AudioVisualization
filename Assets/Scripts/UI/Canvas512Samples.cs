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

    public override IEnumerator Initialize()
    {
        yield return this.StartCoroutine(this.SetupUI());
    }

    public override IEnumerator Run()
    {
        yield return null;
    }

    public override IEnumerator Stop()
    {
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
    }

    #endregion

    #region UI

    public IEnumerator SetupUI()
    {
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

        List<Dropdown.OptionData> dataList = new List<Dropdown.OptionData>();

        foreach (AudioClip clip in AudioManager.Instance.songs)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = clip.name;

            dataList.Add(data);            
        }

        this.songDropdown.AddOptions(dataList);

        yield return null;
    }

    public void ToggleSettingsPanel()
    {
        this.settingsPanel.SetActive(!this.settingsPanel.activeSelf);
    }

    #endregion
}
