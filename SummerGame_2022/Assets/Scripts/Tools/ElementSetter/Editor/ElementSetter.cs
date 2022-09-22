using UnityEditor;
using UnityEngine;

public class ElementSetter : EditorWindow
{
	private SerializedObject thisObject;
	private GUIStyle textStyle;
	private GUIStyle paintingTextStyle;

	[SerializeField] private GameObject parentObject;
	[SerializeField] private GameObject objectToInstantiate;

	[SerializeField] private bool painting;

	private Grid grid;
	private GameObject gameObjectPreview;
	private GameObject selectedGameObject;

	[MenuItem("Custom Tools/Element Setter")]
	public static void ShowWindow() {
		var window = GetWindow<ElementSetter>();
		window.Show();
	}

	private void OnEnable() {
		thisObject = new SerializedObject(this);
		SceneView.duringSceneGui += OnScene;
		grid = FindObjectOfType<Grid>();
		
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
	}

	private void OnGUI() {
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
		if (!objectToInstantiateReference) {
			paintingProperty.boolValue = false;
		}
		if (!paintingReference) {
			if (GUI.Button(paintButtonRect, "Paint")) {
				paintingProperty.boolValue = true;
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
	}
	
	private void DrawSprite(Rect rect, Sprite sprite) {
		GUI.DrawTextureWithTexCoords(rect, sprite.texture, 
			new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height, 
				sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height), true);
	}

	private void OnScene(SceneView scene) {
		Event e = Event.current;
		
		Vector2 mousePos = e.mousePosition;
		mousePos = SceneToWorldPoint(mousePos, scene);
		
		float halfCellSizeX = grid.cellSize.x / 2;
		float halfCellSizeY = grid.cellSize.y / 2;
		
		float tileX = Mathf.Round(mousePos.x);
		tileX += tileX >= mousePos.x ? -halfCellSizeX : halfCellSizeX;

		float tileY = Mathf.Round(mousePos.y);
		tileY += tileY >= mousePos.y ? -halfCellSizeY : halfCellSizeY;
	}
	
	private int GCF(int a, int b)
	{
		while (b != 0)
		{
			int temp = b;
			b = a % b;
			a = temp;
		}
		return a;
	}
	
	private int LCM(int a, int b)
	{
		return (a / GCF(a, b)) * b;
	}

	private Vector3 SceneToWorldPoint(Vector3 scenePoint, SceneView scene) {
		scenePoint.y = scene.camera.pixelHeight - scenePoint.y;
		scenePoint = scene.camera.ScreenToWorldPoint(scenePoint);
		return scenePoint;
	}
}
