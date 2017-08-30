using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShipSocketComponent))]
public class ShipSocketInspector : Editor {

	private void OnSceneGUI() {
		ShipSocketComponent sock = target as ShipSocketComponent;

		Transform handleTx = sock.transform;
		Vector3 point = handleTx.TransformPoint(sock.position);
		Quaternion handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? handleTx.rotation : Quaternion.identity;

		Handles.DoPositionHandle(point, handleRotation);

		EditorGUI.BeginChangeCheck();
		point = Handles.DoPositionHandle(point, handleRotation);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(sock, "Move Socket");
			EditorUtility.SetDirty(sock);
			sock.position = handleTx.InverseTransformPoint(point);
		}
	}
}
