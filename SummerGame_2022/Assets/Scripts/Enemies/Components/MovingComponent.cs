using System;
using UnityEngine;

public abstract class MovingComponent : EnemyComponent {
	public Action Moved;

	protected Transform transform;
	protected Collider2D collider;
	
	public virtual void Init(EnemyBase enemyBase) {
		base.Init(enemyBase);
		transform = enemyBase.transform;
		collider = enemyBase.GetComponent<Collider2D>();
		Moved += enemyBase.Moved;
	}

	public virtual void Move() {
		Moved?.Invoke();
	}

	private void OnDisable() {
		Moved -= enemyBase.Moved;
	}
}
