using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SettingsPanelBase : Mono<SettingsPanelBase>
{
    [Header("Panel")]
    public GameObject settingsPanel;
    public CanvasGroup canvasGroup;

    public void TogglePanel()
    {
        this.settingsPanel.SetActive(!this.settingsPanel.activeSelf);
        this.canvasGroup.interactable = !this.canvasGroup.interactable;
        this.canvasGroup.blocksRaycasts = !this.canvasGroup.blocksRaycasts;
    }
}
