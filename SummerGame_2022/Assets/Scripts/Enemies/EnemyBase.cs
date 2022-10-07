using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour {
    public Action Moved;

    private void Start() {
        TurnManager.Instance.EnemyTurn += OnEnemyTurn;
    }

    private void OnDisable() {
        TurnManager.Instance.EnemyTurn -= OnEnemyTurn;
    }

    protected abstract void OnEnemyTurn();
}
