using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyElement : MonoBehaviour, ITargetable
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
    private int health;

    public ElementType CurrentElement => currentElement;

    private void Awake() {
        currentElement = startingElement;
        enemyBase = GetComponent<EnemyBase>();
        health = bestOf;
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

    private void TakeDamage() {
        health--;
        if (health <= 0) {
            gameObject.SetActive(false);
        }
    }

    public void ReceiveMove(MoveData move) {
        var mono = move.sender as MonoBehaviour;
        if (mono != null && mono.gameObject.CompareTag("Player")) {
            switch (currentElement.Compare(move.moveType)) {
                case -1:
                    TakeDamage();
                    break;
                case 0:
                    break;
                case 1:
                    break;
            }
        }
    }
}
