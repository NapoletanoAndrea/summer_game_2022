using UnityEngine;

public class Key : MonoBehaviour, ITargetable {
	[SerializeField] private ElementType elementType;
	
	public void ReceiveMove(MoveData move) {
		var mono = move.sender as MonoBehaviour;
		if (mono != null && mono.gameObject.CompareTag("Player") && move.moveType == elementType) {
			LevelManager.Instance.hasKey = true;
			gameObject.SetActive(false);
		}
	}
}