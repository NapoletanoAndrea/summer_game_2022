using Sirenix.OdinInspector;
using UnityEngine;

public class Door : MonoBehaviour, ITargetable {
	public bool needsKey;
	public bool needsElement;
	[ShowIf("needsElement")]
	public ElementType elementType;

	public void ReceiveMove(MoveData move) {
		var mono = move.sender as MonoBehaviour;
		if (mono != null && mono.gameObject.CompareTag("Player") && move.moveType == elementType) {
			if (!needsKey || LevelManager.Instance.hasKey) {
				SceneLoader.Instance.LoadLevel(LevelManager.Instance.currentLevel + 1);
			}
		}
	}
}
