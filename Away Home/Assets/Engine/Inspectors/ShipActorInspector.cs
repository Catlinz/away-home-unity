using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShipActorComponent))]
public class ShipActorInspector : Editor {

	private ShipActorComponent ship;
	private Transform shipTx;
	private Quaternion shipRot;

	private void OnSceneGUI() {
		ship = target as ShipActorComponent;
		shipTx = ship.transform;
		shipRot = (Tools.pivotRotation == PivotRotation.Local) ? shipTx.rotation : Quaternion.identity;


		for (int i = 0; i < ship.sockets.Length; ++i) {
			SceneGUIHandleSocket(ref ship.sockets[i]);
		}
	}

	/**
	 * Handle the drawing of the socket for the OnSceneGUI method
	 */
	private void SceneGUIHandleSocket(ref ShipSocket socket) {
		Vector3 point = shipTx.TransformPoint(socket.position);
		Quaternion rot = shipRot * socket.rotation;


		if (Tools.current == Tool.Move || Tools.current == Tool.Rotate) {
			EditorGUI.BeginChangeCheck();
			if (Tools.current == Tool.Move) {
				point = Handles.DoPositionHandle(point, shipRot);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(ship, "Move Socket");
					EditorUtility.SetDirty(ship);
					socket.position = shipTx.InverseTransformPoint(point);
				}
			}
			else if (Tools.current == Tool.Rotate) {
				rot = Handles.DoRotationHandle(rot, point);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(ship, "Rotate Socket");
					EditorUtility.SetDirty(ship);
					socket.rotation = Quaternion.Inverse(shipRot) * rot;
				}
			}
		}
		else {
			Color color = Handles.color;
			Handles.color = new Color(1, 1, 1, 0.1f);
			Handles.DrawSolidDisc(point, rot * Vector3.up, 0.1f * HandleUtility.GetHandleSize(point));
		}
	}
}
