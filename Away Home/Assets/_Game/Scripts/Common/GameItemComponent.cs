using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemComponent : MonoBehaviour, IGameItem {

	#region FIELDS
	[Header("Game Item")]
	/// <summary>A human displayable (and searchable) name for the item.</summary>
	public string itemName;

	/// <summary>A ScriptableObject with more information about the item.</summary>
	public GameItemDescriptor info;

	#endregion

	#region GAME ITEM METHODS
	/// <summary>Get the display name for the item.</summary>
	public string Name {
		get { return itemName; }
	}

	/// <summary>Get the display type for the item.</summary>
	public string Type {
		get { return info.type; }
	}

	/// <summary>Get this item as a MonoBehaviour</summary>
	public MonoBehaviour AsPrefab() { return this as MonoBehaviour; }

	/// <summary>Always returns null.</summary>
	public ScriptableObject AsScriptable() { return null; }

	public virtual InventoryItem CreateInventoryItem(InventoryItem item = null) {
		if (item == null) { 
			item = new InventoryItem();
		}
		item.item = this as IGameItem;
		item.unitVolume = info.volume;
		item.unitMass = info.mass;
		item.unitValue = info.value;
		return item;
	}
	#endregion

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
