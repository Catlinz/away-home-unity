using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView {

    #region FIELDS
    /** The list of filtered items. */
    public List<InventoryItem> Items {
        get { return _items; }
    }

    private List<InventoryItem> _items;
    private List<InventoryFilter> _filters;
    #endregion

    public InventoryView() {
        _items = null;
        _filters = null;
    }

    /// <summary>Reset the view to empty and no filters.</summary>
    public void Reset() {
        // Clear out all the items.
        _items.Clear();

        // Clear out the filters.
        _filters.Clear();
    }

    public void View(InventorySpace space) {
        if (_filters != null && _filters.Count > 0) {

        }
    }



}
