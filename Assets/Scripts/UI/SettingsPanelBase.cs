using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SettingsPanelBase : Mono<SettingsPanelBase>
{
    [Header("Panel")]
    public GameObject settingsPanel;


    public void TogglePanel()
    {
        this.settingsPanel.SetActive(!this.settingsPanel.activeSelf);
    }
}
