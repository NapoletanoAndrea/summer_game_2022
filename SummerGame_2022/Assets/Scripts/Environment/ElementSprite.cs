using UnityEngine;

[ExecuteInEditMode]
public class ElementSprite : MonoBehaviour {
    [SerializeField] private MonoBehaviour associatedMono;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnRenderObject() {
        if (associatedMono) {
            if (!spriteRenderer) {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            var spriteData = Utils.GetAllInstances<DataSpritesSO>();
            if (spriteRenderer && spriteData.Length > 0) {
                switch (associatedMono) {
                    case Door door:
                        spriteRenderer.sprite = spriteData[0].elementSprites[door.elementType];
                        break;
                    case Key key:
                        spriteRenderer.sprite = spriteData[0].elementSprites[key.elementType];
                        break;
                }
            }
        }
    }
}
