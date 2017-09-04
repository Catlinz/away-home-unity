using UnityEngine;
using UnityEditor;

//TODO: Clean this up.

/// <summary>
/// Custom property drawer for drawing ArmorSystem into the inspector GUI.
/// </summary>
[CustomPropertyDrawer(typeof(ArmorSystem))]
public class ArmorSystemDrawer : PropertyDrawer {

	/// <summary>
	/// Returns the height required by the ShipSocket for rendering into the Inspector GUI.
	/// </summary>
	/// <param name="property">The property being drawn.</param>
	/// <param name="label">The label for the property.</param>
	/// <returns>The height the property needs to draw.</returns>
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		if (property.isExpanded) {
			float baseHeight = (EditorGUIUtility.singleLineHeight * 7) + EditorGUIUtility.standardVerticalSpacing * 7;
			if (property.FindPropertyRelative("damageResistance").isExpanded) {
				return baseHeight + (EditorGUIUtility.singleLineHeight * 4) + (EditorGUIUtility.standardVerticalSpacing * 4);
			}
			else { 
				return baseHeight;
			}
		}
		else {
			return base.GetPropertyHeight(property, label);
		}
	}

	/// <summary>
	/// Draws the ArmorSystem property into the inspector GUI.
	/// </summary>
	/// <param name="position">The rect we can draw into.</param>
	/// <param name="property">The serialized ModifiableFloat.</param>
	/// <param name="label">The label for the ModifiableFloat.</param>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		label = EditorGUI.BeginProperty(position, label, property);

		// Set the height back to a single line height, not the entire height of the property.
		position.height = EditorGUIUtility.singleLineHeight;
		// Set the indent level to 1.
		int oldIndentLevel = EditorGUI.indentLevel;

		// Draw the foldout lable for the ArmorSystem.
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);

		// Draw the rest if the foldout is... folded out.
		if (property.isExpanded) {
			EditorGUI.indentLevel += 1;

			// Draw the armor resistance property.
			position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
			SerializedProperty resists = property.FindPropertyRelative("damageResistance");
			resists.isExpanded = EditorGUI.Foldout(position, resists.isExpanded, resists.displayName, true);
			if (resists.isExpanded) {
				EditorGUI.PropertyField(position, resists, true);
				position.y += (position.height + EditorGUIUtility.standardVerticalSpacing) * 4;
			}

			// Draw the armor faces.
			SerializedProperty armor = property.FindPropertyRelative("armor");
			position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.LabelField(position, "Armor Strength", EditorStyles.boldLabel);
			Rect buttonRect = new Rect(position);
			buttonRect.x = 130;
			buttonRect.width = 16;
			if (GUI.Button(buttonRect, "R", EditorStyles.miniButton)) {
				ResetArmorFaces(armor);
			}
			EditorGUI.indentLevel += 1;

			// Draw the Left armor face.
			position = DrawArmorFace(ArmorSystem.Face.Left, armor, position);
			position = DrawArmorFace(ArmorSystem.Face.Right, armor, position);
			position = DrawArmorFace(ArmorSystem.Face.Front, armor, position);
			position = DrawArmorFace(ArmorSystem.Face.Rear, armor, position);
		
		}


		// Set indent back to what it was
		EditorGUI.indentLevel = oldIndentLevel;

		EditorGUI.EndProperty();
	}

	private Rect DrawArmorFace(ArmorSystem.Face faceEnum, SerializedProperty faces, Rect position) {
		SerializedProperty face = faces.GetArrayElementAtIndex((int)faceEnum);

		// Set up the rectangle.
		float prevWidth = position.width;
		float prevX = position.x;
		position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
		position.width = 170;
	

		float prevMax = face.FindPropertyRelative("maximum").floatValue;
		EditorGUI.BeginChangeCheck();
		float newMax = EditorGUI.FloatField(position, faceEnum.ToString(), prevMax);
		if (EditorGUI.EndChangeCheck()) {
			face.FindPropertyRelative("maximum").floatValue = newMax;
			face.FindPropertyRelative("current").floatValue += (newMax - prevMax);
		}
		position.x += position.width + 5;
		position.width = 80;
		int prevIndentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		EditorGUI.LabelField(position, string.Format("({0})", face.FindPropertyRelative("current").floatValue));
		EditorGUI.indentLevel = prevIndentLevel;
		position.x = prevX;
		position.width = prevWidth;
		return position;
	}

	private void ResetArmorFaces(SerializedProperty faces) {
		for (int i = 0; i < 4; ++i) {
			SerializedProperty face = faces.GetArrayElementAtIndex(i);
			face.FindPropertyRelative("current").floatValue = face.FindPropertyRelative("maximum").floatValue;
		}
	}
}
