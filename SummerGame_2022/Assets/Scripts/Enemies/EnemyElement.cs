using Sirenix.OdinInspector;
using UnityEngine;
using Action = System.Action;

public class EnemyElement : MonoBehaviour, ITargeter, ITargetable
{
    public ElementType startingElement;
    [SerializeField, ReadOnly] private ElementType currentElement;
    [SerializeField] private bool changeElement;
    [SerializeField, ShowIf("changeElement")]
    private int turnsToChangeElement;
    [SerializeField, ShowIf("changeElement")]
    private int changeElementTurnCount;
    [SerializeField] private bool attack;
    [SerializeField, ShowIf("attack")]
    private int attackTurns;
    [SerializeField, ShowIf("attack")]
    private int attackTurnCount;
    [Min(1)] public int bestOf;
    
    private EnemyBase enemyBase;
    private int health;

    public ElementType CurrentElement => currentElement;

    public event Action FinishedTurn;

    private void Awake() {
        currentElement = startingElement;
        enemyBase = GetComponent<EnemyBase>();
        health = bestOf;
    }

    private void OnEnable() {
        enemyBase.Moved += OnEnemyTurn;
    }

    private void Start() {
        EnemyManager.Instance.Subscribe(this);
    }

    private void OnDisable() {
        EnemyManager.Instance.Unsubscribe(this);
        enemyBase.Moved -= OnEnemyTurn;
    }

    private void OnEnemyTurn() {
        if (changeElement) {
            ChangeElement();
        }
        if (attack) {
            attackTurnCount++;
            if (attackTurnCount >= attackTurns) {
                SendMove();
                attackTurnCount = 0;
            }
        }
        FinishedTurn?.Invoke();
    }

    private void ChangeElement() {
        changeElementTurnCount++;
        if (changeElementTurnCount >= turnsToChangeElement) {
            int elementNum = (int) currentElement;
            elementNum++;
            if (elementNum > 2) {
                elementNum = 0;
            }
            currentElement = (ElementType) elementNum;
            changeElementTurnCount = 0;
        }
    }

    public void Die() {
        LevelManager.Instance.enemiesKilled.Add(this);
        gameObject.SetActive(false);
    }
    
    public void SendMove() {
        MoveData move = new (
            currentElement,
            this,
            GridUtils.GetCrossPattern(transform.position)
        );
        GridUtils.AttackPosition(move);
    }

    public void ReceiveMove(MoveData move) {
        var mono = move.sender as MonoBehaviour;
        if (mono != null && mono.gameObject.CompareTag("Player")) {
            BattleManager.Instance.StartBattle(this, move.moveType, currentElement);
        }
    }
}
