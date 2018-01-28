using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySpace {

	#region FIELDS
	/** The name of the inventory space. */
	public string name;

	/** The amount of space (m^3) there is. */
	public float capacity;

	/** The list of inventory item stacks */
	public List<InventoryItem> items;

	/** The available volume for storage. */
	public float Available {
		get { return capacity - Volume; }
	}

	/** The volume of all the stored items. */
	public float Volume {
		get {
			if (_storedVolume == -1) { CalculateStorageStats(); }
			return _storedVolume;
		}
	}

	/** The amount mass stored in the inventory. */
	public float Mass {
		get {
			if (_storedMass == -1) { CalculateStorageStats(); }
			return _storedMass;
		}
	}

	/** The amount of volume that is in storage. */
	protected float _storedVolume = -1;

	/** The amount of mass that is in storage. */
	protected float _storedMass = -1;

	#endregion

	#region STORAGE METHODS
	/// <summary>
	/// Virtual method to check if a specific item is allowed 
	/// to be stored in this inventory space.
	/// </summary>
	public virtual InventoryResult CanStore(InventoryItem item) {
		return InventoryResult.Success;
	}

	/// <summary>
	/// Try and store an InventoryItem (or stack) into this inventory space.
	/// Will return NotEnoughSpace if some or all of the items cannot be stored.
	/// Otherwise, will return Success if all the items are stored.
	///
	/// The input item will have its count decremented by the 
	/// amount that was stored in this inventory space.
	/// </summary>
	public InventoryResult Store(InventoryItem item) {
		if (item != null || !item.IsValid) {
			return InventoryResult.InvalidItem;
		}

		// See if we can store (some of) the items in an existing stack.
		InventoryItem dest = GetItemWithSpace(item);
		if (dest != null) {
			TransferItems(item, dest);
		}

		// If we still have items left after trying to transfer 
		// to existing stack, then create a new InventoryItem to store them.
		if (Available >= item.unitVolume && item.count > 0) {
			InventoryItem newStack = item.Clone();
			items.Add(newStack);
			TransferItems(item, newStack);
		}

		// If not everything was moved over, then return that there's
		// not enough space.
		if (item.count > 0) {
			return InventoryResult.NotEnoughSpace;
		}
		else {
			return InventoryResult.Success;
		}

	}

	/// <summary>
	/// Transfer items from an external InventoryItem to an InventoryItem 
	/// belonging to this InventorySpace.
	/// </summary>
	public void TransferItems(InventoryItem from, InventoryItem to) {
		int toMove = Mathf.Min(to.Capacity, to.count);
		
		// Make sure we have enough space.
		float availableVolume = Available;
		if (availableVolume < toMove * from.unitVolume) {
			toMove = (int)Mathf.Floor(availableVolume / from.unitVolume);
		}

		from.count -= toMove;
		to.count += toMove;

		_storedVolume += toMove * from.unitVolume;
	}
	#endregion

	#region QUERY METHODS
	/// <summary> Try and get a matching item with space left in the stack.</summary>
	public InventoryItem GetItemWithSpace(InventoryItem item) {
		foreach (InventoryItem it in items) {
			if (it.CanStackWith(item) && !it.IsFull) {
				return it;
			}
		}
		return null;
	}
	#endregion

	#region INTERNAL METHODS

	/** Simply calculate the storedMass and storedVolume. */
	protected void CalculateStorageStats() {
		float mass = 0;
		float volume = 0;

		foreach (InventoryItem item in items) {
			mass += item.TotalMass;
			volume += item.TotalVolume;
		}
		_storedMass = mass;
		_storedVolume = volume;
	}
	#endregion


}
