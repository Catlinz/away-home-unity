using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for any asset for an Installable Ship Module such as 
/// a turret or a passive armour booster.
/// </summary>
public class InstallableModuleAsset : ScriptableObject {

    /// <summary>The prefab that is used to mount the module on the ship.</summary>
    [Header("Type")]
    public GameObject prefab;

    /// <summary>The amount of CPU resources required to be enabled / installed.</summary>
    [Space(10)]
    [Header("Stats")]
    public int idleCpuUsage;

    /// <summary>The amount of power required to be enabled / installed.</summary>
    public int idlePowerUsage;
}
