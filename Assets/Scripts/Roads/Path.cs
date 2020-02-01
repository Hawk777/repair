﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path {
	[SerializeField, HideInInspector] List<Vector2> points;
	[SerializeField, HideInInspector] bool isClosed;
	[SerializeField, HideInInspector] bool autoSetControlPoints;

	public Path(Vector2 centre) {
		points = new List<Vector2> {
			centre + Vector2.left,
			centre + (Vector2.left + Vector2.up) * .5f,
			centre + (Vector2.right + Vector2.down) *.5f,
			centre + Vector2.right
		};
	}

	public Vector2 this[int i] {
		get {
			return points[i];
		}
	}

	public bool AutoSetControlPoints {
		get {
			return autoSetControlPoints;
		}
		set {
			if (autoSetControlPoints != value) {
				autoSetControlPoints = value;
				if (autoSetControlPoints) {
					AutoSetAllControlPoints();
				}
			}
		}
	}

	public int NumPoints {
		get {
			return points.Count;
		}
	}

	public int NumSegments {
		get {
			return points.Count / 3;
		}
	}

	public void AddSegment(Vector2 anchorPos) {
		points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
		points.Add((points[points.Count - 1] + anchorPos) * .5f);
		points.Add(anchorPos);

		if (autoSetControlPoints) {
			AutoSetAllAffectedControlPoints(points.Count - 1);
		}
	}

	public Vector2[] GetPointsInSegment(int i) {
		return new Vector2[] { points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[LoopIndex(i * 3 + 3)] };
	}

	public void MovePoint(int i, Vector2 pos) {
		if (autoSetControlPoints && i %3 != 0) {
			return;
		}

		Vector2 deltaMove = pos - points[i];

		if (!autoSetControlPoints || i % 3 == 0)
		points[i] = pos;

		if (autoSetControlPoints) {
			AutoSetAllAffectedControlPoints(i);
			return;
		}

		// Moving anchor point
		if (i % 3 == 0) {
			if (isClosed || i + 1 < points.Count) {
				points[LoopIndex(i + 1)] += deltaMove;
			}
			if (isClosed || i - 1 >= 0) {
				points[LoopIndex(i - 1)] += deltaMove;
			}
		}
		else {
			bool nextPointIsAnchor = (i + 1) % 3 == 0;
			int correspondingControlIndex = (nextPointIsAnchor) ? i + 2 : i - 2;
			int anchorIndex = (nextPointIsAnchor) ? i + 1 : i - 1;

			if (isClosed || correspondingControlIndex >= 0 && correspondingControlIndex < points.Count) {
				float distance = (points[LoopIndex(anchorIndex)] - points[LoopIndex(correspondingControlIndex)]).magnitude;
				Vector2 direction = (points[LoopIndex(anchorIndex)] - pos).normalized;
				points[LoopIndex(correspondingControlIndex)] = points[LoopIndex(anchorIndex)] + direction * distance;
			}
		}
	}

	public void ToggleClosed() {
		isClosed = !isClosed;

		if (isClosed) {
			points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
			points.Add(points[0] * 2 - points[1]);
			if (autoSetControlPoints) {
				AutoSetAnchorControlPoints(0);
				AutoSetAnchorControlPoints(points.Count - 3);
			}
		}
		else {
			points.RemoveRange(points.Count - 2, 2);
			if (autoSetControlPoints) {
				AutoSetStartAndEndControls();
			}
		}
	}

	void AutoSetAllAffectedControlPoints(int updatedAnchorIndex) {
		for (int i = updatedAnchorIndex - 3; i <= updatedAnchorIndex + 3; i += 3) {
			if (isClosed || i >= 0 && i < points.Count) {
				AutoSetAnchorControlPoints(LoopIndex(i));
			}
		}

		AutoSetStartAndEndControls();
	}

	void AutoSetAllControlPoints() {
		for (int i = 0; i < points.Count; i += 3) {
			AutoSetAnchorControlPoints(i);
		}

		AutoSetStartAndEndControls();
	}

	void AutoSetAnchorControlPoints(int anchorIndex) {
		Vector2 anchorPos = points[anchorIndex];
		Vector2 direction = Vector2.zero;
		float[] neighbourDistances = new float[2];

		if (isClosed || anchorIndex - 3 >= 0 ) {
			Vector2 offset = points[LoopIndex(anchorIndex - 3)] - anchorPos;
			direction += offset.normalized;
			neighbourDistances[0] = offset.magnitude;
		}

		if (isClosed || anchorIndex + 3 >= 0 ) {
			Vector2 offset = points[LoopIndex(anchorIndex + 3)] - anchorPos;
			direction -= offset.normalized;
			neighbourDistances[1] = -offset.magnitude;
		}

		direction.Normalize();

		for (int i = 0; i < 2; ++i) {
			int controlIndex = anchorIndex + i * 2 - 1;
			if (isClosed || controlIndex >= 0 && controlIndex < points.Count) {
				points[LoopIndex(controlIndex)] = anchorPos + direction * neighbourDistances[i] * .5f;
			}
		}
	}

	void AutoSetStartAndEndControls() {
		if (!isClosed) {
			points[1] = (points[0] + points[2]) * .5f;
			points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * .5f;
		}
	}

	int LoopIndex(int i) {
		return (i + points.Count) % points.Count;
	}
}
