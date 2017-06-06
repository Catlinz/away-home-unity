using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallableComponentAsset : ScriptableObject {

    [Header("Type")]
    /** The name of the MonoBehaviour Component script to create */
    public string behaviourScript;

    [Space(10)]
    [Header("Stats")]
    /** The amount of CPU required to be installed. */
    public float requiredCPU;

    /** The amount of power required to be installed. */
    public float requiredPower;



}
