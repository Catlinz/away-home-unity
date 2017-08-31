using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallableModuleAsset : ScriptableObject {

    [Header("Type")]
    /** The prefab that includes the script and model data */
    public GameObject prefab;

    [Space(10)]
    [Header("Stats")]
    /** The amount of CPU required to be installed. */
    public float idleCpuUsage;

    /** The amount of power required to be installed. */
    public float idlePowerUsage;
}
