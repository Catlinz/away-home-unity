using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom property drawer for drawing ModifiableFloats into the inspector GUI.
/// </summary>
[CustomPropertyDrawer(typeof(ModifiableFloat))]
public class ModifiableFloatDrawer : PropertyDrawer {

    /// <summary>The label for the button to reset the modifiers.</summary>
    private GUIContent resetLabel = new GUIContent("R", "Reset modifiers");

    /// <summary>
    /// Draws the ModifiableFloat property into the inspector GUI.
    /// </summary>
    /// <param name="position">The rect we can draw into.</param>
    /// <param name="property">The serialized ModifiableFloat.</param>
    /// <param name="label">The label for the ModifiableFloat.</param>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		label = EditorGUI.BeginProperty(position, label, property);

		// Draw label
		Rect contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

        // Draw the control as ([...] + added) x modifier.
        contentRect.width = 8;
        EditorGUI.LabelField(contentRect, "(");
        contentRect.x += contentRect.width + 2;
        contentRect.width = 40;
		EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("initial"), GUIContent.none);

        contentRect.x += contentRect.width + 1;
        contentRect.width = 50;
		EditorGUI.LabelField(contentRect, string.Format("+ {0}) x {1}", property.FindPropertyRelative("added").floatValue, property.FindPropertyRelative("modifier").floatValue));

        // Draw a button that resets the added and modifiers to 0 and 1 respectively.
        contentRect.x += contentRect.width + 5;
        contentRect.width = 18;
        if (GUI.Button(contentRect, resetLabel, EditorStyles.miniButton)) {
            property.FindPropertyRelative("added").floatValue = 0;
            property.FindPropertyRelative("modifier").floatValue = 1.0f;
        }
        
        // Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}
