using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Door : MonoBehaviour, ITargetable {
	[FormerlySerializedAs("neededKeys")] public Key[] keysNeeded;
	public EnemyElement[] enemyKillsNeeded;
	public bool needsElement;
	[ShowIf("needsElement")]
	public ElementType elementType;

	public void ReceiveMove(MoveData move) {
		var mono = move.sender as MonoBehaviour;
		if (mono != null && mono.gameObject.CompareTag("Player") && move.moveType == elementType) {
			foreach (var key in keysNeeded) {
				if (!LevelManager.Instance.foundKeys.Contains(key)) {
					return;
				}
			}
			foreach (var kill in enemyKillsNeeded) {
				if (!LevelManager.Instance.enemiesKilled.Contains(kill)) {
					return;
				}
			}
			SceneLoader.Instance.LoadLevel(LevelManager.Instance.currentLevel + 1);
		}
	}
}
