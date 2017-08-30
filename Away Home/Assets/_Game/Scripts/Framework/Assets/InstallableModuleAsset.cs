using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallableModuleAsset : ScriptableObject {

    [Header("Type")]
    /** The name of the MonoBehaviour Module script to create */
    public string behaviourScript;

    [Space(10)]
    [Header("Stats")]
    /** The amount of CPU required to be installed. */
    public float idleCpuUsage;

    /** The amount of power required to be installed. */
    public float idlePowerUsage;

	/** Get the type of script to pass to AddComponent(type). */
	public System.Type GetScriptType() {
		var type = System.Type.GetType(behaviourScript);
		if (type == null || !typeof(Component).IsAssignableFrom(type)) { 
			return null;
		}
		else {
			return type;
		}
	}
}
