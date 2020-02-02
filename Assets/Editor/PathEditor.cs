using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
	PathCreator creator;
	Path Path {
		get {
			return creator.path;
		}
	}

	const float segmentSelectDistanceThreshold = .1f;
	int selectedSegmentIndex = -1;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		EditorGUI.BeginChangeCheck();
		if (GUILayout.Button("Create new")) {
			Undo.RecordObject(creator, "Create new");
			creator.CreatePath();
		}

		bool isClosed = GUILayout.Toggle(Path.IsClosed, "Closed");
		if (isClosed != Path.IsClosed) {
			Undo.RecordObject(creator, "Toggle closed");
			Path.IsClosed = isClosed;
		}

		bool autoSetControlPoints = GUILayout.Toggle(Path.AutoSetControlPoints, "Auto Set Control Points");
		if (autoSetControlPoints != Path.AutoSetControlPoints) {
			Undo.RecordObject(creator, "Toggle auto set controls");
			Path.AutoSetControlPoints = autoSetControlPoints;
		}

		if (EditorGUI.EndChangeCheck()) {
			SceneView.RepaintAll();
		}
	}

	private void OnSceneGUI() {
		Input();
		Draw();
	}

	void Input() {
		Event guiEvent = Event.current;
		Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift) {
			if (selectedSegmentIndex != -1) {
				Undo.RecordObject(creator, "Split segment");
				Path.SplitSegment(mousePos, selectedSegmentIndex);
			} else if (!Path.IsClosed) {
				Undo.RecordObject(creator, "Add segment");
				Path.AddSegment(mousePos);
			}
		}

		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1) {
			float minDistanceToAnchor = creator.anchorDiameter * .5f;
			int closestAnchorIndex = -1;

			for (int i =0; i < Path.NumPoints; i += 3) {
				float distance = Vector2.Distance(mousePos, Path[i]);
				if (distance < minDistanceToAnchor) {
					minDistanceToAnchor = distance;
					closestAnchorIndex = i;
				}
			}

			if (closestAnchorIndex != -1) {
				Undo.RecordObject(creator, "Delete segment");
				Path.DeleteSegment(closestAnchorIndex);
			}
		}

		if (guiEvent.type == EventType.MouseMove) {
			float minDistanceToSegment = segmentSelectDistanceThreshold;
			int newSelectedSegmentIndex = -1;

			for (int i = 0; i < Path.NumSegments; ++i) {
				Vector2[] points = Path.GetPointsInSegment(i);
				float distance = HandleUtility.DistancePointBezier(mousePos, points[0], points[3], points[1], points[2]);
				if (distance < minDistanceToSegment) {
					minDistanceToSegment = distance;
					newSelectedSegmentIndex = i;
				}
			}

			if (newSelectedSegmentIndex != selectedSegmentIndex) {
				selectedSegmentIndex = newSelectedSegmentIndex;
				HandleUtility.Repaint();
			}
		}
	}

	void Draw() {
		for (int i = 0; i < Path.NumSegments; ++i) {
			Vector2[] points = Path.GetPointsInSegment(i);

			if (creator.displayControlPoints) {
				Handles.color = Color.black;
				Handles.DrawLine(points[1], points[0]);
				Handles.DrawLine(points[2], points[3]);
			}
			Color segmentColor = (i == selectedSegmentIndex && Event.current.shift) ? creator.selectedSegmentColor : creator.segmentColor;
			Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentColor, null, 2);
		}

		for (int i = 0; i < Path.NumPoints; ++i) {
			if (creator.displayControlPoints || i % 3 == 0) {
				Handles.color = (i % 3 == 0) ? creator.anchorColor : creator.controlColor;
				float handleSize = (i % 3 == 0) ? creator.anchorDiameter : creator.controlDiamater;
				Vector2 newPos = Handles.FreeMoveHandle(Path[i], Quaternion.identity, handleSize, Vector2.zero, Handles.CylinderHandleCap);
				if (Path[i] != newPos) {
					Undo.RecordObject(creator, "Move point");
					Path.MovePoint(i, newPos);
				}
			}
		}
	}

	void OnEnable() {
		creator = (PathCreator)target;
		if (creator.path == null) {
			creator.CreatePath();
		}
	}
}
