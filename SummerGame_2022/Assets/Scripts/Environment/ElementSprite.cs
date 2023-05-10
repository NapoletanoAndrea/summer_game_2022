using UnityEngine;

[ExecuteInEditMode]
public class ElementSprite : MonoBehaviour
{
	[SerializeField] private MonoBehaviour associatedMono;

	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnRenderObject()
	{
		if (associatedMono)
		{
			if (!spriteRenderer)
			{
				spriteRenderer = GetComponent<SpriteRenderer>();
			}

			if (spriteRenderer)
			{
				switch (associatedMono)
				{
					case Door door:
						SetSprite(door.elementType, door.needsElement);
						break;
					case Key key:
						SetSprite(key.elementType, key.needsElement);
						break;
					case EnemyElement enemy:
						SetSprite(!Application.isPlaying ? enemy.startingElement : enemy.CurrentElement);
						break;
				}
			}
		}
	}

	private void SetSprite(ElementType elementType, bool needsElement = true)
	{
		if (needsElement)
		{
			DataSpritesSO spriteData = null;
			if (Application.isPlaying)
			{
				spriteData = DataSprites.Instance.dataSprites;
			}
			else
			{
#if UNITY_EDITOR
				spriteData = Utils.GetAllInstances<DataSpritesSO>()[0];
#endif
			}
			
			if (spriteData)
			{
				spriteRenderer.sprite = spriteData.elementSprites[elementType];
			}
		}
		else
		{
			spriteRenderer.sprite = null;
		}
	}
}