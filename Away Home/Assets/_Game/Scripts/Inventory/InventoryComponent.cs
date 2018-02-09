using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : MonoBehaviour, ISerializationCallbackReceiver {

	#region FIELDS
	/** The list of inventory spaces to use for storing items. */
	public List<InventorySpace> stores = new List<InventorySpace>();

	/** The actual list of Inventory views for displaying and filtering items. */
	private List<InventoryView> _views = new List<InventoryView>();
	#endregion

	#region SPACE METHODS
	public InventorySpace FindSpaceByName(string name) {
		foreach(InventorySpace space in stores) {
			if (space.name == name) {
				return space;
			}
		}
		return null;
	}
	#endregion

	#region SERIALIZATION
	/** A list that helps with (de)serializing the inventory component. */
	public List<SerializableInventoryView> serializedViews = new List<SerializableInventoryView>();

	public void OnBeforeSerialize() {
		serializedViews.Clear();
		foreach (InventoryView view in _views) {
			serializedViews.Add(new SerializableInventoryView(view));
		}
	}

	public void OnAfterDeserialize() {
		_views.Clear();

		foreach (SerializableInventoryView data in serializedViews) {
			InventoryView view = new InventoryView(data.name, data.spaces.Length);
			
			// Add all the spaces to the view.
			for (int i = 0; i < data.spaces.Length; ++i) {
				InventorySpace space = FindSpaceByName(data.spaces[i]);
				if (space != null) {
					view.Add(space);
				}
			}

			// Add all the filters to the view.
			for (int i = 0; i < data.filters.Length; ++i) {
				view.AddFilter(data.filters[i]);
			}

			// Set the sorting method.
			view.Sorting = data.sortBy;
		}

		serializedViews.Clear();
	}

	#endregion
	
}
