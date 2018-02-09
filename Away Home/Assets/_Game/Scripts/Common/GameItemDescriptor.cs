using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameItemDescriptor", menuName = "Game/Items/GameItemDescriptor", order = 1)]
public class GameItemDescriptor : ScriptableObject {

	#region FIELDS
	[Header("Item Properties")]
	/// <summary>The volume in m^3 of the item.</summary>
	public float volume;

	/// <summary>The mass, in Kg, of the item.</summary>
	public float mass;

	/// <summary>The base value of the item, in credits<summary>
	public ulong value;

	[Header("Item Information")]
	/// <summary>A human readable (and searchable) type of the item.</summary>
	public string type;

	/// <summary>A description of the item</summary>
	public string description;
	#endregion
}
