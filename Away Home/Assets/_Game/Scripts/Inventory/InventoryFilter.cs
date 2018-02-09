using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryFilter {

    public enum Type {
        None = 0, Name, Type
    }

    [System.Serializable]
    public struct FilterValue {
        public string stringValue;
    }

    #region FIELDS
    /** The property to filter the items by. */
    public Type filterType;

    /** The value to filter the items by. */
    public FilterValue filterValue;

    private System.Func<InventoryItem, FilterValue, bool> _filter = null;
    #endregion

    public InventoryFilter(Type type, string value) {
        filterType = type;
        filterValue.stringValue = value;
    }

    /// <summary>
    /// Filter a list of InventoryItems in place.
    /// </summary>
    public void Filter(List<InventoryItem> items) {
        if (_filter == null) { SetFilterMethod(); }
        items.RemoveAll(delegate (InventoryItem it) {
            return !_filter(it, filterValue);
        });
    }

    /// <summary>
    /// Filter a list of InventoryItems and store that list into `store`.
    /// If `store` is null or not provided, a new list will be returned, otherwise, 
    /// we will return `store`.
    /// </summary>
    public List<InventoryItem> Filtered(List<InventoryItem> source, List<InventoryItem> store = null) {
        if (_filter == null) { SetFilterMethod(); }
        // Setup `store` to store the filtered items.
        if (store == null) { store = new List<InventoryItem>(source.Count); }
        else { store.Clear(); }

        // Filter each item.
        foreach (InventoryItem item in source) {
            if (_filter(item, filterValue)) {
                store.Add(item);
            }
        }
        return store;
    }

    #region INTERNAL METHODS
    private void SetFilterMethod() {
        switch (this.filterType) {
            case Type.Name:
                _filter = FilterByName;
                break;
            case Type.Type:
                _filter = FilterByType;
                break;
            default:
                _filter = null;
                break;
        }
    }
    #endregion

    #region FILTER FUNCTIONS
    public static bool FilterByName(InventoryItem item, FilterValue value) {
        return item.Name.IndexOf(value.stringValue, System.StringComparison.OrdinalIgnoreCase) != -1;
    }
    public static bool FilterByType(InventoryItem item, FilterValue value) {
        return item.Type.IndexOf(value.stringValue, System.StringComparison.OrdinalIgnoreCase) != -1;
    }
    #endregion
}
