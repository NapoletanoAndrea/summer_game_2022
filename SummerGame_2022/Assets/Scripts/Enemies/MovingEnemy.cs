using UnityEngine;

public class MovingEnemy : EnemyBase {
	[SerializeField] private MovingComponent movingComponent;

	protected override void OnEnemyTurn() {
		movingComponent.Move();
	}
}
