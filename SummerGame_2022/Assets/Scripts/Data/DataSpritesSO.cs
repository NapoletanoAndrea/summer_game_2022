using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Element Sprites")]
public class DataSpritesSO : ScriptableObject
{
	public UnitySerializedDictionary<ElementType, Sprite> elementSprites;
}
