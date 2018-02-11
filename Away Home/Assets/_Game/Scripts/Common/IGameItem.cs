using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameItem {

	/** The displayed name for the GameItem. */
	string Name { get; }

	/** The displayed Type for the GameItem. */
	string Type { get; }

	/** The volume (in m^3) for the GameItem. */
	float Volume { get; }

	/** Return this item as a MonoBehaviour, if possible. */
	MonoBehaviour AsPrefab();

	/** Return this item as a ScriptableObject, if possible. */
	ScriptableObject AsScriptable();

	/** Create a new InventoryItem from the GameItem. */
	InventoryItem CreateInventoryItem(InventoryItem item = null);
}
