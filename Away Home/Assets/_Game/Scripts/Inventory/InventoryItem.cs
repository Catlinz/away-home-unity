using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem {
	/** Max stack size for an inventory item. */
	public static readonly int MaxStackSize = 1000000;  

	#region FIELDS
	/** The item that is being stored.  Can be a prefab or a ScriptableObject. */
	public UnityEngine.Object item;

	/** The number of items stored in this inventory item stack. */
	public int count = 0;

	/** The volume per item in the stack. */
	public float unitVolume; 

	/** The mass per item in the stack. */
	public float unitMass;

	/** The monetary value of each item in the stack. */
	public float unitValue;

	/** The current 'health' of the item as a percentage.  
	 * -1 indicates the item cannot be damaged. */
	public float health = -1;

	/** The number of items that can be stored in the stack. */
	public int Capacity {
		get { return MaxStackSize - count; }
	}

	/** Whether or not this stack is full. */
	public bool IsFull {
		get { return count >= MaxStackSize; }
	}

	/** Whether or not this InventoryItem is valid. */
	public virtual bool IsValid {
		get { return count > 0 && item != null; }
	}

	/** The total mass of the inventory stack. */
	public float TotalMass {
		get { return unitMass * (float)count; }
	}

	/** The total volume of the inventory stack. */
	public float TotalVolume {
		get { return unitVolume * (float)count; }
	}
	#endregion

	#region METHODS
	/// <summary>Try and get the item as a specified type. */
	public T As<T>() where T : UnityEngine.Object {
		T obj = item as T;
		Debug.Assert(obj != null, "Inventory.As<T>() returned null!");
		return obj;
	}

	/// <summary> Test to see if two InventoryItems can be combined into a stack.</summary>
	public bool CanStackWith(InventoryItem other) {
		return 	(other.item == this.item) && 
				(other.health == this.health);
	}

	/// <summary> Return a new InventoryItem that is a clone of this one, except for count.</summary>
	public InventoryItem Clone() {
		InventoryItem stack = new InventoryItem();
		stack.count = 0;
		stack.health = health;
		stack.item = item;
		stack.unitValue = unitValue;
		stack.unitVolume = unitVolume;
		stack.unitMass = unitMass;
		return stack;
	}

	/// <summary>Return true if the item is of the specified type. */
	public bool Is<T>() where T : UnityEngine.Object {
		T obj = item as T;
		return (obj != null);
	}
	#endregion
}
