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
    /// Filter a list of InventoryItems and return a new list with the 
    /// filtered items.
    /// </summary>
    public List<InventoryItem> Filtered(List<InventoryItem> items) {
        List<InventoryItem> newList = new List<InventoryItem>(items);
        Filter(newList);
        return newList;
    }
    
    /// <summary>
    /// Test whether or not a single InventoryItem passes the filter.
    /// </summary>
    public bool Test(InventoryItem item) {
        return (item != null && item.IsValid);
    }
}
