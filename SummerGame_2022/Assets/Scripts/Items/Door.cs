using UnityEngine;

public class Door : MonoBehaviour, ITargetable {
	[SerializeField] private bool needsKey;
	[SerializeField] private ElementType elementType;

	public void ReceiveMove(MoveData move) {
		var mono = move.sender as MonoBehaviour;
		if (mono != null && mono.gameObject.CompareTag("Player") && move.moveType == elementType) {
			if (!needsKey || LevelManager.Instance.hasKey) {
				SceneLoader.Instance.LoadLevel(LevelManager.Instance.currentLevel + 1);
			}
		}
	}
}
