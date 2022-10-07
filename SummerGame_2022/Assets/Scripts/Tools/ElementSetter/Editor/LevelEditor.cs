using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditor : EditorWindow
{
	private SerializedObject thisObject;
	private GUIStyle textStyle;
	private GUIStyle centeredTextStyle;
	private GUIStyle paintingTextStyle;

	[SerializeField] private GameObject parentObject;
	[SerializeField] private GameObject objectToInstantiate;

	[SerializeField] private bool painting;
	[SerializeField] private bool deleting;

	private Grid grid;
	private GameObject gameObjectPreview;
	private GameObject selectedGameObject;

	[MenuItem("Custom Tools/Level Editor")]
	public static void ShowWindow() {
		var window = GetWindow<LevelEditor>(false, "Level Editor");
		window.Show();
	}

	private void Init() {
		grid = FindObjectOfType<Grid>();
	}

	private void OnEnable() {
		thisObject = new SerializedObject(this);
		SceneView.duringSceneGui += OnScene;

		Init();

		centeredTextStyle = new GUIStyle
		{
			alignment = TextAnchor.MiddleCenter,
			normal = new GUIStyleState
			{
				textColor = Color.white
			}
		};
		
		textStyle = new GUIStyle
		{
			alignment = TextAnchor.MiddleLeft,
			normal = new GUIStyleState
			{
				textColor = Color.white
			}
		};
	}

	private void OnDisable() {
		SceneView.duringSceneGui -= OnScene;
		if (gameObjectPreview) {
			DestroyImmediate(gameObjectPreview);
		}
	}
	
	private void OnInspectorUpdate() {
		if (gameObjectPreview) {
			if (mouseOverWindow) {
				if (mouseOverWindow.GetType() != typeof(SceneView)) {
					DestroyImmediate(gameObjectPreview);
				}
			}
			else {
				DestroyImmediate(gameObjectPreview);
			}
		}
	}

	private void OnGUI() {
		if (!grid) {
			Init();
			if (!grid) {
				GUI.Label(new Rect(0, 0, position.width, position.height), "No tilemap grid could be found!", centeredTextStyle);
				return;
			}	
		}
		
		float w = position.width / 2;
		float h = 20;

		var parentLabelRect = new Rect(0, 0, w, h);
		GUI.Label(parentLabelRect, "Parent GameObject: ", textStyle);

		var parentRect = new Rect(w, 0, w, h);
		SerializedProperty parentObjectProperty = thisObject.FindProperty("parentObject");
		EditorGUI.PropertyField(parentRect, parentObjectProperty, GUIContent.none);
		
		var objectLabelRect = new Rect(0, h, w, h);
		GUI.Label(objectLabelRect, "GameObject to Instantiate: ", textStyle);
		
		var objectRect = new Rect(w, h, w, h);
		SerializedProperty objectToInstantiateProperty = thisObject.FindProperty("objectToInstantiate");
		EditorGUI.PropertyField(objectRect, objectToInstantiateProperty, GUIContent.none);

		float previewX = position.width / 1.5f;
		float previewY = h * 2 + position.width / 10;
		float previewW = position.width / 5;
		GameObject objectToInstantiateReference = (GameObject) objectToInstantiateProperty.objectReferenceValue;
		if (objectToInstantiateReference) {
			var objectRenderer = objectToInstantiateReference.GetComponent<SpriteRenderer>();
			if (objectRenderer) {
				Sprite sprite = objectRenderer.sprite;
				float spriteRatio = sprite.bounds.size.x / sprite.bounds.size.y;
				var previewRect = new Rect(previewX, previewY, previewW, previewW / spriteRatio);
				DrawSprite(previewRect, sprite);
			}
		}

		var paintButtonY = previewY * 1.2f;
		var paintButtonRect = new Rect(0, paintButtonY, w, h);
		
		SerializedProperty paintingProperty = thisObject.FindProperty("painting");
		bool paintingReference = paintingProperty.boolValue;
		
		SerializedProperty deletingProperty = thisObject.FindProperty("deleting");
		bool deletingReference = deletingProperty.boolValue;
		
		if (!objectToInstantiateReference) {
			paintingProperty.boolValue = false;
		}
		if (!paintingReference) {
			if (GUI.Button(paintButtonRect, "Paint")) {
				paintingProperty.boolValue = true;
				deletingProperty.boolValue = false;
			}
		}
		else {
			paintingTextStyle = new GUIStyle(GUI.skin.button)
			{
				fontStyle = FontStyle.Italic
			};
			if (GUI.Button(paintButtonRect, "Painting...", paintingTextStyle)) {
				paintingProperty.boolValue = false;
			}
		}

		var delButtonY = paintButtonY + h;
		var delButtonRect = new Rect(0, delButtonY, w, h);
		
		if (!deletingReference) {
			if (GUI.Button(delButtonRect, "Delete")) {
				paintingProperty.boolValue = false;
				deletingProperty.boolValue = true;
			}
		}
		else {
			paintingTextStyle = new GUIStyle(GUI.skin.button)
			{
				fontStyle = FontStyle.Italic
			};
			if (GUI.Button(delButtonRect, "Deleting...", paintingTextStyle)) {
				deletingProperty.boolValue = false;
			}
		}

		float selectedLabelY = previewY + position.height / 5;
		var selectedLabelRect = new Rect(0, selectedLabelY, w, h);

		bool hasActiveGameObject = false;
		
		if (Selection.activeGameObject) {
			GUI.Label(selectedLabelRect, "Selected GameObject: " + Selection.activeGameObject.name);
			hasActiveGameObject = true;
		}
		else {
			GUI.Label(selectedLabelRect, "No valid GameObject has been selected!");
		}

		float buttonW = 60;
		float buttonH = 40;

		float upAndDownX = position.width / 4;
		float leftX = upAndDownX - 30;
		float rightX = upAndDownX + 30;

		float upY = selectedLabelY + position.height / 10;
		float leftAndRightY = upY + buttonH + 10;
		float downY = leftAndRightY + buttonH + 10;

		var upRect = new Rect(upAndDownX, upY, buttonW, buttonH);
		var downRect = new Rect(upAndDownX, downY, buttonW, buttonH);
		var leftRect = new Rect(leftX, leftAndRightY, buttonW, buttonH);
		var rightRect = new Rect(rightX, leftAndRightY, buttonW, buttonH);

		Transform activeGameObjectTransform = null;
		if (hasActiveGameObject) {
			activeGameObjectTransform = Selection.activeGameObject.transform;
		}
		if (GUI.Button(upRect, "Up") && hasActiveGameObject) {
			activeGameObjectTransform.position += Vector3.up * grid.cellSize.y;
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
		if (GUI.Button(downRect, "Down") && hasActiveGameObject) {
			activeGameObjectTransform.position += Vector3.down * grid.cellSize.y;
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
		if (GUI.Button(leftRect, "Left") && hasActiveGameObject) {
			activeGameObjectTransform.position += Vector3.left * grid.cellSize.x;
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
		if (GUI.Button(rightRect, "Right") && hasActiveGameObject) {
			activeGameObjectTransform.position += Vector3.right * grid.cellSize.x;
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
		
		thisObject.ApplyModifiedProperties();
	}
	
	private void DrawSprite(Rect rect, Sprite sprite) {
		GUI.DrawTextureWithTexCoords(rect, sprite.texture, 
			new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height, 
				sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height), true);
	}

	private void OnScene(SceneView scene) {
		if (!grid) {
			return;
		}
		
		Event e = Event.current;
		
		Vector2 mousePos = e.mousePosition;
		mousePos = SceneToWorldPoint(mousePos, scene);
		
		float halfCellSizeX = grid.cellSize.x / 2;
		float halfCellSizeY = grid.cellSize.y / 2;

		float tileX = GridUtils.SnapNumber(mousePos.x, halfCellSizeX);
		float tileY = GridUtils.SnapNumber(mousePos.y, halfCellSizeY);
		Vector2 pos = new Vector2(tileX, tileY);

		if (painting) {
			if (!objectToInstantiate) {
				return;
			}
		
			if (!gameObjectPreview) {
				InstantiatePreview(pos);
			}
			else if (gameObjectPreview.transform.position != (Vector3) pos) {
				DestroyImmediate(gameObjectPreview);
				gameObjectPreview = null;
				InstantiatePreview(pos);
			}

			if (e.type == EventType.MouseDown && e.button == 1) {
				InstantiateObject(pos);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
		else if (deleting) {
			if (gameObjectPreview) {
				DestroyImmediate(gameObjectPreview);
			}
			if (e.type == EventType.MouseDown && e.button == 1) {
				var toDeleteArray = FindObjectsOfType<SpriteRenderer>();
				foreach (var toDelete in toDeleteArray) {
					if (toDelete.transform.position == (Vector3) pos) {
						DestroyImmediate(toDelete.gameObject);
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
						break;
					}
				}
			}
		}
		else {
			if (gameObjectPreview) {
				DestroyImmediate(gameObjectPreview);
			}
		}
	}

	private void InstantiatePreview(Vector3 pos) {
		gameObjectPreview = Instantiate(objectToInstantiate, pos, Quaternion.identity);
		if (parentObject) {
			gameObjectPreview.transform.parent = parentObject.transform;
		}
		var spriteRenderer = gameObjectPreview.GetComponent<SpriteRenderer>();
		var color = spriteRenderer.color;
		color.a = .2f;
		spriteRenderer.color = color;
	}

	private void InstantiateObject(Vector3 pos) {
		if (PrefabUtility.IsPartOfAnyPrefab(objectToInstantiate)) {
			selectedGameObject = (GameObject) PrefabUtility.InstantiatePrefab(objectToInstantiate);
			selectedGameObject.transform.position = pos;
		}
		else {
			selectedGameObject = Instantiate(objectToInstantiate, pos, Quaternion.identity);
		}
		if (parentObject) {
			selectedGameObject.transform.parent = parentObject.transform;
		}
		Selection.activeGameObject = selectedGameObject;
	}

	private Vector3 SceneToWorldPoint(Vector3 scenePoint, SceneView scene) {
		scenePoint.y = scene.camera.pixelHeight - scenePoint.y;
		scenePoint = scene.camera.ScreenToWorldPoint(scenePoint);
		return scenePoint;
	}
}
