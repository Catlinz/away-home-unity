using UnityEngine;
using UnityEditor;

// Custom drawer for ModifiableFloat types.
[CustomPropertyDrawer(typeof(ModifiableFloat))]
public class ModifiableFloatDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		label = EditorGUI.BeginProperty(position, label, property);

		// Draw label
		Rect contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

        contentRect.width = 8;
        EditorGUI.LabelField(contentRect, "(");
        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        contentRect.x += contentRect.width + 2;
        contentRect.width = 40;
		EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("initial"), GUIContent.none);

        contentRect.x += contentRect.width + 1;
        contentRect.width = 50;
		EditorGUI.LabelField(contentRect, string.Format("+ {0}) x {1}", property.FindPropertyRelative("added").floatValue, property.FindPropertyRelative("modifier").floatValue));
		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}
