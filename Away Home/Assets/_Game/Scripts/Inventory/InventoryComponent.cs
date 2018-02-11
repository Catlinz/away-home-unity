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

	#region STORAGE METHODS
	/// <summary>
	/// Try and store the specified number of the item into the inventories.
	/// Starts with the main inventory, then tries to store in all other inventories.
	/// </summary>
	public InventoryResult Store(InventoryItem item, uint count = 0) {
		if (count != 0) {
			Debug.Assert(item.count == 0, "[InventoryComponent] Overwriting count of InventoryItem with count > 0 on Store!");
			item.count = count; 
		}

		InventoryResult result = InventoryResult.ItemNotAllowed;
		foreach (InventorySpace space in stores) {
			if (space.CanStore(item.Item)) {
				if ((result = space.Store(item)) != InventoryResult.NotEnoughSpace) {
					break;
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Try and store the items into the specified inventory space.
	/// </summary>
	public InventoryResult StoreIn(string name, InventoryItem item, uint count = 0) {
		if (count != 0) {
			Debug.Assert(item.count == 0, "[InventoryComponent] Overwriting count of InventoryItem with count > 0 on StoreIn!");
			item.count = count; 
		}

		InventorySpace space = FindSpaceByName(name);
		if (space != null) {
			return space.Store(item);
		}
		else {
			return InventoryResult.InvalidSpace;
		}
	}
	/// <summary>
	/// Try and take the specified number of items from the Inventories, starting 
	/// with the main inventory, then searching all others until we got enough 
	/// items, or we run out of inventories.
	/// </summary>
	/// <returns>An inventory item with the number of items taken.</returns>
	public InventoryItem Take(IGameItem item, uint count = 1) {
		InventoryItem ret = item.CreateInventoryItem();

		foreach (InventorySpace space in stores) {
			space.Take(ret, count - ret.count);
			if (ret.count >= count) { break; }
		}
		
		return ret;
	}

	/// <summary>
	/// Try and take the specified number of items from a specific inventory space.
	/// </summary>
	/// <returns>An InventoryItem, with number of items taken (0 if none).</returns>
	public InventoryItem TakeFrom(string name, IGameItem item, uint count = 1) {
		InventorySpace space = FindSpaceByName(name);
		if (space != null) {
			return space.Take(item, count);
		}
		else {
			return item.CreateInventoryItem(); /* Creates empty inventory item. */
		}
	}
	#endregion

	#region QUERY METHODS
	/// <summary>Get the total number of `item` stored in all inventories.</summary>
	public ulong Count(IGameItem item) {
		ulong count = 0;
		foreach (InventorySpace space in stores) {
			count += space.Count(item);
		}
		return count;
	}

	/// <summary>Get the total number of `item` in the specified InventorySpace.</summary>
	public ulong CountIn(string name, IGameItem item) {
		InventorySpace space = FindSpaceByName(name);
		if (space != null) {
			return space.Count(item);
		}
		else { return 0; }
	}


	/// <summary>
	/// Get the number of the specified item that can be stored 
	/// in this InventoryComponent.  Looks at all the spaces that 
	/// can hold the item.
	/// </summary>
	public ulong GetSpaceFor(IGameItem item) {
		ulong count = 0;
		foreach (InventorySpace space in stores) {
			if (space.CanStore(item)) {
				count += space.GetSpaceFor(item);
			}
		}
		return count;
	}
	#endregion

	#region SPACE METHODS
	/// <summary>
	/// Adds a new InventorySpace to the list of spaces.
	/// If an InventorySpace already exists with the provided name, it will 
	/// return `SpaceAlreadyExists`, otherwise will return `Success`.
	/// </summary>
	public InventoryResult AddSpace(string name, float capacity) {
		if (FindSpaceByName(name) != null) {
			return InventoryResult.SpaceAlreadyExists;
		}

		InventorySpace space = new InventorySpace(name, capacity);
		stores.Add(space);

		// Make sure if there are any InventoryViews active, then we 
		// update them with the new InventorySpace.
		foreach (InventoryView view in _views) {
			if (view.Active) {
				view.Add(space);
			}
		}

		return InventoryResult.Success;
	}

	/// <summary>
	/// Remove and returns the InventorySpace with the provided name.
	/// </summary>
	public InventorySpace RemoveSpace(string name) {
		InventorySpace space = FindSpaceByName(name);
		if (space != null) {
			stores.Remove(space);
		}

		// Make sure the InventorySpace is removed from all 
		// InventoryViews.
		foreach (InventoryView view in _views) {
			view.Remove(space);
		}

		return space;
	}

	/// <summary>Get the InventorySpace with the provided name.</summary>
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
	[HideInInspector]
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
