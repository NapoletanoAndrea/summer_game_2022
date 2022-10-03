using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum Turn {
    PlayerTurn,
    EnemyTurn
}

public class TurnManager : MonoBehaviour {
    [SerializeField] private PlayerController playerController;

    [SerializeField] private Turn startTurn;
    [SerializeField, ReadOnly] private Turn currentTurn;
    public Turn CurrentTurn => currentTurn;
    [NonSerialized] public int turnsLeftForMoveExecution;
    
    public static TurnManager Instance;

    public event Action PlayerTurn;
    public event Action EnemyTurn;

    private void Awake() {
        Instance = this;
        currentTurn = startTurn;
    }

    private void Start() {
        playerController.Moved += OnPlayerMoved;
        EnemyManager.Instance.FinishedEnemyTurn += OnEnemyMoved;
        Invoke(nameof(LateStart), .1f);
    }

    private void OnDisable() {
        playerController.Moved -= OnPlayerMoved;
        EnemyManager.Instance.FinishedEnemyTurn -= OnEnemyMoved;
    }

    private void LateStart() {
        if (currentTurn == Turn.PlayerTurn) {
            OnPlayerTurn();
        }
        if (currentTurn == Turn.EnemyTurn) {
            OnEnemyTurn();
        }
    }

    private void OnPlayerTurn() {
        PlayerTurn?.Invoke();
    }

    private void OnEnemyTurn() {
        EnemyTurn?.Invoke();
    }

    private void OnPlayerMoved() {
        currentTurn = Turn.EnemyTurn;
        if (EnemyManager.Instance.AreThereEnemiesLeft()) {
            OnEnemyTurn();
        }
        else {
            OnPlayerTurn();
        }
    }

    private void OnEnemyMoved() {
        currentTurn = Turn.PlayerTurn;
        OnPlayerTurn();
    }
}
