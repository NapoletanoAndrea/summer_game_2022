using Sirenix.OdinInspector;
using UnityEngine;
using Action = System.Action;

public class PlayerElement : MonoBehaviour, ITargeter, ITargetable {
    [SerializeField] private ElementType startingElement;
    [SerializeField, ReadOnly] private ElementType currentElement;
    [SerializeField] private bool changeElement;
    [SerializeField, ShowIf("changeElement")]
    private int turnsToChangeElement;
    [SerializeField, ShowIf("changeElement")]
    private int turnCount;

    private PlayerController playerController;

    public event Action FinishedTurn;

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
            if (turnCount >= turnsToChangeElement) {
                SendMove();
                int elementNum = (int) currentElement;
                elementNum++;
                if (elementNum > 2) {
                    elementNum = 0;
                }
                currentElement = (ElementType) elementNum;
                turnCount = 0;
            }
            TurnManager.Instance.turnsLeftForMoveExecution = turnsToChangeElement - turnCount;
            FinishedTurn?.Invoke();
        }
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
        if (mono != null && mono.gameObject.CompareTag("Enemy")) {
            BattleManager.Instance.StartBattle((EnemyElement) mono, currentElement, move.moveType);
        }
    }
}
