public class SequenceMoveSelector : EnemyMoveSelector {
	public ElementType[] moves;

	public override ElementType GetMove() {
		ElementType move = moves[moveCount];
		moveCount = Utils.Repeat(moveCount + 1, 0, moves.Length);
		return move;
	}
}
