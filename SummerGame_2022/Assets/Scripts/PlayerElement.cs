using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerElement : MonoBehaviour {
    [SerializeField] private ElementType startingElement;
    [SerializeField, ReadOnly] private ElementType currentElement;
    [SerializeField] private bool changeElement;
    [SerializeField, ShowIf("changeElement")]
    private int turnsToChangeElement;

    private PlayerController playerController;
    private int turnCount;

    private void Awake() {
        currentElement = startingElement;
        playerController = GetComponent<PlayerController>();
    }

    private void OnEnable() {
        playerController.Moved += ChangeElement;
    }

    private void OnDisable() {
        playerController.Moved -= ChangeElement;
    }

    private void ChangeElement() {
        if (changeElement) {
            turnCount++;
            if (turnCount == turnsToChangeElement) {
                int elementNum = (int) currentElement;
                elementNum++;
                if (elementNum > 2) {
                    currentElement = 0;
                }
                turnCount = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        EnemyElement enemyElement = col.GetComponent<EnemyElement>();
        if (enemyElement) {
            int compareValue = Elements.Compare(currentElement, enemyElement.CurrentElement);
            if (compareValue == 1) {
                Destroy(enemyElement.gameObject);
            }
            if (compareValue == -1) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
