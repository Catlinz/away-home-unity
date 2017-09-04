using UnityEngine;

/// <summary>
/// A class that holds the strength and status of the 
/// armor for an object.
/// </summary>
[System.Serializable]
public class ArmorSystem {

	/// <summary>The four different armor faces.</summary>
	public enum Face {Left, Right, Front, Rear};

	/// <summary>
	/// A struct that holds the maximum and current values for 
	/// a particular armor face.
	/// </summary>
	[System.Serializable]
	public struct ArmorState {

		/// <summary>The maximum armor hit points for the face.</summary>
		public float maximum;
		/// <summary>The current armor hit points for the face.</summary>
		public float current;

		/// <summary>The percentage [0-1] of damage that goes through the armor.</summary>
		public float PercentPenetration {
			get {
				float p = (current / maximum);
				if (p > 0.75) { return 0.0f; }
				else { return 1.0f - p; }
			}
		}

		/// <summary>
		/// Set the maximum and current to the given value.
		/// </summary>
		/// <param name="max">The maximum number of armor hit points for the face.</param>
		public ArmorState(float max) {
			maximum = max;
			current = max;
		}
	}

	/// <summary>The damage resistance to various damage types.</summary>
	public DamageModifiers damageResistance;

	/// <summary>The armor strength of each face as a percentage [0-100].</summary>
	public ArmorState[] armor = new ArmorState[4];

	public ArmorSystem() {

	}

}
