using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using PatrolTile = PatrollingEnemy.PatrolTile;

[CustomEditor(typeof(PatrollingEnemy))]
public class PatrollingEnemyEditor : Editor {
	private SerializedObject thisObject;
	private bool settingPatrolTiles;
	private bool removingPatrolTiles;
	private bool editingTileConnections;
	private bool editingConnectionsOrder;

	private bool displayAllConnectionOrders;

	private GUIStyle paintingTextStyle;

	private Grid grid;

	private int index;
	private Vector2 tempPos;
	private PatrolTile tempTile;

	private TileDrawPoint selectedDrawPoint;

	private void OnEnable() {
		thisObject = new SerializedObject(this);
		SceneView.duringSceneGui += OnScene;
	}

	private void OnDisable() {
		SceneView.duringSceneGui -= OnScene;
		Tools.hidden = false;
	}

	private void DisableOtherSettings() {
		settingPatrolTiles = false;
		removingPatrolTiles = false;
		editingTileConnections = false;
		editingConnectionsOrder = false;
	}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if (!grid) {
			grid = FindObjectOfType<Grid>();
		}

		PatrollingEnemy enemy = (PatrollingEnemy) target;
		
		paintingTextStyle = new GUIStyle(GUI.skin.button)
		{
			fontStyle = FontStyle.Italic
		};
		
		if (!settingPatrolTiles) {
			if (GUILayout.Button("Set Patrol Tiles")) {
				DisableOtherSettings();
				settingPatrolTiles = true;
			}
		}
		else {
			if (GUILayout.Button("Setting Patrol Tiles...", paintingTextStyle)) {
				settingPatrolTiles = false;
			}
		}

		if (!removingPatrolTiles) {
			if (GUILayout.Button("Remove Patrol Tiles")) {
				DisableOtherSettings();
				removingPatrolTiles = true;
			}
		}
		else {
			if (GUILayout.Button("Removing Patrol Tiles...", paintingTextStyle)) {
				removingPatrolTiles = false;
			}
		}
		
		if (GUILayout.Button("Clear")) {
			enemy.patrolTiles.Clear();
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}

		if (!editingTileConnections) {
			if (GUILayout.Button("Edit Tile Connections")) {
				DisableOtherSettings();
				editingTileConnections = true;
				tempPos = Vector2.zero;
			}
		}
		else {
			if (GUILayout.Button("Editing Tile Connections...", paintingTextStyle)) {
				editingTileConnections = false;
			}
		}

		if (!editingConnectionsOrder) {
			if (GUILayout.Button("Edit Connections Order")) {
				DisableOtherSettings();
				editingConnectionsOrder = true;
			}
		}
		else {
			if (GUILayout.Button("Editing Connections Order...", paintingTextStyle)) {
				editingConnectionsOrder = false;
			}
		}

