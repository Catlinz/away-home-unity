using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveModuleAsset : InstallableModuleAsset {

	/// <summary>The amount of CPU resources required to be enabled / installed.</summary>
	[Space(10)]
	[Header("Active Stats")]
	public int activeCpuUsage;

	/// <summary>The amount of energy required to be enabled / installed.</summary>
	public int activeEnergyDrain;
}
