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

    public event Action BattleStarted;
    public event Action BattleFinished;

    private void Awake() {
        Instance = this;
    }

    public void StartBattle(EnemyElement enemy, ElementType playerElement, ElementType enemyElement) {
        winsNeeded = enemy.bestOf;
        switch (playerElement.Compare(enemyElement)) {
            case -1:
                enemyWins++;
                if (enemyWins >= winsNeeded) {
                    SceneLoader.Instance.Reload();
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
        //BattleStarted?.Invoke();
    }
}
