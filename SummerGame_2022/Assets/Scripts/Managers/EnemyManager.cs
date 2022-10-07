using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
	public static EnemyManager Instance;
	
	private List<EnemyElement> enemies = new();
	private int finishedCount;

	public event Action FinishedEnemyTurn;

	private void Awake() {
		Instance = this;
	}

	public void Subscribe(EnemyElement enemyElement) {
		enemies.Add(enemyElement);
		enemyElement.FinishedTurn += OnEnemyFinishedTurn;
	}

	public void Unsubscribe(EnemyElement enemyElement) {
		Debug.Log("Uns");
		enemies.Remove(enemyElement);
		enemyElement.FinishedTurn -= OnEnemyFinishedTurn;
		if (!AreThereEnemiesLeft() && TurnManager.Instance.CurrentTurn == Turn.EnemyTurn) {
			Debug.Log("Finished");
			FinishedEnemyTurn?.Invoke();
		}
	}

	public bool AreThereEnemiesLeft() {
		return enemies.Count > 0;
	}

	private void OnEnemyFinishedTurn() {
		finishedCount++;
		if (finishedCount >= enemies.Count) {
			FinishedEnemyTurn?.Invoke();
		}
	}
}
