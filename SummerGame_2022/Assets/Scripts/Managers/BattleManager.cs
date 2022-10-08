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

    public EnemyMoveSelector enemyMoveSelector;

    public event Action BattleStarted;
    public event Action BattleFinished;

    private void Awake() {
        Instance = this;
    }

    public void StartBattle(EnemyElement enemy, ElementType playerElement, ElementType enemyElement) {
        winsNeeded = enemy.bestOf;
        playerWins = 0;
        enemyWins = 0;

        switch (playerElement.Compare(enemyElement)) {
            case -1:
                enemyWins++;
                if (enemyWins >= winsNeeded) {
                    SceneLoader.Instance.Reload();
                    return;
                }
                break;
            case 1:
                playerWins++;
                if (playerWins >= winsNeeded) {
                    enemy.Die();
                    return;
                }
                break;
        }

        enemyMoveSelector = enemy.MoveSelector;
        BattleStarted?.Invoke();
    }
}
