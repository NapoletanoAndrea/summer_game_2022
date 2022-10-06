using System.Collections;
using UnityEngine;

public abstract class MovingEnemy : EnemyBase {
	[SerializeField] protected float moveSeconds;
	[SerializeField] protected bool slow;

	protected void Move(Vector2 direction) {
		if (CanMove()) {
			StartCoroutine(MoveCoroutine(direction));
		}
	}
	
	protected bool CanMove() {
		if (slow) {
			return TurnManager.Instance.turnsLeftForMoveExecution == 1;
		}
		return true;
	}
	
	private IEnumerator MoveCoroutine(Vector2 vector, bool isLocation = false) {
		float count = 0;
		float t = 0;
		Vector2 startPosition = transform.position;
		
		if (!isLocation) {
			while (count <= moveSeconds) {
				count += Time.deltaTime;
				t += Time.deltaTime / moveSeconds;
				transform.position = Vector3.Lerp(startPosition, startPosition + vector, t);
				yield return null;
			}
			transform.position = startPosition + vector;
		}
		
		Moved?.Invoke();
	}
}
