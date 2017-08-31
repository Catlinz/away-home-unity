using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Custom property drawer for ShipSockets.
[CustomPropertyDrawer(typeof(ShipSocket))]
public class ShipSocketDrawer : PropertyDrawer {

    private bool showSocket = true;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (showSocket) {
            return (EditorGUIUtility.singleLineHeight * 8) + EditorGUIUtility.standardVerticalSpacing * 11;
        }
        else {
            return base.GetPropertyHeight(property, label);
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Being the property control
        label = EditorGUI.BeginProperty(position, label, property);
        position.height = EditorGUIUtility.singleLineHeight;

        // Render the label for the ShipSocket.
        showSocket = EditorGUI.Foldout(position, showSocket, label, true);
        
        // Render the rest of the data if we need to.
        if (showSocket) {
            // Set the indent level to 0.
            int oldIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 1;

            Rect rect = new Rect(position);
            rect.height = EditorGUIUtility.singleLineHeight;

            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("socketName"));
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("maxCpuBandwidth"));
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("maxPowerOutput"));
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("arcLimits"));
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("position"));
            rect.y += (rect.height * 2) + EditorGUIUtility.standardVerticalSpacing * 5;
            EditorGUI.BeginChangeCheck();
            SerializedProperty rotProp = property.FindPropertyRelative("rotation");
            Vector3 rot = rotProp.quaternionValue.eulerAngles;
            rot = EditorGUI.Vector3Field(rect, rotProp.displayName, rot);
            if (EditorGUI.EndChangeCheck()) {
                rotProp.quaternionValue = Quaternion.Euler(rot);
            }

            EditorGUI.indentLevel = oldIndentLevel;
        }

       

        // Finish rendering the property.
        EditorGUI.EndProperty();
    }
}
