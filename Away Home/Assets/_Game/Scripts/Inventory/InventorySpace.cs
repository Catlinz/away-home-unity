using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Encapsulates a space in which inventory items can be stored.
/// Can be something like Ammo storage, an extra cargo bay, or
/// just the main inventory space.
/// </summary>
[System.Serializable]
public class InventorySpace {

	#region FIELDS
	/** The name / id of the inventory space. */
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
    /// Method to take out 1 or more items of a specific type from the inventory space.
    /// The method will make sure to take things from the stack that has the least number
    /// of items first.
    /// Returns an InventoryItem with at most count items in it.
    /// </summary>
    /// <param name="toTake"></param>
    /// <param name="count">Max number of items to take, defaults to 1.</param>
    /// <returns></returns>
    public InventoryItem Take(IGameItem toTake, int count = 1) {
        // List of item stacks to hold potential sources.
        List<InventoryItem> stacks = new List<InventoryItem>(Mathf.FloorToInt((float)count / InventoryItem.MaxStackSize) + 1);
        
        // Loop through the inventory to find the stacks.
        foreach (InventoryItem item in items) {
            if (item.item == toTake) {
                stacks.Add(item);
            }
        }

        // If none, return empty inventory item.
        if (stacks.Count == 0 || count == 0) {
            return new InventoryItem();
        }

        // Now sort by item count if we need to.
        if (stacks.Count > 2) {
            stacks.Sort(InventoryItem.SortByCountAsc);
        }
        else if (stacks.Count > 1) {
            if (stacks[0].count > stacks[1].count) {
                stacks.Reverse();
            }
        }

        // Create the new InventoryItem to hold the items.
        InventoryItem newItem = stacks[0].Clone();

        // Now take items from the stacks, starting with the smallest stack.
        foreach (InventoryItem stack in stacks) {
            if (newItem.CanStackWith(stack)) {
                TransferItemsOut(stack, newItem, count);
                if (newItem.count >= count) { break; }
            }
        }

        return newItem;
    }


    /// <summary>
	/// Transfer items from an external InventoryItem to an InventoryItem 
	/// belonging to this InventorySpace.
	/// </summary>
    public int TransferItemsIn(InventoryItem from, InventoryItem to, int maxCount = -1) {
        int transferred = TransferItems(from, to, maxCount);
        _storedVolume += transferred * to.unitVolume;
        _storedMass += transferred * to.unitMass;
        return transferred;
    }

    /// <summary>
	/// Transfer items from an InventoryItem in this inventory space, to a different 
    /// InventoryItem, not in this inventory space.
	/// </summary>
    public int TransferItemsOut(InventoryItem from, InventoryItem to, int maxCount = -1) {
        int transferred = TransferItems(from, to, maxCount);
        _storedVolume -= transferred * from.unitVolume;
        _storedMass -= transferred * from.unitMass;

        if (from.count == 0) {
            items.Remove(from);
        }
        return transferred;
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
	private void CalculateStorageStats() {
		float mass = 0;
		float volume = 0;

		foreach (InventoryItem item in items) {
			mass += item.TotalMass;
			volume += item.TotalVolume;
		}
		_storedMass = mass;
		_storedVolume = volume;
	}

    /** Delegate for List.sort to get stack with least items to take from */
    private static int SortByCountDelegate(InventoryItem a, InventoryItem b) {
        return a.count - b.count;
    }

    /** Move items from one InventoryItem stack to another. */
    private int TransferItems(InventoryItem from, InventoryItem to, int maxCount = -1) {
        if (maxCount == -1 || maxCount > from.count) {
            maxCount = from.count;
        }
        int toMove = Mathf.Min(to.Capacity, maxCount);

        // Make sure we have enough space.
        float availableVolume = Available;
        if (availableVolume < toMove * from.unitVolume) {
            toMove = (int)Mathf.Floor(availableVolume / from.unitVolume);
        }

        from.count -= toMove;
        to.count += toMove;

        return toMove;
    }
    #endregion


}
