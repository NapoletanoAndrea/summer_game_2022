using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour {
    public Action Moved;

    private void Start() {
        TurnManager.Instance.EnemyTurn += OnEnemyTurn;
        EnemyManager.Instance.Subscribe(this);
    }

    private void OnDisable() {
        TurnManager.Instance.EnemyTurn -= OnEnemyTurn;
        EnemyManager.Instance.Unsubscribe(this);
    }

    protected abstract void OnEnemyTurn();
}
