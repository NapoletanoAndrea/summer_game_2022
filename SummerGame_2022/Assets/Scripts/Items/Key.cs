using UnityEngine;

public class Key : MonoBehaviour, ITargetable
{
	public void ReceiveMove(MoveData move) {
		var mono = move.sender as MonoBehaviour;
		if (mono != null && mono.gameObject.CompareTag("Player")) {
			LevelManager.Instance.hasKey = true;
		}
	}
}
