using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ship/Modules/Passive")]
public class PassiveModuleAsset : InstallableModuleAsset {

	[Space(10)]
	[Header("Assets")]
	/** The mesh to use for the component. */
	public Mesh mesh;
}
