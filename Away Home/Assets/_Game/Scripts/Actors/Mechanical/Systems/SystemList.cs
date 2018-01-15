using System.Collections;
using System.Collections.Generic;

public class SystemList {

	#region FIELDS

	/// <summary>Are any autoEnable systems disabled?</summary>
	public bool HasDisabledAuto {
		get { return _autoDisabled > 0; }
	}

	private SystemComponent[] _auto = {};
	private SystemComponent[] _manual = {};

	private System.Random _rand = new System.Random();

	private int _autoDisabled = 0;

	#endregion

	#region METHODS
	/// <summary> Nulls the array references, should be used on destruction.</summary>
	public void Clear() {
		_auto = null;
		_manual = null;
		_autoDisabled = 0;
	}

	/// <summary>
	/// Try and disable a random system.  It will look at the manual systems first, 
	/// then disable one of the auto systems if no manual system was active.
	/// </summary>
	public bool DisableRandom() {
		int numManual = _manual.Length;

		if (numManual > 0) {
			int mIndex = _rand.Next(0, numManual);
			for (int i = 0; i < numManual; ++i) {
				// Find the first active autoEnable system.
				if (_manual[mIndex].IsOnline) {
					// deactivate the system.
					_manual[mIndex].Deactivate();
					return true;
				}
				mIndex = (mIndex + 1) % numManual;
			}
		}

		int numAuto = _auto.Length;

		if (_autoDisabled < numAuto) {
			// Choose a random starting point in the list.
			int aIndex = _rand.Next(0, numAuto);

			for (int i = 0; i < numAuto; ++i) {
				// Find the first active autoEnable system.
				if (_auto[aIndex].IsOnline) {
					// deactivate the system.
					_auto[aIndex].Deactivate();
					++_autoDisabled;
					return true;
				}
				aIndex = (aIndex + 1) % numAuto;
			}
		}

		// No systems to deactivate
		return false;
	}

	/// <summary>
	/// Try and enable a random autoEnable system that was previously offline.
	/// </summary>
	public bool EnableRandom() {
		if (_autoDisabled > 0) {
			int numSystems = _auto.Length;

			// Choose a random starting point in the list.
			int index = _rand.Next(0, numSystems);

			for (int i = 0; i < numSystems; ++i) {
				// Find the first inactive system.
				if (_auto[index].autoEnable && !_auto[index].IsOnline) {
					// Try and activate the system.
					if (_auto[index].Activate() == OperationResult.Status.OK) {
						--_autoDisabled;
						return true;
					}
				}
				index = (index + 1) % numSystems;
			}
		}

		// Failed to activate any systems.
		return false;
	}

	/// <summary>
	/// Register a new component into the system list.  If the component has 
	/// autoEnable true, then it will try and enable the component as well.
	/// </summary>
	public void Register(SystemComponent component) {
		if (component.autoEnable) {
			_auto = AHArray.Added(_auto, component);
			if (component.Activate() != OperationResult.Status.OK) {
				++_autoDisabled;
			}
		}
		else {
			_manual = AHArray.Added(_manual, component);
		}
	}
	#endregion
}
