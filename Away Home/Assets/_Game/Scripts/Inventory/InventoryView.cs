using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InventoryViewSet {
    public InventorySpace space;
    private List<InventoryItem> _items;

    /// <summary>
    /// Apply an InventoryFilter to all the items in the InventorySpace.
    /// </summary>
    public void Apply(InventoryFilter filter, int index) {
        Debug.Assert(space != null, "Cannot filter null InventorySpace!");
        // For the first filter, fill the list from the InventorySpace.
        if (index == 0) {
            _items = filter.Filtered(space.items, _items);
        }
        else { // On subsequent filtering, just filter the already filtered items.
            filter.Filter(_items);
        }
    }

    public void Clear() {
        space = null;
        _items = null;
    }

    public List<InventoryItem> Get() {
        if (_items != null) {
            return _items;
        }
        else {
            return (space != null) ? space.items : null;
        }
    }

    public void Reset(bool toNull = false) {
        if (toNull) { 
            _items = null; 
        }
        else if (_items != null) {
            _items.Clear();
        }
    }

    public void Set(InventorySpace space) {
        this.space = space;
        _items = null;
    }

    public void Sort(System.Comparison<InventoryItem> compare) {
        if (_items == null) {
            Debug.Assert(space != null, "Cannot sort null InventorySpace!");
            _items = new List<InventoryItem>(space.items);
        }
        _items.Sort(compare);
    }
}

public class InventoryView {

    /** Enum to indicate how the InventoryView should sort the results. */
    public enum SortBy {
        None, Type, Count, UnitValue
    };

    #region FIELDS
    /** If true, then regenerate on filter or sorting change. */
    public bool Active {get; set;}

    /** The number of Sets in this view. */
    public int Count {
        get { return _count; }
    }

    public string name;

    /** The Array of inventory view result sets. */
    public InventoryViewSet[] Sets {
        get { return _sets; }
    }

    /** The method used to sort the inventory view results. */
    public SortBy Sorting { 
        get { return _sorting; }
        set { 
            if (_sorting != value) {
                _sorting = value;
                if (Active) { SortAll(); }
            }
        }
     }

    private InventoryViewSet[] _sets;
    private List<InventoryFilter> _filters;
    
    /** The number of InventorySpaces assigned to the view. */
    private int _count;

    /** How to sort the InventoryView results. */
    private SortBy _sorting = SortBy.None;

    #endregion

    public InventoryView(string name, int numSets = 1) {
        this.name = name;
        
        _filters = new List<InventoryFilter>();
        _sets = new InventoryViewSet[numSets];
        Active = false;
        Clear();
    }

    /// <summary>Indexer to get the specified set at the index</summary>
    public InventoryViewSet this[int i] {
        get { return _sets[i]; }
    }

    /// <summary>
    /// Adds a new InventorySpace to the list of spaces in this 
    /// InventoryView.
    /// </summary>
    public void Add(InventorySpace space) {
        int index = -1;
        for (int i = 0; i < _sets.Length; ++i) {
            if (_sets[i].space == null) {
                _sets[i].Set(space);
                index = i;
                break;
            }
        }

        if (index != -1) {
            index = _sets.Length;
            // Otherwise, have to grow array.  This should happen rarely, if ever.
            Debug.LogWarning("Growing array in InventoryView from ["+index+"]");
            _sets = AHArray.Added(_sets, new InventoryViewSet());
            _sets[index].Set(space);
        }

        ++_count;

        if (Active) {
            Filter(_sets[index]);
            Sort(_sets[index]);
        }
    }

    /// <summary>Clear out all the sets from the view.</summary>
    public void Clear() {
        // Clear out all the items.
        for (int i = 0; i < _sets.Length; ++i) {
            _sets[i].Clear();
        }
        _count = 0;

        // Clear out the filters.
        _filters.Clear();
        _sorting = SortBy.None;
    }

    /// <summary>
    /// Generates the set of InventoryItems for the InventorySpaces
    /// based on the current set of filters.
    /// </summary>
    public void Generate() {    
        // Reset, filter and sort each of the sets.
        for (int i = 0; i < _count; ++i) {
            _sets[i].Reset();
            Filter(_sets[i]);
            Sort(_sets[i]);
        }
    }

    /// <summary>
    /// Remove an InventorySpace from the view.
    /// </summary>
    public void Remove(InventorySpace space) {
        int index = -1;
        for (int i = 0; i < _sets.Length; ++i) {
            if (_sets[i].space == space) {
                index = i;  break;
            }
        }

        if (index != -1) {
            _sets[index].Clear();
            if (index != _sets.Length - 1 && _sets[index + 1].space != null) {
                for (int i = index + 1; i < _sets.Length; ++i) {
                    _sets[i - 1] = _sets[i];
                    _sets[i].Clear();
                }
            }
            --_count;
        }
    }

    /// <summary>Reset the view to empty and no filters.</summary>
    public void Reset() {
        // Reset all the items.
        for (int i = 0; i < _sets.Length; ++i) {
            _sets[i].Reset(true);
        }

        // Clear out the filters.
        _filters.Clear();
        // Clear out any sorting.
        _sorting = SortBy.None;
    }

    #region INTERNAL METHODS

    /** Filter an InventoryViewSet by each of the stored filters. */
    private void Filter(InventoryViewSet set) {
        int index = 0;
        foreach (InventoryFilter filter in _filters) {
            set.Apply(filter, index++);
        }
    }

    /** Sort an InventoryViewSet by the current sorting method. */
    private void Sort(InventoryViewSet set) {
        switch(_sorting) {
            case SortBy.Count:
                set.Sort(InventoryItem.SortByCountDsc);
                break;
            case SortBy.Type:
                set.Sort(InventoryItem.SortByTypeAsc);
                break;
            case SortBy.UnitValue:
                set.Sort(InventoryItem.SortByUnitValueDsc);
                break;
        }
    }

    private void SortAll() {
        if (_sorting == SortBy.None) { Generate(); }
        else {
            for (int i = 0; i < _count; ++i) {
                Sort(_sets[i]);
            }
        }
    }
    #endregion
}