		displayAllConnectionOrders = EditorGUILayout.Toggle("Display All Connection Orders", displayAllConnectionOrders);
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
			foreach (var adjPos in tile.adjacentTiles) {
				Handles.color = Color.red;
				Handles.DrawLine(tile.tilePosition, adjPos, 2.5f);
			}
		}
		Tools.hidden = false;
		Event e = Event.current;
		if (settingPatrolTiles) {
			Tools.hidden = true;
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			if (e.type is EventType.MouseDrag or EventType.MouseDown && e.button == 0) {
				Vector2 mousePos = GetMousePosition(scene);
				enemy.AddPatrolTile(mousePos);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
		if (removingPatrolTiles) {
			Tools.hidden = true;
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			if (e.type is EventType.MouseDrag or EventType.MouseDown && e.button == 0) {
				Vector2 mousePos = GetMousePosition(scene);
				enemy.RemovePatrolTile(mousePos);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
		if (editingTileConnections) {
			Tools.hidden = true;
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			if (e.button == 0) {
				if (e.type == EventType.MouseDown) {
					tempPos = GetMousePosition(scene);
				}
				if (tempPos != Vector2.zero) {
					Handles.DrawLine(tempPos, SceneToWorldPoint(e.mousePosition, scene));
				}
				if (e.type == EventType.MouseUp) {
					enemy.AddTileConnection(tempPos, GetMousePosition(scene));
					tempPos = Vector2.zero;
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}
			}
			if (e.button == 1) {
				if (e.type == EventType.MouseDown) {
					tempPos = GetMousePosition(scene);
				}
				if (e.type == EventType.MouseDrag) {
					e.Use();
				}
				if (e.type == EventType.MouseUp) {
					enemy.RemoveTileConnection(tempPos, GetMousePosition(scene));
					tempPos = Vector2.zero;
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}
			}
		}
		if (displayAllConnectionOrders || editingConnectionsOrder) {
			foreach (var tile in enemy.patrolTiles) {
				DrawConnectionOrders(tile);
			}
		}
		if (editingConnectionsOrder) {
			Tools.hidden = true;
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			if (selectedDrawPoint == null) {
				if (e.type == EventType.MouseDown && e.button == 0) {
					List<TileDrawPoint> tileDrawPoints = new();
					foreach (var tile in enemy.patrolTiles) {
						tileDrawPoints.AddRange(GetDrawPoints(tile));
					}
					Vector2 mousePos = SceneToWorldPoint(e.mousePosition, scene);
					if (tileDrawPoints.Count >= 2) {
						tileDrawPoints = tileDrawPoints.OrderBy(t => (t.drawPoint - mousePos).sqrMagnitude).ToList();
						if (Vector2.Distance(tileDrawPoints[0].drawPoint, mousePos) < cellSize.x / 2) {
							selectedDrawPoint = tileDrawPoints[0];
						}
					}
				}
			}
			else {
				var tileDrawPoints = GetDrawPoints(selectedDrawPoint.tile);
				Vector2 mousePos = SceneToWorldPoint(e.mousePosition, scene);
				tileDrawPoints = tileDrawPoints.OrderBy(t => (t.drawPoint - mousePos).sqrMagnitude).ToList();
				if (tileDrawPoints[0].drawPoint != selectedDrawPoint.drawPoint) {
					Swap(tileDrawPoints[0], selectedDrawPoint);
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}
				if (e.type == EventType.MouseUp && e.button == 0) {
					selectedDrawPoint = null;
				}
			}
		}
	}

	private void DrawConnectionOrders(PatrolTile tile) {
		PatrollingEnemy enemy = (PatrollingEnemy) target;
		Vector2 cellSize = grid.cellSize;
		
		int drawLines = tile.adjacentTiles.Count - 1;
		foreach (var adjTilePos in tile.adjacentTiles) {
			Vector2 direction = adjTilePos - tile.tilePosition;
			for (int i = 0; i < drawLines; i++) {
				float offset = i * cellSize.x * .05f;
				Vector2 drawPoint = direction / 2 + tile.tilePosition - direction * offset;
				float lenght = cellSize.x * .15f;
				float thickness = 1.2f;
				var degrees = 57;
						
				var dir = direction.normalized * lenght;
				dir = dir.Rotate(-degrees);
				Vector2 endPoint1 = drawPoint - dir;
				Handles.DrawLine(drawPoint, endPoint1, thickness);
						
				dir = direction.normalized * lenght;
				dir = dir.Rotate(degrees);
				Vector2 endPoint2 = drawPoint - dir;
				Handles.DrawLine(drawPoint, endPoint2, thickness);
			}
			drawLines--;
		}
	}

	private class TileDrawPoint {
		public PatrolTile tile;
		public Vector2 drawPoint;
		public int adjTileIndex;
	}
	
	private List<TileDrawPoint> GetDrawPoints(PatrolTile tile) {
		List<TileDrawPoint> drawPoints = new();
		if (tile.adjacentTiles.Count >= 2) {
			int i = 0;
			foreach (var adjTilePos in tile.adjacentTiles) {
				Vector2 direction = adjTilePos - tile.tilePosition;
				Vector2 drawPointPos = direction / 2 + tile.tilePosition - direction.normalized * .01f;
				TileDrawPoint drawPoint = new()
				{
					tile = tile,
					drawPoint = drawPointPos,
					adjTileIndex = i
				};
				drawPoints.Add(drawPoint);
				i++;
			}
		}
		return drawPoints;
	}

	private void Swap(TileDrawPoint toSwap, TileDrawPoint selected) {
		int swapIndex1 = selected.adjTileIndex;
		int swapIndex2 = toSwap.adjTileIndex;
		var adjTileList = selected.tile.adjacentTiles;
		(adjTileList[swapIndex1], adjTileList[swapIndex2]) = (adjTileList[swapIndex2], adjTileList[swapIndex1]);
		(selected.drawPoint, toSwap.drawPoint) = (toSwap.drawPoint, selected.drawPoint);
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
