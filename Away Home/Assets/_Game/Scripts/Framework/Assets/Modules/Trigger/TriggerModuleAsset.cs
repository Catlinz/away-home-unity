using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerModuleAsset : InstallableModuleAsset
{

    /// <summary>The amount of energy required to be enabled / installed.</summary>
    [Space(10)]
    [Header("Trigger Stats")]
    public int triggerEnergyUse;

    /// <summary>The number of seconds between firing/triggering the module.</summary>
    public float cooldownSec;
}
