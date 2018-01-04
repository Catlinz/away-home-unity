using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSystemComponent : MonoBehaviour, ISystem {

	#region DELEGATES
    /// <param name="change">If less than 0, then indicates allocated resource loss, otherwise indicates idle resource gain.</param>
	public delegate void ComputerResourcesChanged(float change);
	/// <summary>Fired when allocated computer resources are lost or gained.</summary>
    public event ComputerResourcesChanged onComputerResourcesChanged;

    #endregion

    #region PRIVATE_FIELDS
    private SystemModifierList _modifiers;
    #endregion

    #region  PUBLIC METHODS
    public void AddModifier(SystemModifier modifier) {
        //TODO Fill this in
        _modifiers.Add(modifier);
    }

    public void RemoveModifier(SystemModifier modifier) {
        // TODO Fill this in.
        _modifiers.Remove(modifier);
    }

    public void ReplaceModifier(SystemModifier modifier) {
        _modifiers.Replace(modifier);
    }
	#endregion

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
