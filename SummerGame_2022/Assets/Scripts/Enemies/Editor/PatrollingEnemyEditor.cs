using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PatrollingEnemy))]
public class PatrollingEnemyEditor : Editor {
	private SerializedObject thisObject;
	private bool settingPatrolTiles;
	private bool removingPatrolTiles;

	private GUIStyle paintingTextStyle;

	private Grid grid;

	private void OnEnable() {
		thisObject = new SerializedObject(this);
		SceneView.duringSceneGui += OnScene;
	}

	private void OnDisable() {
		SceneView.duringSceneGui -= OnScene;
		Tools.hidden = false;
	}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if (!grid) {
			grid = FindObjectOfType<Grid>();
		}

		PatrollingEnemy enemy = (PatrollingEnemy) target;
		
		if (!settingPatrolTiles) {
			if (GUILayout.Button("Set Patrol Tiles")) {
				settingPatrolTiles = true;
				removingPatrolTiles = false;
				thisObject.ApplyModifiedProperties();
			}
		}
		else {
			paintingTextStyle = new GUIStyle(GUI.skin.button)
			{
				fontStyle = FontStyle.Italic
			};
			if (GUILayout.Button("Setting Patrol Tiles...", paintingTextStyle)) {
				settingPatrolTiles = false;
				thisObject.ApplyModifiedProperties();
			}
		}

		if (!removingPatrolTiles) {
			if (GUILayout.Button("Remove Patrol Tiles")) {
				removingPatrolTiles = true;
				settingPatrolTiles = false;
				thisObject.ApplyModifiedProperties();
			}
		}
		else {
			paintingTextStyle = new GUIStyle(GUI.skin.button)
			{
				fontStyle = FontStyle.Italic
			};
			if (GUILayout.Button("Removing Patrol Tiles...", paintingTextStyle)) {
				removingPatrolTiles = false;
				thisObject.ApplyModifiedProperties();
			}
		}
		
		if (GUILayout.Button("Clear")) {
			enemy.patrolTiles.Clear();
		}
	}

	private void OnScene(SceneView scene) {
		if (!grid) {
			return;
		}
		PatrollingEnemy enemy = (PatrollingEnemy) target;
		Vector2 cellSize = grid.cellSize;
		foreach (var tile in enemy.patrolTiles) {
			Vector2 rectPos = tile.tilePosition;
			rectPos -= cellSize / 2;
			Color faceColor = Color.red;
			faceColor.a = .2f;
			Color outlineColor = Color.red;
			Handles.DrawSolidRectangleWithOutline(new Rect(rectPos, cellSize), faceColor, outlineColor);
		}
		Tools.hidden = false;
		Event e = Event.current;
		if (settingPatrolTiles) {
			Tools.hidden = true;
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			if (e.type is EventType.MouseDrag or EventType.MouseDown && e.button == 0) {
				Vector2 mousePos = GetMousePosition(scene);
				enemy.AddPatrolTile(mousePos);
			}
		}
		if (removingPatrolTiles) {
			Tools.hidden = true;
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			if (e.type is EventType.MouseDrag or EventType.MouseDown && e.button == 0) {
				Vector2 mousePos = GetMousePosition(scene);
				enemy.RemovePatrolTile(mousePos);
			}
		}
	}

	private Vector2 GetMousePosition(SceneView scene) {
		Vector2 mousePos = Event.current.mousePosition;
		mousePos = SceneToWorldPoint(mousePos, scene);
		
		float halfCellSizeX = grid.cellSize.x / 2;
		float halfCellSizeY = grid.cellSize.y / 2;

		float tileX = GridUtils.SnapNumber(mousePos.x, halfCellSizeX);
		float tileY = GridUtils.SnapNumber(mousePos.y, halfCellSizeY);
		return new Vector2(tileX, tileY);
	}
	
	private Vector3 SceneToWorldPoint(Vector3 scenePoint, SceneView scene) {
		scenePoint.y = scene.camera.pixelHeight - scenePoint.y;
		scenePoint = scene.camera.ScreenToWorldPoint(scenePoint);
		return scenePoint;
	}
}
