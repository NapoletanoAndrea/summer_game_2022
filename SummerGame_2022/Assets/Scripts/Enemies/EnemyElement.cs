using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyElement : MonoBehaviour
{
    [SerializeField] private ElementType startingElement;
    [SerializeField, ReadOnly] private ElementType currentElement;
    [SerializeField] private bool changeElement;
    [SerializeField, ShowIf("changeElement")]
    private int turnsToChangeElement;
    [SerializeField, ShowIf("changeElement")]
    private int turnCount;
    [SerializeField] private int bestOf;
    
    private EnemyBase enemyBase;

    public ElementType CurrentElement => currentElement;

    private void Awake() {
        currentElement = startingElement;
        enemyBase = GetComponent<EnemyBase>();
    }

    private void OnEnable() {
        enemyBase.Moved += ChangeElement;
    }

    private void OnDisable() {
        enemyBase.Moved -= ChangeElement;
    }

    private void ChangeElement() {
        if (changeElement) {
            turnCount++;
            if (turnCount == turnsToChangeElement) {
                int elementNum = (int) currentElement;
                elementNum++;
                if (elementNum > 2) {
                    elementNum = 0;
                }
                currentElement = (ElementType) elementNum;
                turnCount = 0;
            }
        }
    }
}
