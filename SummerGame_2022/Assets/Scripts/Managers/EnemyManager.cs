using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
	public static EnemyManager Instance;
	
	private List<EnemyBase> enemies = new();
	private int movedCount;

	public event Action FinishedEnemyTurn;

	private void Awake() {
		Instance = this;
	}

	public void Subscribe(EnemyBase enemyBase) {
		enemies.Add(enemyBase);
		enemyBase.Moved += OnEnemyMoved;
	}

	public void Unsubscribe(EnemyBase enemyBase) {
		enemies.Remove(enemyBase);
		enemyBase.Moved -= OnEnemyMoved;
		if (!AreThereEnemiesLeft() && TurnManager.Instance.CurrentTurn == Turn.EnemyTurn) {
			FinishedEnemyTurn?.Invoke();
		}
	}

	public bool AreThereEnemiesLeft() {
		return enemies.Count > 0;
	}

	private void OnEnemyMoved() {
		movedCount++;
		if (movedCount >= enemies.Count) {
			FinishedEnemyTurn?.Invoke();
		}
	}
}
