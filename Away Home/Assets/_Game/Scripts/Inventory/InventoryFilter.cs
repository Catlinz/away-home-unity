using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryFilter {

    /// <summary>
    /// Filter a list of InventoryItems in place.
    /// </summary>
    public void Filter(List<InventoryItem> items) {
        items.RemoveAll(delegate (InventoryItem it) {
            return !Test(it);
        });
    }

    /// <summary>
    /// Filter a list of InventoryItems and store that list into `store`.
    /// If `store` is null or not provided, a new list will be returned, otherwise, 
    /// we will return `store`.
    /// </summary>
    public List<InventoryItem> Filtered(List<InventoryItem> source, List<InventoryItem> store = null) {
        // Setup `store` to store the filtered items.
        if (store == null) { store = new List<InventoryItem>(source.Count); }
        else { store.Clear(); }

        // Filter each item.
        foreach (InventoryItem item in source) {
            if (Test(item)) {
                store.Add(item);
            }
        }
        return store;
    }
    
    /// <summary>
    /// Test whether or not a single InventoryItem passes the filter.
    /// </summary>
    public bool Test(InventoryItem item) {
        return (item != null && item.IsValid);
    }
}
