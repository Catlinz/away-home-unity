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
	public string name = null;

	/** The list of inventory item stacks */
	public List<InventoryItem> items = new List<InventoryItem>();

	/** The amount of space (m^3) there is. */
	public float capacity = 0.0f;

	/** Whether or not this inventory space can be accessed. */
	public bool isAccessible = true;

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

	public InventorySpace(string name, float capacity) {
		this.name = name;
		this.capacity = capacity;
	}

	#region STORAGE METHODS
	/// <summary>
	/// Try and store an InventoryItem (or stack) into this inventory space.
	/// Will return NotEnoughSpace if some or all of the items cannot be stored.
	/// Otherwise, will return Success if all the items are stored.
	///
	/// The input item will have its count decremented by the 
	/// amount that was stored in this inventory space.
	/// </summary>
	public InventoryResult Store(InventoryItem item) {
		if (!isAccessible) { return InventoryResult.SpaceNotAccessible; }
		if (item != null || !item.IsValid) { return InventoryResult.InvalidItem; }

		// See if we can store (some of) the items in an existing stack.
		InventoryItem dest = GetItemWithSpace(item);
		if (dest != null) {
			TransferItems(item, dest);
		}

		// If we still have items left after trying to transfer 
		// to existing stack, then create a new InventoryItem to store them.
		while (Available >= item.unitVolume && item.count > 0) {
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

	/// <summary>Try and store one or more IGameItems into the inventory.</summary>
	/// See <see cref="InventorySpace.Store(InventoryItem)"/> for more details.
	public InventoryResult Store(IGameItem item, uint count = 1) {
		InventoryItem toStore = item.CreateInventoryItem();
		toStore.count = count;
		return Store(toStore);
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
    public InventoryItem Take(InventoryItem itemStack, uint count = 1) {
		if (!isAccessible) { return itemStack; }

        // List of item stacks to hold potential sources.
        List<InventoryItem> stacks = new List<InventoryItem>(Mathf.FloorToInt((float)count / InventoryItem.MaxStackSize) + 1);
        
        // Loop through the inventory to find the stacks.
        foreach (InventoryItem item in items) {
            if (item._item == itemStack._item) {
                stacks.Add(item);
            }
        }

        // If none, return empty inventory item.
        if (stacks.Count == 0 || count == 0) {
            return itemStack;
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

        // Now take items from the stacks, starting with the smallest stack.
        foreach (InventoryItem stack in stacks) {
            if (itemStack.CanStackWith(stack)) {
                TransferItemsOut(stack, itemStack, count);
                if (itemStack.count >= count) { break; }
            }
        }

        return itemStack;
    }

	/// See <see cref="InventorySpace.Take(InventoryItem, uint)">
	public InventoryItem Take(IGameItem item, uint count = 1) {
		InventoryItem newItem = item.CreateInventoryItem();
		return Take(newItem, count);
	}


    /// <summary>
	/// Transfer items from an external InventoryItem to an InventoryItem 
	/// belonging to this InventorySpace.
	/// </summary>
    public uint TransferItemsIn(InventoryItem from, InventoryItem to, uint maxCount = 0) {
        uint transferred = TransferItems(from, to, maxCount);
        _storedVolume += transferred * to.unitVolume;
        _storedMass += transferred * to.unitMass;
        return transferred;
    }

    /// <summary>
	/// Transfer items from an InventoryItem in this inventory space, to a different 
    /// InventoryItem, not in this inventory space.
	/// </summary>
    public uint TransferItemsOut(InventoryItem from, InventoryItem to, uint maxCount = 0) {
        uint transferred = TransferItems(from, to, maxCount);
        _storedVolume -= transferred * from.unitVolume;
        _storedMass -= transferred * from.unitMass;

        if (from.count == 0) {
            items.Remove(from);
        }
        return transferred;
    }
	#endregion

	#region QUERY METHODS
	/// <summary>
	/// Virtual method to check if a specific item is allowed 
	/// to be stored in this inventory space.
	/// </summary>
	public virtual bool CanStore(IGameItem item) {
		return isAccessible;
	}

	/// <summary>Get the total number of the items stored.</summary>
	public ulong Count(IGameItem item) {
		ulong count = 0;
		if (isAccessible) {
			foreach (InventoryItem it in items) {
				if (it.Item == item) {
					count += it.count;
				}
			}
		}
		return count;
	}

	/// <summary> Try and get a matching item with space left in the stack.</summary>
	public InventoryItem GetItemWithSpace(InventoryItem item) {
		if (isAccessible) {
			foreach (InventoryItem it in items) {
				if (it.CanStackWith(item) && !it.IsFull) {
					return it;
				}
			}
		}
		return null;
	}

	/// <summary>Return the amount of items of this type that can be stored.</summary>
	public ulong GetSpaceFor(IGameItem item) {
		if (isAccessible) {
			float volume = item.Volume;
			return (ulong)Mathf.Floor(capacity / volume);
		}
		else {
			return 0;
		}
		
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
        return (int)(a.count - b.count);
    }

    /** Move items from one InventoryItem stack to another. */
    private uint TransferItems(InventoryItem from, InventoryItem to, uint maxCount = 0) {
        if (maxCount == 0 || maxCount > from.count) {
            maxCount = from.count;
        }
        uint toMove = (to.Capacity < maxCount) ? to.Capacity : maxCount;

        // Make sure we have enough space.
        float availableVolume = Available;
        if (availableVolume < toMove * from.unitVolume) {
            toMove = (uint)Mathf.Floor(availableVolume / from.unitVolume);
        }

        from.count -= toMove;
        to.count += toMove;

        return toMove;
    }
    #endregion


}
