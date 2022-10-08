using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum Turn {
    PlayerTurn,
    EnemyTurn,
    InBattle
}

public class TurnManager : MonoBehaviour {
    public PlayerElement playerElement;

    [SerializeField] private Turn startTurn;
    [SerializeField, ReadOnly] private Turn currentTurn;
    public Turn CurrentTurn => currentTurn;
    private Turn lastTurn;
    [NonSerialized] public int turnsLeftForMoveExecution;
    
    public static TurnManager Instance;

    public event Action PlayerTurn;
    public event Action EnemyTurn;

    private event Action BattleFinished;

    private void Awake() {
        Instance = this;
        currentTurn = startTurn;
        if (!playerElement) {
            playerElement = GameObject.FindWithTag("Player").GetComponent<PlayerElement>();
        }
    }

    private void Start() {
        playerElement.FinishedTurn += OnPlayerFinishedTurn;
        EnemyManager.Instance.FinishedEnemyTurn += OnEnemyFinishedTurn;
        BattleManager.Instance.BattleStarted += OnBattleStarted;
        BattleManager.Instance.BattleFinished += OnBattleFinished;
        Invoke(nameof(LateStart), .1f);
    }

    private void OnDisable() {
        playerElement.FinishedTurn -= OnPlayerFinishedTurn;
        EnemyManager.Instance.FinishedEnemyTurn -= OnEnemyFinishedTurn;
        BattleManager.Instance.BattleStarted -= OnBattleStarted;
        BattleManager.Instance.BattleFinished -= OnBattleFinished;
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
        currentTurn = Turn.PlayerTurn;
        PlayerTurn?.Invoke();
    }

    private void OnEnemyTurn() {
        currentTurn = Turn.EnemyTurn;
        EnemyTurn?.Invoke();
    }

    private void OnPlayerFinishedTurn() {
        if (currentTurn == Turn.PlayerTurn) {
            if (EnemyManager.Instance.AreThereEnemiesLeft()) {
                OnEnemyTurn();
            }
            else {
                OnPlayerTurn();
            }
        }
        else {
            BattleFinished = OnPlayerTurn;
        }
    }

    private void OnEnemyFinishedTurn() {
        if (currentTurn == Turn.EnemyTurn) {
            OnPlayerTurn();
        }
        else {
            BattleFinished = OnEnemyTurn;
        }
    }

    private void OnBattleStarted() {
        if (currentTurn != Turn.InBattle) {
            lastTurn = currentTurn;
            currentTurn = Turn.InBattle;
        }
    }

    private void OnBattleFinished() {
        currentTurn = lastTurn;
        BattleFinished?.Invoke();
    }
}
