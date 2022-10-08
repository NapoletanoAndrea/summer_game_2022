using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Action = System.Action;

public class BattleManager : MonoBehaviour {
    public static BattleManager Instance;

    public int playerWins;
    public int enemyWins;

    public int winsNeeded;

    public EnemyElement enemyElement;
    public EnemyMoveSelector enemyMoveSelector;
    public ElementType currentEnemyMove;

    public event Action BattleStarted;
    public event Action BattleFinished;

    public event Action<ElementType> BattleTurnStarted;

    private void Awake() {
        Instance = this;
    }

    public void StartBattle(EnemyElement enemy, ElementType playerElement, ElementType enemyElement) {
        winsNeeded = enemy.bestOf;
        playerWins = 0;
        enemyWins = 0;

        this.enemyElement = enemy;
        enemyMoveSelector = enemy.MoveSelector;

        switch (playerElement.Compare(enemyElement)) {
            case -1:
                enemyWins++;
                if (enemyWins >= winsNeeded) {
                    SceneLoader.Instance.Reload();
                }
                return;
            case 1:
                playerWins++;
                if (playerWins >= winsNeeded) {
                    enemy.Die();
                }
                return;
        }
        
        BattleStarted?.Invoke();
        StartBattleTurn();
    }

    private void StartBattleTurn() {
        currentEnemyMove = enemyMoveSelector.GetMove();
        BattleTurnStarted?.Invoke(currentEnemyMove);
    }

    public void SelectElement(ElementType elementType, bool missed) {
        if (missed) {
            EnemyWin();
        }
        else {
            switch (elementType.Compare(currentEnemyMove)) {
                case -1:
                    EnemyWin();
                    break;
                case 1:
                    PlayerWin();
                    break;
            }
        }

        if (TurnManager.Instance.CurrentTurn == Turn.InBattle) {
            Invoke(nameof(StartBattleTurn), 1);
        }
    }

    private void EnemyWin() {
        enemyWins++;
        if (enemyWins >= winsNeeded) {
            SceneLoader.Instance.Reload();
        }
    }

    private void PlayerWin() {
        playerWins++;
        if (playerWins >= winsNeeded) {
            enemyElement.Die();
            BattleFinished?.Invoke();
        }
    }
}
