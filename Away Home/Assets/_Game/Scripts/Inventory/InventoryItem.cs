using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem {
	/** Max stack size for an inventory item. */
	public static readonly uint MaxStackSize = 1000000;

	#region FIELDS
	/** The item that is being stored.  Can be a prefab or a ScriptableObject. */
	public UnityEngine.Object _item = null;

	/** The number of items stored in this inventory item stack. */
	public uint count = 0;

	/** The volume per item in the stack. */
	public float unitVolume = 0.0f;

	/** The mass per item in the stack. */
	public float unitMass = 0.0f;

	/** The monetary value of each item in the stack. */
	public ulong unitValue = 0;

	/** The current 'health' of the item as a percentage.  
	 * -1 indicates the item cannot be damaged. */
	public float health = -1;

	/** The number of items that can be stored in the stack. */
	public uint Capacity {
		get { return MaxStackSize - count; }
	}

	/** Whether or not this stack is full. */
	public bool IsFull {
		get { return count >= MaxStackSize; }
	}

	/** Whether or not this InventoryItem is valid. */
	public virtual bool IsValid {
		get { return count > 0 && _item != null; }
	}

	/** The stored item as a IGameItem */
	public IGameItem Item {
		get { return _item as IGameItem; }
	}

	public string Name {
		get { return Item.Name; }
	}

	/** The total mass of the inventory stack. */
	public float TotalMass {
		get { return unitMass * (float)count; }
	}

	/** The total volume of the inventory stack. */
	public float TotalVolume {
		get { return unitVolume * (float)count; }
	}

	/** The type of the item as a string. */
	public string Type {
		get { return Item.Type; }
	}
	#endregion

	#region METHODS
	/// <summary>Try and get the item as a specified type. */
	public T As<T>() where T : UnityEngine.Object {
		T obj = _item as T;
		Debug.Assert(obj != null, "Inventory.As<"+obj.GetType().ToString()+">() returned null!");
		return obj;
	}

	/// <summary> Test to see if two InventoryItems can be combined into a stack.</summary>
	public bool CanStackWith(InventoryItem other) {
		return 	(other._item == this._item) && 
				(other.health == this.health);
	}

	/// <summary> Return a new InventoryItem that is a clone of this one, except for count.</summary>
	public InventoryItem Clone() {
		InventoryItem stack = InventoryItem.Create();
		stack.health = health;
		stack._item = _item;
		stack.unitValue = unitValue;
		stack.unitVolume = unitVolume;
		stack.unitMass = unitMass;
		return stack;
	}

	/// <summary>Create and return a new InventoryItem, with optional count.</summary>
	public static InventoryItem Create(uint count = 0) {
		InventoryItem stack = new InventoryItem();
		stack.count = count;
		return stack;
	}

	/// <summary>Create and return a new InventoryItem from a GameItem, with optional count.</summary>
	public static InventoryItem Create(IGameItem item, uint count = 0) {
		InventoryItem stack = item.CreateInventoryItem();
		stack.count = count;
		return stack;
	}

	/// <summary>Return true if the item is of the specified type. */
	public bool Is<T>() where T : UnityEngine.Object {
		T obj = _item as T;
		return (obj != null);
	}
	#endregion

	#region SORTING FUNCTIONS
	public static int SortByCountAsc(InventoryItem a, InventoryItem b) {
		return (int)(a.count - b.count);
	}
	public static int SortByCountDsc(InventoryItem a, InventoryItem b) {
		return (int)(b.count - a.count);
	}
	public static int SortByNameAsc(InventoryItem a, InventoryItem b) {
		return a.Name.CompareTo(b.Name);
	}
	public static int SortByNameDsc(InventoryItem a, InventoryItem b) {
		return b.Name.CompareTo(a.Name);
	}
	public static int SortByTypeAsc(InventoryItem a, InventoryItem b) {
		return a.Type.CompareTo(b.Type);
	}
	public static int SortByTypeDsc(InventoryItem a, InventoryItem b) {
		return b.Type.CompareTo(a.Type);
	}
	public static int SortByUnitValueAsc(InventoryItem a, InventoryItem b) {
		if (a.unitValue == b.unitValue) { return 0; }
		return (a.unitValue < b.unitValue) ? -1 : 1;
	}
	public static int SortByUnitValueDsc(InventoryItem a, InventoryItem b) {
		if (a.unitValue == b.unitValue) { return 0; }
		return (a.unitValue < b.unitValue) ? 1 : -1;
	}
	#endregion
}
