using Sirenix.OdinInspector;
using UnityEngine;
using Action = System.Action;

public class PlayerElement : MonoBehaviour, ITargeter, ITargetable {
    public ElementType currentElement;
    [SerializeField] private bool changeElement;
    [SerializeField, ShowIf("changeElement"), ReadOnly]
    private int turnsToChangeElement;
    [ShowIf("changeElement")]
    public int turnCount;

    private PlayerController playerController;

    public event Action FinishedTurn;

    private void Awake() {
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
                int elementNum = (int) currentElement;
                elementNum++;
                if (elementNum > 2) {
                    elementNum = 0;
                }
                currentElement = (ElementType) elementNum;
                turnCount = 0;
                SendMove();
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
