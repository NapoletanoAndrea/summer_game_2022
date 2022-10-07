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

            if (spriteRenderer) {
                switch (associatedMono) {
                    case Door door:
                        SetSprite(door.elementType, door.needsElement);
                        break;
                    case Key key:
                        SetSprite(key.elementType, key.needsElement);
                        break;
                }
            }
        }
    }

    private void SetSprite(ElementType elementType, bool needsElement) {
        if (needsElement) {
            var spriteData = Utils.GetAllInstances<DataSpritesSO>();
            if (spriteData.Length > 0) {
                spriteRenderer.sprite = spriteData[0].elementSprites[elementType];
            }
        }
        else {
            spriteRenderer.sprite = null;
        }
    }
}
