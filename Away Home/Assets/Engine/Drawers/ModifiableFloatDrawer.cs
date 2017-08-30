using UnityEngine;
using UnityEditor;

// Custom drawer for ModifiableFloat types.
[CustomPropertyDrawer(typeof(ModifiableFloat))]
public class ModifiableFloatDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects
		var initialRect = new Rect(position.x, position.y, 40, position.height);
		var timesRect = new Rect(position.x + 45, position.y, 10, position.height);
		var modifierRect = new Rect(position.x + 60, position.y, 30, position.height);

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(initialRect, property.FindPropertyRelative("initial"), GUIContent.none);
		EditorGUI.LabelField(timesRect, "x");
		EditorGUI.PropertyField(modifierRect, property.FindPropertyRelative("modifier"), GUIContent.none);

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}
