﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisualizationBase : Mono<VisualizationBase>
{
    [Header("Parent")]
    [SerializeField]
    protected Transform parentTransform;
}
